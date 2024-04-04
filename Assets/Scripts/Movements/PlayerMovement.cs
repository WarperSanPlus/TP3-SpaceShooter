using UnityEngine;
using UnityEngine.InputSystem;

namespace Movements
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        private Rigidbody2D rb;

        // Start is called before the first frame update
        private void Start()
        {
            this.rb = this.gameObject.GetComponent<Rigidbody2D>();

            Camera cam = Camera.main;
            Vector3 origin = cam.gameObject.transform.position;

            // https://youtu.be/ailbszpt_AI?si=aBLqcL0_CV5yxHin
            Vector3 stageDimensions = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

            this.minPosition.x = origin.x - stageDimensions.x + (this.playerSize.x / 2);
            this.minPosition.y = origin.y - stageDimensions.y + (this.playerSize.y / 2);
            this.maxPosition.x = origin.x + stageDimensions.x - (this.playerSize.x / 2);
            this.maxPosition.y = origin.y + stageDimensions.y - (this.playerSize.y / 2);
        }

        private void FixedUpdate() => this.MoveTowardsTarget();

        #region Move

        private Vector2 direction;

        [SerializeField, Min(0)]
        private float speed;

        [SerializeField, Min(0)]
        private float sneakSpeed;

        private bool isSneaking = false;

        public void OnMove(InputAction.CallbackContext ctx)
        {
            this.direction = ctx.ReadValue<Vector2>();
            this.UpdateAnimator();
        }

        public void OnSneak(InputAction.CallbackContext ctx) => this.isSneaking = ctx.ReadValueAsButton();

        [Header("Capping Position")]
        [SerializeField] private Vector2 playerSize;

        private Vector2 minPosition;
        private Vector2 maxPosition;

        private void MoveTowardsTarget()
        {
            var currentSpeed = this.isSneaking ? this.sneakSpeed : this.speed;
            Vector3 nextPosition = this.transform.position + (Vector3)(this.direction * currentSpeed);

            // Cap next position
            nextPosition.x = Mathf.Clamp(nextPosition.x, this.minPosition.x, this.maxPosition.x);
            nextPosition.y = Mathf.Clamp(nextPosition.y, this.minPosition.y, this.maxPosition.y);

            this.rb.MovePosition(nextPosition);
        }

        #endregion Move

        #region Animator

        [Header("Animation")]
        [SerializeField]
        private Animator playerAnimator;

        private void UpdateAnimator()
        {
            if (this.playerAnimator == null)
                return;

            this.playerAnimator.SetBool("isGoingRight", this.direction.x > 0);
            this.playerAnimator.SetBool("isGoingLeft", this.direction.x < 0);
        }

        #endregion Animator

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if (!this.enabled)
                return;

            Gizmos.DrawWireCube(this.transform.position, this.playerSize);
        }

        #endregion Gizmos
    }
}