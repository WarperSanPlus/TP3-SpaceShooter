using UnityEngine;

namespace UtilityScripts
{
    public class RotateByVector : MonoBehaviour
    {
        [SerializeField, Tooltip("Direction of the rotation")]
        private Vector3 direction = Vector3.forward;

        [SerializeField, Tooltip("By how much the rotation is sped up")]
        private float speed = 1f;

        private Vector3 GetRotation(float elapsed) 
            => this.speed * elapsed * this.direction.normalized;

        #region MonoBehaviour

        /// <inheritdoc/>
        private void FixedUpdate()
            => this.transform.Rotate(this.GetRotation(Time.fixedDeltaTime));

        #endregion
    }
}