using Interfaces;
using UnityEngine;
using UtilityScripts;

namespace Movements
{
    [RequireComponent(typeof(MoveByVector))]
    public class ZigZagMovement : MonoBehaviour, IActivatable, IResetable
    {
        [SerializeField]
        private float magnitude;

        [SerializeField]
        private float speed;

        private MoveByVector moveByVector;

        [SerializeField]
        private bool isMovementVertical = true;

        private float timer;

        private float GetPosition(float x) => Mathf.Sin(x) * this.speed / 2;

        #region MonoBehaviour

        /// <inheritdoc/>
        private void Start() => this.moveByVector = this.gameObject.GetComponent<MoveByVector>();

        /// <inheritdoc/>
        private void FixedUpdate()
        {
            Vector3 dir = this.moveByVector.direction;

            var pos = this.GetPosition(this.timer / this.magnitude);

            if (this.isMovementVertical)
                dir.x = pos;
            else
                dir.y = pos;

            this.moveByVector.direction = dir;

            this.timer += Time.fixedDeltaTime;
        }

        #endregion

        #region IEnterActivation

        /// <inheritdoc/>
        public void SetActive(bool isActive) => this.enabled = isActive;

        #endregion IEnterActivation

        #region IResetable

        /// <inheritdoc/>
        public void OnReset()
        {
            this.timer = 0;
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if (!this.enabled || Application.isPlaying)
                return;

            this.moveByVector = this.gameObject.GetComponent<MoveByVector>();
            var speed = this.moveByVector.speed;

            if (speed == 0)
                return;

            Vector3 dirRef = this.moveByVector.direction;

            Gizmos.color = Color.blue;

            Vector3 currentPos = this.transform.position;

            var limit = 2 * Mathf.PI * this.magnitude;

            for (var i = 0f; i < limit; i += Time.fixedDeltaTime)
            {
                // Get direction
                Vector3 dir = dirRef;
                var pos = this.GetPosition(i / this.magnitude);

                if (this.isMovementVertical)
                    dir.x = pos;
                else
                    dir.y = pos;

                dir = this.moveByVector.GetDirection(dir);

                // Get next position
                Vector3 nextPos = currentPos + dir;

                Gizmos.DrawLine(currentPos, nextPos);

                currentPos = nextPos;
            }
        }

        #endregion
    }
}