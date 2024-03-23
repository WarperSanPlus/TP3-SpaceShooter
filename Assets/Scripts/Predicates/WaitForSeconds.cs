using System;

namespace Predicates
{
    public class WaitForSeconds : PredicateScript
    {
        public float time = 0;

        public override Func<float, bool> GetCondition() => new(elapsed =>
        {
            this.time -= elapsed;

            return this.time <= 0;
        });
    }
}
