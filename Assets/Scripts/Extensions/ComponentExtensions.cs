using System.Reflection;
using UnityEngine;

namespace Extensions
{
    public static class ComponentExtensions
    {
        // Taken from:https://discussions.unity.com/t/copy-a-component-at-runtime/71172/3
        public static void Copy<T>(this T destination, T original) where T : Component
        {
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

            CopyFields(original, destination, fields);
        }

        public static Component CopyComponent(this Component original, GameObject destination)
        {
            Component[] m_List = destination.GetComponents<Component>();
            System.Type type = original.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

            foreach (Component comp in m_List)
            {
                // If we already have one of them
                if (original.GetType() == comp.GetType())
                {
                    foreach (FieldInfo field in fields)
                        field.SetValue(comp, field.GetValue(original));

                    return comp;
                }
            }

            // By here, we need to add it
            Component copy = destination.AddComponent(type);

            // Copied fields can be restricted with BindingFlags
            CopyFields(original, copy, fields);

            return copy;
        }

        private static void CopyFields(Component original, Component destination, FieldInfo[] fields)
        {
            foreach (FieldInfo field in fields)
            {
                if (field.GetCustomAttribute<IgnoreCopyAttribute>() != null)
                    continue;

                field.SetValue(destination, field.GetValue(original));
            }
        }
    }

    public class IgnoreCopyAttribute : System.Attribute
    {
        public IgnoreCopyAttribute() { }
    }
}
