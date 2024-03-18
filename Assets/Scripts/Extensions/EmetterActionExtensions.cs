using Serializables;

namespace Extensions
{
    public static class EmetterActionExtensions
    {
        public static void CallActions(this EmetterAction[] actions)
        {
            foreach (EmetterAction item in actions)
                item.script.enabled = item.willBeActive;
        }
    }
}