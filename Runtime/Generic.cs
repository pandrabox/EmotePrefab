// <copyright file="Generic.cs"></copyright>
#pragma warning disable SA1600 // Elements should be documented

using System;
using System.Collections.Generic;
using System.Linq;
using nadena.dev.ndmf;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace com.github.pandrabox.emoteprefab.runtime
{
    /// <summary>
    /// 汎用ツール
    /// </summary>
    public static class Generic
    {
#if UNITY_EDITOR
        public static void WriteWarning(string functionname, string msg)
        {
            Debug.LogWarning($@"[EmotePrefab][{functionname}][{msg}]");
        }

        public static T FindComponentFromParent<T>(GameObject currentObject)
            where T : Component
        {
            while (currentObject != null)
            {
                var component = currentObject.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }

                currentObject = currentObject.transform.parent?.gameObject;
            }

            return null;
        }
        public static string FindPathRecursive(Transform parent, Transform target)
        {
            if (parent == target)
            {
                return "";
            }

            // 子要素を再帰的に調べる
            foreach (Transform child in parent)
            {
                string childPath = FindPathRecursive(child, target);
                if (childPath != null)
                {
                    // 親の名前を返さず、子のパスのみを返す
                    return child.name + (string.IsNullOrEmpty(childPath) ? "" : "/" + childPath);
                }
            }
            return null;
        }
#endif
    }
}



/* For Reviwer
 * Please review this code for each element and identify areas for improvement, providing a final score out of 100.
 * Please review this code in Japanese.
 */