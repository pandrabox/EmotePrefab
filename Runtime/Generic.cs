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
using UnityEngine.UIElements;
using VRC.SDK3.Avatars.Components;
using static UnityEngine.GraphicsBuffer;

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
        public static string FindPathRecursive(Transform root, Transform child)
        {
            if (root == child) return "";

            List<string> pathSegments = new List<string>();
            while (child != root && child != null)
            {
                pathSegments.Add(child.name);
                child = child.parent;
            }

            if (child == null && root != null) return null;

            pathSegments.Reverse();
            return String.Join("/", pathSegments);
        }
        public static AnimationClip DummyClip(int flame)
        {
            var clip = new AnimationClip();
            AnimationCurve curve = AnimationCurve.Constant(0, (float)flame/60f, 0);
            clip.SetCurve(string.Empty, typeof(Animator), $"pandrabox/dummy", curve);
            return clip;
        }
#endif
    }
}



/* For Reviwer
 * Please review this code for each element and identify areas for improvement, providing a final score out of 100.
 * Please review this code in Japanese.
 */