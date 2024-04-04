using System;
using UnityEngine;

namespace Emetters
{
    public class RangeEmetter : BaseEmetter
    {
        [Header("Range Emetter")]
        [SerializeField, Min(0)]
        private float rangeWidth;

        [SerializeField, Range(0, 1)]
        private float leftBound = 1;

        [SerializeField, Range(0, 1)]
        private float rightBound = 1;

        [SerializeField, Min(0)]
        private int minProjectileCount = 1;

        [SerializeField, Min(0)]
        private int maxProjectileCount = 2;

        [Header("Projectile Size")]
        [SerializeField, Min(0)]
        private float minProjectileSize = 1f;

        [SerializeField, Min(0)]
        private float maxProjectileSize = 2f;

        private Vector3 GetPos(float boundValue)
        {
            var angle = Mathf.Deg2Rad * this.transform.rotation.eulerAngles.z;
            var boundsRotation = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));

            boundValue = Mathf.Clamp(boundValue, -1, 1);

            return this.transform.position + (boundValue * -this.rangeWidth * boundsRotation / 2);
        }

        #region BaseEmetter

        /// <inheritdoc/>
        protected override Vector3 GetOrigin(int index)
            => this.GetPos(UnityEngine.Random.Range(-this.rightBound, this.leftBound));

        /// <inheritdoc/>
        protected override Quaternion GetRotation(int index) => this.transform.rotation;

        /// <inheritdoc/>
        protected override int GetProjectileCount()
            => UnityEngine.Random.Range(this.minProjectileCount, this.maxProjectileCount);

        /// <inheritdoc/>
        protected override GameObject GetProjectile(int index)
        {
            GameObject proj = base.GetProjectile(index);

            proj.transform.localScale = Vector3.one * UnityEngine.Random.Range(this.minProjectileSize, this.maxProjectileSize);

            return proj;
        }

        #endregion BaseEmetter

        #region Gizmos

        /// <inheritdoc/>
        private void OnDrawGizmosSelected()
        {
            // Draw max bounds
            Gizmos.color = Color.green;

            // Min
            Gizmos.DrawRay(this.GetPos(-1), this.transform.up * 10);

            // Max
            Gizmos.DrawRay(this.GetPos(1), this.transform.up * 10);

            // Draw bounds
            Gizmos.color = Color.red;
            Gizmos.DrawRay(this.GetPos(-this.rightBound), this.transform.up * 5);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(this.GetPos(this.leftBound), this.transform.up * 5);
        }

        #endregion Gizmos
    }
}