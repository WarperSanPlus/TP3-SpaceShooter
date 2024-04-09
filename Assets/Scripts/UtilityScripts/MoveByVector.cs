using Interfaces;
using UnityEngine;

namespace UtilityScripts
{
    public class MoveByVector : MonoBehaviour, IActivatable
    {
        [Tooltip("Direction of the movement")]
        public Vector3 direction;

        [Tooltip("By how much the movement is sped up")]
        public float speed;

        private Vector3 Direction => this.GetDirection(this.direction, Time.fixedDeltaTime);

        public Vector3 GetDirection(Vector3 dir, float elapsed) => this.speed * elapsed * dir.normalized;

        #region MonoBehaviour

        /// <inheritdoc/>
        private void FixedUpdate() => this.transform.Translate(this.Direction, Space.World);

        #endregion

        #region IActivatable

        /// <inheritdoc/>
        public void SetActive(bool isActive) => this.enabled = isActive;

        #endregion IActivatable

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if (!this.enabled)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(this.transform.position, this.transform.position + (this.Direction * 10));
        }

        #endregion Gizmos
    }
}