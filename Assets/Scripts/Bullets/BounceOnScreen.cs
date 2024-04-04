using Bullets;
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

        Camera cam = Camera.main;
        Vector3 origin = cam.gameObject.transform.position;

        // https://youtu.be/ailbszpt_AI?si=aBLqcL0_CV5yxHin
        Vector3 stageDimensions = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        this.minPosition.x = origin.x - stageDimensions.x + (this.playerSize.x / 2);
        this.minPosition.y = origin.y - stageDimensions.y + (this.playerSize.y / 2);
        this.maxPosition.x = origin.x + stageDimensions.x - (this.playerSize.x / 2);
        this.maxPosition.y = origin.y + stageDimensions.y - (this.playerSize.y / 2);
    }

    private void FixedUpdate()
    {
        // Angle
        var angleDir = Mathf.Deg2Rad * this.transform.rotation.eulerAngles.z;

        // Get direction
        Vector2 dir = new(Mathf.Cos(angleDir), Mathf.Sin(angleDir));

        // Get speed
        var speed = this.bullet.GetSpeed();

        // Get next position
        Vector3 nextPos = this.transform.position + (speed * Time.fixedDeltaTime * this.transform.forward);

        var outsideX = nextPos.x < this.minPosition.x || nextPos.x > this.maxPosition.x;
        var outsideY = nextPos.y < this.minPosition.y || nextPos.y > this.maxPosition.y;

        // If not going outside
        if (!outsideX && !outsideY)
            return;

        var differentDirections = Mathf.Sign(dir.y) != Mathf.Sign(dir.x);
        var angle = outsideX ? differentDirections ? 1 : -1 : (float)(differentDirections ? -1 : 1);
        var diff = Mathf.Abs(this.transform.rotation.eulerAngles.z % 90);

        if (angle < 0)
            diff = 90 - diff;

        angle *= 180 - (2 * diff);

        this.Bounce(angle);
    }

    private void Bounce(float rotation) => this.transform.Rotate(0, 0, rotation);
}