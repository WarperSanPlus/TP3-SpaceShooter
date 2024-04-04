using Controllers;
using Emetters;
using Entities;
using Extensions;
using Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Aya : BaseController, IResetable, IPredicatable
{
    public int State = 0;

    [SerializeField]
    private BaseEmetter WindChargeEmetter;

    private BaseEntity Self;

    public override void OnReset()
    {
        this.Self ??= this.GetComponent<BaseEntity>();
        this.State = 0;
        this.Reset_Phase1();
        base.OnReset();
    }

    private void Start() => this.Start_Phase1();

    protected override void OnTimerAdvanced(float timer, float elapsed)
    {
        if (this.State == 0)
        {
            foreach (BaseEmetter item in this.Phase1_Emetters)
                _ = item.Tick(elapsed);
        }
    }

    #region Predicates

    private readonly List<Guid> ActivePredicates = new();

    private void ClearAllPredicates()
    {
        // Delete all active predicates
        foreach (Guid item in this.ActivePredicates)
            this.Remove(item);
        this.ActivePredicates.Clear();
    }

    private Guid AddPredicate(System.Func<float, bool> condition)
    {
        Guid guid = this.Add(condition);
        this.ActivePredicates.Add(guid);

        return guid;
    }

    #endregion Predicates

    #region Phase 1

    [Header("Phase 1")]
    [SerializeField]
    private BaseEmetter[] Phase1_Emetters;

    [SerializeField]
    private GameObject Phase1_InvincibleSprite;

    private float Phase1_StunTimer;

    private void Reset_Phase1()
    {
        this.InitEmetter(this.Phase1_Emetters);
        this.Phase1_StunTimer = 20;
    }

    private void Start_Phase1()
    {
        // Become invincible
        this.Self.isInvicible = true;

        // Show invincible display
        if (this.Phase1_InvincibleSprite != null)
            this.Phase1_InvincibleSprite.SetActive(true);

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
                this.ActivePredicates.Add(entity.Link(this.Self));
                orbs[i] = entity;
            }
        }

        // Link orbs to Aya
        _ = this.AddPredicate(t => Predicates.WaitForSomeEntities.IsValid(orbs, orbs.Length));

        // Become invincible
        // Go to middle of the screen

        // Start Emetters
        this.InitEmetter(this.Phase1_Emetters);
    }

    private void Trigger_Phase1()
    {
        // Go to next phase
        this.State = 1;

        // Become hittable
        this.Self.isInvicible = false;

        // Hide invincible display
        if (this.Phase1_InvincibleSprite != null)
            this.Phase1_InvincibleSprite.SetActive(false);

        // Delete all active predicates
        this.ClearAllPredicates();

        // Stun boss
        _ = this.AddPredicate(t =>
        {
            this.Phase1_StunTimer -= t;

            return this.Phase1_StunTimer <= 0;
        });
    }

    #endregion Phase 1

    #region IPredicatable

    /// <inheritdoc/>
    public void Trigger()
    {
        if (this.State == 0)
            this.Trigger_Phase1();
    }

    #endregion IPredicatable
}