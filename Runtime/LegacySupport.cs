#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using com.github.pandrabox.emoteprefab.runtime;
using nadena.dev.ndmf;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;
using static com.github.pandrabox.emoteprefab.runtime.Generic;


namespace com.github.pandrabox.emoteprefab.runtime
{
    public class LegacySupport
    {
        public LegacySupport(VRCAvatarDescriptor descriptor)
        {
            AnimationClip[] clips = descriptor.baseAnimationLayers[3].animatorController?.animationClips;
            if (clips == null) return;
            HashSet<AnimationClip> uniqueClipsSet = new HashSet<AnimationClip>(clips);
            AnimationClip[] uniqueClips = new AnimationClip[uniqueClipsSet.Count];
            uniqueClipsSet.CopyTo(uniqueClips);
            var root = new GameObject(Config.LegacyObjName);
            var folder = root.AddComponent<EmoteFolder>();
            folder.FolderName = "Legacies";
            root.transform.SetParent(descriptor.transform);
            foreach(var cl in uniqueClips)
            {
                var obj = new GameObject();
                obj.transform.SetParent(root.transform);
                var ep = obj.AddComponent<EmotePrefab>();
                ep.SetEasy(cl);
            }
        }
    }
}

#endif

