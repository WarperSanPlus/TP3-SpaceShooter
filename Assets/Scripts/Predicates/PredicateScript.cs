using Extensions;
using Interfaces;
using System;

namespace Predicates
{
    public abstract class PredicateScript : UnityEngine.MonoBehaviour
    {
        public abstract Func<float, bool> GetCondition();

        public void Add(IPredicatable source) => source.Add(this.GetCondition());
    }
}
