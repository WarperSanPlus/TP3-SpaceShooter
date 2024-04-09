using Controllers;
using Emetters;
using Entities;
using Extensions;
using Interfaces;
using Singletons;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Aya : BaseController, IResetable, IPredicatable
{
    [SerializeField]
    private BaseEmetter WindChargeEmetter;

    private BaseEntity Self;

    public override void OnReset()
    {
        this.Self ??= this.GetComponent<BaseEntity>();

        this.Reset_Phase1();

        base.OnReset();
    }

    private void Start()
    {
        this.Self.onHealthChanged += this.HealthUpdate;
        this.Start_Phase1();
    }

    protected override void OnTimerAdvanced(float timer, float elapsed)
    {
        foreach (BaseEmetter item in this.Emetters)
            _ = item.Tick(elapsed);
    }

    #region UI

    [SerializeField]
    private Slider healthBar;

    private void HealthUpdate(float newHealth, float oldHealth, float maxHealth)
    {
        if (this.healthBar == null)
            return;

        this.healthBar.value = newHealth / maxHealth;
    }

    #endregion

    #region Predicates

    private readonly List<(Guid guid, Action callback)> ActivePredicates = new();

    private void ClearAllPredicates()
    {
        // Delete all active predicates
        foreach ((Guid item, _) in this.ActivePredicates)
            this.Remove(item);
        this.ActivePredicates.Clear();
    }

    private Guid AddPredicate(Func<float, bool> condition, Action callback = null)
        => this.AddPredicate(this.Add(condition), callback);

    private Guid AddPredicate(Guid guid, Action callback = null)
    {
        this.ActivePredicates.Add((guid, callback));
        return guid;
    }

    #endregion Predicates

    [SerializeField]
    private BaseEmetter[] Emetters;

    [SerializeField]
    private GameObject InvincibleSprite;

    private float StunTimer;

    private void Reset_Phase1()
    {
        this.InitEmetter(this.Emetters);
        foreach (BaseEmetter item in this.Emetters)
            item.SetActive(true);

        this.StunTimer = 20;
    }

    private void Start_Phase1()
    {
        // Become invincible
        this.Self.isInvicible = true;

        // Show invincible display
        if (this.InvincibleSprite != null)
            this.InvincibleSprite.SetActive(true);

        // Enable all emetters
        foreach (BaseEmetter item in this.Emetters)
            item.gameObject.SetActive(true);

        // Spawns orbs
        const int ORB_COUNT = 2;

        var orbs = new BaseEntity[ORB_COUNT];

        for (var i = 0; i < ORB_COUNT; i++)
        {
            GameObject windCharge = this.WindChargeEmetter.ShootProjectile(i);

            if (windCharge == null)
                continue;

            // Random orientation
            windCharge.transform.Rotate(0, 0, UnityEngine.Random.Range(0, 360));

            // Link Aya to orb
            if (windCharge.TryGetComponent(out SubEntity entity))
            {
                _ = this.AddPredicate(entity.Link(this.Self));
                orbs[i] = entity;
            }
        }

        // Link orbs to Aya
        _ = this.AddPredicate(
            t => Predicates.WaitForSomeEntities.IsValid(orbs, orbs.Length),
            this.Trigger_Phase1);

        // Start Emetters
        this.InitEmetter(this.Emetters);
    }

    private void Trigger_Phase1()
    {
        // Become hittable
        this.Self.isInvicible = false;

        // Hide invincible display
        if (this.InvincibleSprite != null)
            this.InvincibleSprite.SetActive(false);

        // Delete all active predicates
        this.ClearAllPredicates();

        // Delete all projectiles
        this.DestroyBullets();

        GameObject obj = ObjectPool.GetPooledObject("Flashbang", "Effects");
        obj.transform.position = this.transform.position;
        obj.SetActive(true);

        // Disable all emetters
        foreach (BaseEmetter item in this.Emetters)
            item.gameObject.SetActive(false);

        var timer = this.StunTimer;
        var nextHealth = this.Self.Health - 50;

        // Stun boss
        _ = this.AddPredicate(t =>
        {
            timer -= t;

            return timer <= 0 || this.Self.Health < nextHealth;
        }, this.Start_Phase1);
    }

    #region IPredicatable

    /// <inheritdoc/>
    public void Trigger(Guid guid)
    {
        // Find Predicate
        var index = this.ActivePredicates.FindIndex(p => p.guid == guid);

        if (index == -1)
            return;

        (Guid guid, Action callback) predicate = this.ActivePredicates[index];

        // Call callback
        predicate.callback?.Invoke();
    }

    #endregion IPredicatable
}