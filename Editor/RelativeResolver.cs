using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using com.github.pandrabox.emoteprefab.editor;
using com.github.pandrabox.emoteprefab.runtime;
using nadena.dev.modular_avatar.core;
using nadena.dev.modular_avatar.core.editor;
using nadena.dev.ndmf;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;
using static com.github.pandrabox.emoteprefab.runtime.Generic;
using static com.github.pandrabox.emoteprefab.editor.EmoteManager;

namespace com.github.pandrabox.emoteprefab.editor
{
    public static class RelativeResolver
    {
        public enum ResolveMode
        {
            Absolute,
            AutoByRelative,
            AutoByAbsolute
        }
        public static AnimationClip RelativeResolve(Transform Root, AnimationClip Original, ResolveMode mode)
        {
            var original = UnityEngine.Object.Instantiate(Original);
            var clip = UnityEngine.Object.Instantiate(Original);
            if (mode == ResolveMode.Absolute)  return original;
            var RootPath = FindPathRecursive(Descriptor.transform, Root);

            // CleanUp
            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
            {
                AnimationUtility.SetEditorCurve(clip, binding, null);
            }

            // Create ResolvedAnim
            foreach (var binding in AnimationUtility.GetCurveBindings(original))
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(original, binding);
                var path = binding.path;
                if (Root.Find(path) && path.Length > 0)
                {
                    path = $@"{RootPath}/{path}";
                }
                clip.SetCurve(path, binding.type, binding.propertyName, curve);
            }

            return clip;
        }
    }
}
