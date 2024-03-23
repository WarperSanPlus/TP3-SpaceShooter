using Interfaces;
using System;

namespace Predicates
{
    [Serializable]
    public class Predicate
    {
        private readonly IPredicatable source;
        private readonly Func<float, bool> condition;

        public Predicate(IPredicatable source) : this(source, null) { }

        public Predicate(IPredicatable source, Func<float, bool> condition)
        {
            this.source = source;
            this.condition = condition;
        }

        public void Trigger() => this.source?.Trigger();
        public bool IsComplete(float elapsed) => this.condition?.Invoke(elapsed) ?? true;
        public bool Exists() => this.source != null;
        public bool IsSource(IPredicatable source) => this.source == source;
    }
}
