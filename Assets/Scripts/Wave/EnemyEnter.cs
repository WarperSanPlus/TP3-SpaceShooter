using Entities;
using Interfaces;
using Predicates;
using UnityEngine;

public class EnemyEnter : MonoBehaviour, IResetable, IPredicatable
{
    private const float TO_IDLE_PERCENT = 0.75f;
    private const string TO_IDLE_FLAG = "isIdling";

    [SerializeField, Tooltip("Amounf of seconds needed to travel to the end position")]
    private float duration = 1f;

    private bool canStart = false;

    [SerializeField, Tooltip("Determines the height of the movement")]
    private float height = 5f;

    [SerializeField, Tooltip("Determines the final position of the movement")]
    private Vector3 end;

    private Vector3 start;
    private float time;

    public void Scale(Vector2 scale)
    {
        this.end *= scale;
        this.height *= scale.y;
    }

    #region MonoBehaviour

    /// <inheritdoc/>
    private void FixedUpdate()
    {
        if (!this.canStart)
            return;

        if (this.duration > 0)
        {
            if (this.entity != null)
            {
                if (this.entity.isInvicible)
                    this.entity.isInvicible = false;

                this.entity.transform.position = Parabola(this.start, this.end, this.height, this.time / this.duration);
            }

            this.time += Time.fixedDeltaTime;
        }

        if (this.time >= this.duration)
        {
            this.enabled = false;

            if (this.entity != null)
                this.entity.transform.position = this.end;
        }

        this.CheckAnimation();
    }

    private void OnDisable() => this.SetAllActivatables(true);

    #endregion MonoBehaviour

    #region IActivatable

    private IActivatable[] onArrived;

    private void SetAllActivatables(bool isActive)
    {
        if (this.onArrived == null)
            return;

        foreach (IActivatable item in this.onArrived)
            item.SetActive(isActive);
    }

    #endregion IActivatable

    #region IResetable

    public void OnReset()
    {
        this.start = this.entity == null ? this.transform.position : this.entity.transform.position;

        this.onArrived ??= this.transform.parent.GetComponents<IActivatable>();
        this.SetAllActivatables(false);
        this.enabled = true;
        this.time = 0;

        // Animator
        if (this.animator != null)
            this.animator.SetBool(TO_IDLE_FLAG, false);
        this.toIdleFlag = false;

        // Delay
        if (this.TryGetComponent(out Predicate predicate))
            predicate.Add(this);

        this.canStart = predicate == null;
    }

    #endregion IResetable

    #region IPredicatable

    /// <inheritdoc/>
    public void Trigger(System.Guid guid) => this.canStart = true;

    #endregion

    #region BaseEntity

    [SerializeField, Extensions.IgnoreCopy]
    private BaseEntity entity;

    #endregion BaseEntity

    #region Animator

    [SerializeField, Extensions.IgnoreCopy]
    private Animator animator;

    private bool toIdleFlag = false;

    private void CheckAnimation()
    {
        if (this.toIdleFlag || this.animator == null)
            return;

        if (this.duration == 0 || this.time / this.duration < TO_IDLE_PERCENT)
            return;

        this.animator.SetBool(TO_IDLE_FLAG, true);
        this.toIdleFlag = true;
    }

    #endregion Animator

    #region Static

    // From: https://gist.github.com/ditzel/68be36987d8e7c83d48f497294c66e08
    private static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
    {
        float f(float x)
        {
            return (-4 * height * x * x) + (4 * height * x);
        }

        var mid = Vector2.Lerp(start, end, t);

        return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
    }

    #endregion Static

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        // Only show when the script is active
        if (!this.enabled)
            return;

        Vector2 size = Vector2.one;

        // Get the correct starting position
        Vector3 start = Application.isPlaying ? this.start : this.transform.position;

        // Draw the curve
        for (float i = 0; i < this.duration; i += Time.fixedDeltaTime)
        {
            Vector2 pos = Parabola(start, this.end, this.height, i / this.duration);
            Gizmos.DrawCube(pos, size);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawCube(this.end, size);
    }

    #endregion Gizmos
}