using Extensions;
using Interfaces;
using System;

namespace Predicates
{
    /// <summary>
    /// Class that represents a condition
    /// </summary>
    public abstract class Predicate : UnityEngine.MonoBehaviour
    {
        /// <returns>Should this predicate activate?</returns>
        public abstract bool GetCondition(float elapsed);

        /// <summary>
        /// Adds this predicate to <paramref name="source"/>
        /// </summary>
        public void Add(IPredicatable source) => source.Add(this.GetCondition);
    }
}