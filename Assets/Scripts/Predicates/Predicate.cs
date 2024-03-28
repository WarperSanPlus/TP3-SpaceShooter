using Extensions;
using Interfaces;
using System;

namespace Predicates
{
    public abstract class Predicate : UnityEngine.MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elapsed"></param>
        /// <returns></returns>
        public abstract bool GetCondition(float elapsed);

        public void Add(IPredicatable source) => source.Add(this.GetCondition);
    }
}
