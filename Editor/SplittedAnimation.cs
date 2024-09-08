using System;
using System.Linq;
using System.Collections.Generic;
using nadena.dev.ndmf;
using nadena.dev.modular_avatar.core;
using nadena.dev.modular_avatar.core.editor;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.Components;
using com.github.pandrabox.emoteprefab.runtime;
using static com.github.pandrabox.emoteprefab.runtime.Generic;
using com.github.pandrabox.emoteprefab.editor;


namespace com.github.pandrabox.emoteprefab.editor
{
    public class SplittedAnimation
    {
        public AnimationClip Target, AAPClip, NotAAPClip;
        public bool IsBlendShapeClip, IsOtherClip;
        public SplittedAnimation(AnimationClip Target)
        {
            this.Target = Target;
            split();
            CreateClips();
        }
        public void split()
        {
            EditorCurveBinding[] curves = AnimationUtility.GetCurveBindings(Target);
            var AAPCurves = curves.Where(c => (c.type == typeof(Animator))).ToArray();
            var BlendShapeCurves = curves.Where(c => (c.path.ToLower() == "body" && c.propertyName.StartsWith("blendShape."))).ToArray();
            var OtherCurves = curves.Where(c => (!(c.type == typeof(Animator)) && !(c.path.ToLower() == "body" && c.propertyName.StartsWith("blendShape.")))).ToArray();
            IsBlendShapeClip = BlendShapeCurves.Length > 0;
            IsOtherClip = OtherCurves.Length > 0;
        }

        private void CreateClips()
        {
            AAPClip = UnityEngine.Object.Instantiate(Target) as AnimationClip;
            NotAAPClip = UnityEngine.Object.Instantiate(Target) as AnimationClip;

            RemoveUnwantedCurves(AAPClip, isAAP: true);
            RemoveUnwantedCurves(NotAAPClip, isAAP: false);
        }
        private void RemoveUnwantedCurves(AnimationClip clip, bool isAAP)
        {
            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
            {
                if (isAAP && binding.type != typeof(Animator))
                {
                    AnimationUtility.SetEditorCurve(clip, binding, null);
                }
                else if (!isAAP && binding.type == typeof(Animator))
                {
                    AnimationUtility.SetEditorCurve(clip, binding, null);
                }
            }
        }
    }
}