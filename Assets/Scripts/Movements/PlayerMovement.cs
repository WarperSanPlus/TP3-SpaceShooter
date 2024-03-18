using UnityEngine;
using UnityEngine.InputSystem;

namespace Movements
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        private Rigidbody2D rb;
        private Vector2 direction;
        [Min(0)] public float speed;

        [Header("Capping Position")]
        [SerializeField] private Vector2 minPosition;
        [SerializeField] private Vector2 maxPosition;

        // Start is called before the first frame update
        private void Start() => this.rb = this.gameObject.GetComponent<Rigidbody2D>();

        public void OnMove(InputAction.CallbackContext ctx)
        {
            this.direction = ctx.ReadValue<Vector2>();
            this.UpdateAnimator();
        }

        private void FixedUpdate() => this.MoveTowardsTarget();

        private void MoveTowardsTarget()
        {
            Vector3 nextPosition = this.transform.position + (Vector3)(this.direction * this.speed);

            // Cap next position
            nextPosition.x = Mathf.Clamp(nextPosition.x, this.minPosition.x, this.maxPosition.x);
            nextPosition.y = Mathf.Clamp(nextPosition.y, this.minPosition.y, this.maxPosition.y);

            this.rb.MovePosition(nextPosition);
        }

        #region Animator

        [Header("Animation")]
        [SerializeField]
        private Animator playerAnimator;

        private void UpdateAnimator()
        {
            this.playerAnimator.SetBool("isGoingRight", this.direction.x > 0);
            this.playerAnimator.SetBool("isGoingLeft", this.direction.x < 0);
        }

        #endregion
    }
}