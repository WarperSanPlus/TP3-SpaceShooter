using UnityEngine;

namespace Emetters
{
    /// <summary>
    /// Class that allows to shoot in an arc-shaped form
    /// </summary>
    public class BuckshotEmetter : BaseEmetter
    {
        [Header("Buckshot Emetter")]
        [SerializeField, Min(0), Tooltip("Amount of bullets shot")]
        private int amount = 1;

        [SerializeField, Range(0, 360), Tooltip("Angle of the arc")]
        private float angle;

        [SerializeField, Tooltip("Determines if the emetter randomly chooses the angle")]
        private bool isRandomAngle = true;

        private float GetAngle(int index)
        {
            // Get direction of the buckshot
            Vector3 dir = -this.transform.up;

            // Get center of the arc
            var centerAngle = this.amount % 2 == 0 ? (this.amount / 2) - 0.5f : this.amount / 2;

            // Get by how much each rod rotate
            var angleVariation = this.angle / this.amount * Mathf.Deg2Rad;

            // Get the starting angle
            var angle = Mathf.Atan2(dir.y, dir.x) - ((this.amount - centerAngle) * angleVariation);

            // Calculate the angle
            angle += angleVariation * (this.isRandomAngle ? Random.Range(1f, this.amount) : index + 1);

            return angle;
        }

        #region BaseEmetter

        /// <inheritdoc/>
        protected override int GetProjectileCount() => this.amount;

        /// <inheritdoc/>
        protected override Vector3 GetOrigin(int index) => this.transform.position;

        /// <inheritdoc/>
        protected override Quaternion GetRotation(int index)
            => Quaternion.Euler(0, 0, (Mathf.Rad2Deg * this.GetAngle(index)) - 90f);

        #endregion BaseEmetter

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if (this.isRandomAngle)
                return;

            for (var i = 0; i < this.amount; i++)
            {
                var angle = this.GetAngle(i);

                var targetDirection = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);

                Gizmos.DrawLine(this.transform.position, this.transform.position + targetDirection);
            }
        }

        #endregion Gizmos
    }
}