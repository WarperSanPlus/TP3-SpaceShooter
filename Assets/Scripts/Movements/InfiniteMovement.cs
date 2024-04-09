using Bullets;
using UnityEngine;

public class InfiniteMovement : MonoBehaviour
{
    [SerializeField, Tooltip("Direction of the rotation")]
    private Vector3 direction = Vector3.forward;

    [SerializeField, Min(0)]
    private float rightRadius = 1;
    private float rightSpeed;
    private float rightDuration;

    [SerializeField, Min(0)]
    private float leftRadius = 1;
    private float leftSpeed;
    private float leftDuration;

    private float timer;
    private bool goingRight = true;

    private void Start()
    {
        if (!this.TryGetComponent(out BaseBullet bullet))
        {
            this.enabled = false;
            return;
        }

        var elapsed = Time.fixedDeltaTime;
        var cirRight = Mathf.PI * 2 * this.rightRadius;
        var cirLeft = Mathf.PI * 2 * this.leftRadius;

        var speed = bullet.Speed * elapsed;
        this.rightDuration = cirRight / speed * elapsed;
        this.leftDuration = cirLeft / speed * elapsed;

        this.rightSpeed = -360 / (cirRight / speed) / elapsed;
        this.leftSpeed = 360 / (cirLeft / speed) / elapsed;

        this.goingRight = false;
        this.timer = this.leftDuration;
    }

    private void Advance(float elapsed)
    {
        if (this.timer <= 0)
        {
            this.goingRight = !this.goingRight;
            this.timer = this.goingRight ? this.rightDuration : this.leftDuration;
        }

        this.timer -= elapsed;

        var angle = this.goingRight ? this.rightSpeed : this.leftSpeed;

        this.transform.Rotate(angle * elapsed * this.direction.normalized);
    }

    #region MonoBehaviour

    /// <inheritdoc/>
    private void FixedUpdate() => this.Advance(Time.fixedDeltaTime);

    #endregion

    #region Gizmos

#if UNITY_EDITOR
    private Vector3 startPos;

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
            this.startPos = this.transform.position;

        Gizmos.DrawWireSphere(this.startPos + (Vector3.right * this.rightRadius), this.rightRadius);
        Gizmos.DrawWireSphere(this.startPos + (Vector3.left * this.leftRadius), this.leftRadius);
    }
#endif

    #endregion
}
