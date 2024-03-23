using Interfaces;

namespace Extensions
{
    public static class IPredicatableExtensions
    {
        public static void Add(this IPredicatable source, System.Func<float, bool> condition) 
            => Singletons.ActionPredicate.Instance.Add(source, condition);

        public static void Remove(this IPredicatable source, int limit = -1)
            => Singletons.ActionPredicate.Instance.Remove(source, limit);
    }
}
