using Extensions;
using Interfaces;
using System;

namespace Entities
{
    public class SubEntity : EnemyEntity, IPredicatable
    {
        // Link to parent
        // Destroy if parent dies
        public Guid Link(BaseEntity parent) => this.Add(t => !parent.enabled);

        #region IPredicatable

        /// <inheritdoc/>
        public void Trigger(Guid guid) => this.KillSelf(false);

        #endregion IPredicatable
    }
}