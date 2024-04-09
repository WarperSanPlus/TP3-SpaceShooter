using Bullets;
using Singletons;
using UnityEngine;

public class BounceOnScreen : MonoBehaviour
{
    private BaseBullet bullet;
    private Vector2 minPosition;
    private Vector2 maxPosition;
    [SerializeField] private Vector2 playerSize;

    private void Start()
    {
        this.bullet = this.GetComponent<BaseBullet>();

        // Disable if not found
        if (this.bullet == null)
        {
            this.enabled = false;
            return;
        }

        this.minPosition = SceneScalingManager.GetMin(this.playerSize / 2);
        this.maxPosition = SceneScalingManager.GetMax(this.playerSize / 2);
    }

    private void FixedUpdate()
    {
        var angle = this.GetBounceAngle(Time.fixedDeltaTime);
        if (angle != 0)
            this.Bounce(angle);
    }

    private float GetBounceAngle(float elapsed)
    {
        // Get current angle
        var angleDir = Mathf.Deg2Rad * this.transform.rotation.eulerAngles.z;

        // Get the direction of the object
        Vector2 dir = new(Mathf.Cos(angleDir), Mathf.Sin(angleDir));

        // Get the next position of the bullet
        Vector3 nextPos = this.bullet.GetNextPosition(elapsed);

        var outsideX = nextPos.x < this.minPosition.x || nextPos.x > this.maxPosition.x;
        var outsideY = nextPos.y < this.minPosition.y || nextPos.y > this.maxPosition.y;

        // If not going outside
        if (!outsideX && !outsideY)
            return 0;

        var differentDirections = Mathf.Sign(dir.y) != Mathf.Sign(dir.x);
        var angle = outsideX ? differentDirections ? 1 : -1 : (float)(differentDirections ? -1 : 1);
        var diff = Mathf.Abs(this.transform.rotation.eulerAngles.z % 90);

        if (angle < 0)
            diff = 90 - diff;

        angle *= 180 - (2 * diff);
        return angle;
    }

    /// <summary>
    /// Rotates the object by <paramref name="rotation"/> degrees
    /// </summary>
    private void Bounce(float rotation) => this.transform.Rotate(0, 0, rotation);
}