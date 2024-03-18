using Entities;
using Interfaces;
using UnityEngine;

[RequireComponent(typeof(BaseEntity))]
[RequireComponent(typeof(Animator))]
public class EnemyEnter : MonoBehaviour, IResetable
{
    private const float TO_IDLE_PERCENT = 0.75f;
    private const string TO_IDLE_FLAG = "isIdling";

    [SerializeField, Tooltip("Amounf of seconds needed to travel to the end position")]
    private float duration = 1f;

    [SerializeField, Min(0)]
    private float delay = 0f;

    [SerializeField, Tooltip("Determines the height of the movement")]
    private float height = 5f;

    [SerializeField, Tooltip("Determines the final position of the movement")]
    private Vector3 end;

    private Vector3 start;
    private float time;

    #region MonoBehaviour

    /// <inheritdoc/>
    private void FixedUpdate()
    {
        this.delay -= Time.fixedDeltaTime;

        if (this.delay > 0f)
            return;

        if (this.duration > 0)
        {
            if (this.entity != null)
            {
                this.entity.isInvicible = false;
                this.entity = null;
            }

            this.transform.position = Parabola(this.start, this.end, this.height, this.time / this.duration);

            this.CheckAnimation();

            this.time += Time.fixedDeltaTime;
        }

        if (this.time >= this.duration)
        {
            this.enabled = false;
            this.transform.position = this.end;
        }
    }

    private void OnDisable() => this.SetAllActivatables(true);

    #endregion

    #region IActivatable

    private IActivatable[] onArrived;

    private void SetAllActivatables(bool isActive)
    {
        if (this.onArrived == null)
            return;

        foreach (IActivatable item in this.onArrived)
            item.SetActive(isActive);
    }

    #endregion

    #region IResetable

    public void OnReset()
    {
        this.entity = this.entity != null ? this.entity : this.gameObject.GetComponent<BaseEntity>();

        this.start = this.transform.position;

        this.onArrived ??= this.gameObject.GetComponents<IActivatable>();
        this.SetAllActivatables(false);
        this.enabled = true;
        this.time = 0;

        // Animator
        this.animator = this.animator != null ? this.animator : this.gameObject.GetComponent<Animator>();
        this.animator.SetBool(TO_IDLE_FLAG, false);
        this.toIdleFlag = false;
    }

    #endregion

    #region BaseEntity

    private BaseEntity entity = null;

    #endregion

    #region Animator

    private Animator animator;
    private bool toIdleFlag = false;

    private void CheckAnimation()
    {
        if (this.toIdleFlag)
            return;

        if (this.time / this.duration < TO_IDLE_PERCENT)
            return;

        this.animator.SetBool(TO_IDLE_FLAG, true);
        this.toIdleFlag = true;
    }

    #endregion

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

    #endregion

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

    #endregion
}
