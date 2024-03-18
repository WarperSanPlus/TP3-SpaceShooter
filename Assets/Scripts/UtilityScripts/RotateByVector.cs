using UnityEngine;

namespace UtilityScripts
{
    public class RotateByVector : MonoBehaviour
    {
        [SerializeField, Tooltip("Direction of the rotation")]
        private Vector3 direction = Vector3.forward;

        [SerializeField, Tooltip("By how much the rotation is sped up")]
        private float speed = 1f;

        private void FixedUpdate() 
            => this.transform.Rotate(this.speed * Time.fixedDeltaTime * this.direction.normalized);
    }
}
