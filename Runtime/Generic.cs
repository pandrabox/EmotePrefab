using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using com.github.pandrabox.emoteprefab.runtime;
using VRC.SDK3.Avatars.Components;


namespace com.github.pandrabox.emoteprefab.runtime
{
    public static class Generic
    {
        /// <summary>
        /// Searches for a specific component in the self or parent direction.
        /// Example of use: var Descriptor = FindComponentFromParent<VRCAvatarDescriptor>(MyGameObject);
        /// </summary>
        /// <typeparam name="T">Target Component Name</typeparam>
        /// <param name="CurrentObject">GameObject to search from</param>
        /// <returns>The first component found, or null if none.</returns>
        public static T FindComponentFromParent<T>(GameObject CurrentObject) where T : Component
        {
            while (CurrentObject != null)
            {
                var component = CurrentObject.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
                CurrentObject = CurrentObject.transform.parent?.gameObject;
            }
            return null;
        }
        /// <summary>
        /// Searches for a specific component in the self or parent direction.
        /// Example of use: var Descriptor = FindComponentFromParent<VRCAvatarDescriptor>(MyTransform);
        /// </summary>
        /// <typeparam name="T">Target Component Name</typeparam>
        /// <param name="CurrentTransform">Transform to search from</param>
        /// <returns>The first component found, or null if none.</returns>
        public static T FindComponentFromParent<T>(Transform CurrentTransform) where T : Component
        {
            return FindComponentFromParent<T>(CurrentTransform?.gameObject);
        }
        public static GameObject GetAvatarRootObject(GameObject Target)
        {
            return FindComponentFromParent<VRCAvatarDescriptor>(Target)?.gameObject;
        }
        public static GameObject GetAvatarRootObject(Transform Target)
        {
            return FindComponentFromParent<VRCAvatarDescriptor>(Target)?.gameObject;
        }
        public static Transform GetAvatarRootTransform(GameObject Target)
        {
            return FindComponentFromParent<VRCAvatarDescriptor>(Target)?.gameObject?.transform;
        }
        public static Transform GetAvatarRootTransform(Transform Target)
        {
            return FindComponentFromParent<VRCAvatarDescriptor>(Target)?.gameObject?.transform;
        }
        public static bool IsInAvatar(GameObject Target)
        {
            return GetAvatarRootObject(Target) != null;
        }
        public static bool IsInAvatar(Transform Target)
        {
            return IsInAvatar(Target.gameObject);
        }

        public static GUIStyle TitleStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.normal.background = MakeTex(1, 1, new Color(255f / 255f, 128f / 255f, 0f / 255f, 1f));
            style.normal.textColor = Color.black;
            style.fontStyle = FontStyle.Bold;
            return style;
        }
        public static Texture2D MakeTex(int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = color;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
        public static bool IsWithinErrorRange(Vector3 vector3, float referenceValue, float errorThreshold)
        {
            return Mathf.Abs(vector3.x - referenceValue) <= errorThreshold &&
                   Mathf.Abs(vector3.y - referenceValue) <= errorThreshold &&
                   Mathf.Abs(vector3.z - referenceValue) <= errorThreshold;
        }

        public static void SetEditorOnly(string TargetName, bool SW, GameObject ParentObject = null)
        {
            var Targets = GetGameObjectsByName(TargetName, ParentObject);
            foreach (var Target in Targets)
            {
                SetEditorOnly(Target, SW);
            }
        }
        public static void SetEditorOnly(GameObject Target, bool SW)
        {
            if (SW)
            {
                Target.tag = "EditorOnly";
                Target.SetActive(false);
            }
            else
            {
                Target.tag = "Untagged";
                Target.SetActive(true);
            }
        }
        public static void SetEditorOnly(Transform Target, bool SW)
        {
            SetEditorOnly(Target.gameObject, SW);
        }


        public static Transform[] GetTransformsByName(string TargetName, Transform ParentTransform = null)
        {
            Transform[] Transforms;
            if (ParentTransform != null)
            {
                Transforms = ParentTransform.GetComponentsInChildren<Transform>(true)?.Where(t => t.name == TargetName)?.ToArray();
            }
            else
            {
                Transforms = GameObject.FindObjectsOfType<Transform>()?.Where(t => t.name == TargetName)?.ToArray();
            }
            return Transforms;
        }
        public static Transform[] GetTransformsByName(string TargetName, GameObject ParentObject = null)
        {
            return GetTransformsByName(TargetName, ParentObject.transform);
        }
        public static GameObject[] GetGameObjectsByName(string TargetName, Transform ParentTransform = null)
        {
            Transform[] Transforms = GetTransformsByName(TargetName, ParentTransform);
            GameObject[] GameObjects = new GameObject[Transforms.Length];
            for (int i = 0; i < Transforms.Length; i++)
            {
                GameObjects[i] = Transforms[i].gameObject;
            }
            return GameObjects;
        }
        public static GameObject[] GetGameObjectsByName(string TargetName, GameObject ParentGameObject = null)
        {
            return GetGameObjectsByName(TargetName, ParentGameObject.transform);
        }




        public static bool IsTargetEditorOnly(string TargetName, GameObject ParentObject = null)
        {
            Transform[] Transforms = GetTransformsByName(TargetName, ParentObject);
            if (Transforms == null || Transforms.Length < 1) { return false; }
            return IsTargetEditorOnly(Transforms[0].gameObject);
        }
        public static bool IsTargetEditorOnly(GameObject target)
        {
            return target.tag == "EditorOnly" && target.activeSelf == false;
        }

        public static float DELTA = 0.00001f;

        public static string[] GestureNames = new string[] { "Neutral", "Fist", "HandOpen", "FingerPoint", "Victory", "RocknRoll", "HandGun", "Thumbsup" };
        public enum Gesture
        {
            Neutral,
            Fist,
            HandOpen,
            FingerPoint,
            Victory,
            RocknRoll,
            HandGun,
            Thumbsup
        }
        public const int GESTURENUM = 8;
    }
}
