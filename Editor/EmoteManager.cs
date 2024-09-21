#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using VRC.SDK3.Dynamics.PhysBone.Components;
using VRC.SDK3.Avatars.Components;
using System.Linq;
using com.github.pandrabox.emoteprefab.runtime;
using UnityEditor.Animations;

namespace com.github.pandrabox.emoteprefab.editor
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Pan/EmoteManager")]
    public static class EmoteManager
    {
        public static EmotePrefab[] EmotePrefabs;
        public static VRCAvatarDescriptor Descriptor;
        public static GameObject EmotePrefabRootObject, ActionObject, FXObject, FXRelativeObject, SyncObject;
        public static AnimatorController ActionController, FXController, FXRelativeController;

        public static Transform EmotePrefabRootTransform => EmotePrefabRootObject.transform;
        public static bool HasAFK => EmotePrefabs.Where(e => e.IsAFK).Any();
        public static bool HasTask => HasAFK || EmotePrefabs.Where(e => e.IsEmote).Any();
        public static int AFKCount => EmotePrefabs.Where(e => e.IsAFK).Count();
    }
}
#endif