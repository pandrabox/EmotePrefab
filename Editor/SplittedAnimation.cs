using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;

namespace com.github.pandrabox.emoteprefab.editor
{
    public class SplittedAnimation
    {
        public AnimationClip Target, AAPClip, NotAAPClip, DefaultValueClip;
        public bool IsBlendShapeClip, IsOtherClip;
        GameObject AnimRootObject;

        public SplittedAnimation(GameObject AnimRootObject, AnimationClip input)
        {
            this.AnimRootObject = AnimRootObject;
            Target = UnityEngine.Object.Instantiate(input) as AnimationClip;
            AddKeyframesAtEnd();
            split();
            CreateClips();
        }

        // 全てのカーブに最後のフレームでキーフレームを追加
        private void AddKeyframesAtEnd()
        {
            float clipLength = Target.length;
            EditorCurveBinding[] curves = AnimationUtility.GetCurveBindings(Target);

            foreach (var binding in curves)
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(Target, binding);

                if (curve != null && curve.keys.Length > 0)
                {
                    Keyframe lastKey = curve.keys[curve.keys.Length - 1];
                    if (lastKey.time < clipLength)
                    {
                        // 最後のフレームにキーフレームを追加
                        curve.AddKey(new Keyframe(clipLength, lastKey.value, lastKey.inTangent, lastKey.outTangent));
                        AnimationUtility.SetEditorCurve(Target, binding, curve);
                    }
                }
            }
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

            DefaultValueClip = CreateDefaultValueClip();
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


        // GameObject AnimRootObjectのAnimatorにNotAAPClipがアタッチされている
        // NotAAPClipに含まれる全てのカーブの値を現行の値にした1Fのアニメを作成する
        private AnimationClip CreateDefaultValueClip()
        {
            if (!IsOtherClip && !IsBlendShapeClip)
            {
                return null;
            }
            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(NotAAPClip);
            AnimationClip clip = new AnimationClip();
            foreach (EditorCurveBinding binding in bindings)
            {
                float currentValue;
                AnimationUtility.GetFloatValue(AnimRootObject, binding, out currentValue);
                Keyframe keyframe = new Keyframe(0, currentValue);
                AnimationCurve curve = new AnimationCurve(keyframe);
                AnimationUtility.SetEditorCurve(clip, binding, curve);
            }
            return clip;
        }
    }
}
