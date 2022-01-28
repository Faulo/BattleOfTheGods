using System.Linq;
using UnityEngine;

namespace Runtime.Extensions {
    public static class TransformExtensions {
        /// <summary>
        /// Delete all children
        /// </summary>
        /// <param name="parent"></param>
        public static void Clear(this Transform parent) {
            var children = Enumerable
                .Range(0, parent.childCount)
                .Select(parent.GetChild)
                .ToList();
            if (Application.isPlaying) {
                foreach (var child in children) {
                    Object.Destroy(child.gameObject);
                }
            } else {
                foreach (var child in children) {
                    Object.DestroyImmediate(parent.GetChild(0).gameObject);
                }
            }
        }
        public static bool TryGetComponentInParent<T>(this Transform context, out T target)
            where T : Component {
            for (var ancestor = context; ancestor; ancestor = ancestor.parent) {
                if (ancestor.TryGetComponent(out target)) {
                    return true;
                }
            }
            target = default;
            return false;
        }
        public static bool TryGetComponentInChildren<T>(this Transform context, out T target)
            where T : Component {
            if (context.TryGetComponent(out target)) {
                return true;
            }
            for (int i = 0; i < context.childCount; i++) {
                if (context.GetChild(i).TryGetComponentInChildren(out target)) {
                    return true;
                }
            }
            return false;
        }
    }
}