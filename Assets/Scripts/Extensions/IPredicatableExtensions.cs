using Interfaces;
using System;

namespace Extensions
{
    public static class IPredicatableExtensions
    {
        /// <inheritdoc cref="Singletons.ActionPredicate.Add(IPredicatable, Func{float, bool})"/>
        public static Guid Add(this IPredicatable source, Func<float, bool> condition)
            => Singletons.ActionPredicate.Instance.Add(source, condition);

        public static void RemoveAll(this IPredicatable source, int limit = -1)
            => Singletons.ActionPredicate.Instance.Remove(source, limit);

        public static void Remove(this IPredicatable source, Guid guid)
            => Singletons.ActionPredicate.Instance.Remove(guid);
    }
}