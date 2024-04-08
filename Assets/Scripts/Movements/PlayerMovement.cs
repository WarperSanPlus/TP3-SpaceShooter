using UnityEngine;
using UnityEngine.InputSystem;

namespace Movements
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        private Rigidbody2D rb;

        // Start is called before the first frame update
        private void Start() => this.rb = this.gameObject.GetComponent<Rigidbody2D>();

        private void FixedUpdate() => this.MoveTowardsTarget();


        #region Move

        private Vector2 direction;
        [SerializeField, Min(0)] 
        private float speed;

        [Header("Capping Position")]
        [SerializeField] private Vector2 minPosition;
        [SerializeField] private Vector2 maxPosition;

        public void OnMove(InputAction.CallbackContext ctx)
        {
            this.SetDirection(ctx.ReadValue<Vector2>());
        }

        private void MoveTowardsTarget()
        {
            var currentSpeed = this.isSneaking ? this.sneakSpeed : this.speed;
            Vector3 nextPosition = this.transform.position + (Vector3)(this.direction * currentSpeed);

            // Cap next position
            nextPosition.x = Mathf.Clamp(nextPosition.x, this.minPosition.x, this.maxPosition.x);
            nextPosition.y = Mathf.Clamp(nextPosition.y, this.minPosition.y, this.maxPosition.y);

            this.rb.MovePosition(nextPosition);
        }

        public void SetDirection(Vector2 direction)
        {
            this.direction = direction;
            this.UpdateAnimator();
        }
        #endregion

        #region Sneak
        [SerializeField, Min(0)]
        private float sneakSpeed;
        private bool isSneaking = false;

        public void OnSneak(InputAction.CallbackContext ctx)
        {
            SetSneak(ctx.ReadValueAsButton());
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

        #endregion
    }
}