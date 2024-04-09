using UnityEngine;
using UnityEngine.InputSystem;

namespace Movements
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        #region MonoBehaviour

        /// <inheritdoc/>
        private void Start()
        {
            this.rb = this.gameObject.GetComponent<Rigidbody2D>();

            this.minPosition = Singletons.SceneScalingManager.GetMin(this.playerSize / 2);
            this.maxPosition = Singletons.SceneScalingManager.GetMax(this.playerSize / 2);
        }

        /// <inheritdoc/>
        private void FixedUpdate() => this.MoveTowardsTarget();

        #endregion

        #region Move
        
        private Rigidbody2D rb;

        private Vector2 direction;

        [SerializeField, Min(0)]
        private float speed;

        [SerializeField, Min(0)]
        private float sneakSpeed;

        private bool isSneaking = false;

        public void OnMove(InputAction.CallbackContext ctx)
        {
            this.SetDirection(ctx.ReadValue<Vector2>());
        }

        public void OnSneak(InputAction.CallbackContext ctx) => this.SetSneak(ctx.ReadValueAsButton());

        private void MoveTowardsTarget()
        {
            var currentSpeed = this.isSneaking ? this.sneakSpeed : this.speed;
            Vector3 nextPosition = this.transform.position + (Vector3)(this.direction * currentSpeed);

            this.rb.MovePosition(this.ClampPosition(nextPosition));
        }

        public void SetDirection(Vector2 direction)
        {
            this.direction = direction;
            this.UpdateAnimator();
        }
        #endregion Move

        #region Capping Position

        [Header("Capping Position")]
        [SerializeField]
        private Vector2 playerSize;

        private Vector2 minPosition;
        private Vector2 maxPosition;

        private Vector3 ClampPosition(Vector3 position)
        {
            // Clamp next position
            position.x = Mathf.Clamp(position.x, this.minPosition.x, this.maxPosition.x);
            position.y = Mathf.Clamp(position.y, this.minPosition.y, this.maxPosition.y);

            return position;
        }

        public void SetSneak(bool sneak)
        {
            this.isSneaking = sneak;
        }
        #endregion

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