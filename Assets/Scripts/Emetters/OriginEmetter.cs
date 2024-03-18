using UnityEngine;

namespace Emetters
{
    /// <summary>
    /// Class that allows to shoot at given points
    /// </summary>
    public class OriginEmetter : BaseEmetter
    {
        [Header("Origin Emetter")]
        [SerializeField, Tooltip("Determines if the projectiles should use the rotation of the origin")]
        private bool useOriginRotation = true;

        [SerializeField, Tooltip("List of the origins that the emetter will shoot from")]
        private Transform[] origins;

        /// <returns>Target to use for the projectile at <paramref name="index"/></returns>
        /// <remarks>If the index is invalid, it will return null</remarks>
        private Transform GetTarget(int index) => this.origins != null && this.origins.Length > index ? this.origins[index] : null;

        #region BaseEmetter

        /// <inheritdoc/>
        protected override int GetProjectileCount() => this.origins == null ? 0 : this.origins.Length;

        /// <inheritdoc/>
        protected override Vector3 GetOrigin(int index)
        {
            Transform target = this.GetTarget(index);

            return target == null ? base.GetOrigin(index) : target.position;
        }

        /// <inheritdoc/>
        protected override Quaternion GetRotation(int index)
        {
            if (!this.useOriginRotation)
                return base.GetRotation(index);

            Transform target = this.GetTarget(index);

            return target != null ? target.rotation : throw new System.NullReferenceException();
        }

        #endregion
    }
}