using Interfaces;
using UnityEngine;

namespace UtilityScripts
{
    public class MoveByVector : MonoBehaviour, IActivatable
    {
        [Tooltip("Direction of the movement")]
        public Vector3 direction;

        [SerializeField, Tooltip("By how much the movement is sped up")]
        public float speed;

        // Update is called once per frame
        private void FixedUpdate() 
            => this.transform.Translate(this.GetDirection(), Space.World);

        private Vector3 GetDirection() => this.GetDirection(this.direction);
        public Vector3 GetDirection(Vector3 dir) => this.speed * Time.fixedDeltaTime * dir.normalized;

        #region IEnterActivation

        /// <inheritdoc/>
        public void SetActive(bool isActive) => this.enabled = isActive;

        #endregion IEnterActivation

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if (!this.enabled)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(this.transform.position, this.transform.position + (this.GetDirection() * 10));
        }

        #endregion

    }
}
