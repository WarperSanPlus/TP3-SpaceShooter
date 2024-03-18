using System.Reflection;
using UnityEngine;

namespace Extensions
{
    public static class ComponentExtensions
    {
        // Taken from:https://discussions.unity.com/t/copy-a-component-at-runtime/71172/3
        public static void Copy<T>(this T destination, T original) where T : Component
        {
            foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
                field.SetValue(destination, field.GetValue(original));
        }
    }
}
