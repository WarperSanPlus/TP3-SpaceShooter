using UnityEngine;

namespace Extensions
{
    public static class GameObjectExtensions
    {
        // Source: https://forum.unity.com/threads/help-with-layer-change-in-all-children.779147/#post-7087573
        // I just changed the names
        public static void SetLayerRecursive(this GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
            {
                child.gameObject.layer = layer;

                if (child.childCount != 0)
                    child.gameObject.SetLayerRecursive(layer);
            }
        }
    }
}
