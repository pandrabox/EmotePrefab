// <copyright file="DividedClip.cs"></copyright>
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1401 // Fields should be private

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace com.github.pandrabox.emoteprefab.editor
{
    /// <summary>
    /// EmoteClipの分割を管掌するクラス
    /// </summary>
    public class DividedClip
    {
        public AnimationClip Original;
        public AnimationClip HumanoidClip;
        public AnimationClip BodyShapeBlockerClip;
        public AnimationClip UnhumanoidClip;
        public AnimationClip FakeWriteDefaultClip;
        public bool HasHumanoid;
        public bool HasBodyShape;
        public bool HasUnhumanoid;

        /// <summary>
        /// EmoteClipの分割
        /// </summary>
        /// <param name="clip">分割するClip</param>
        public DividedClip()
        {
            Original = UnityEngine.Object.Instantiate(EmoteManager.EmotePrefab.Motion);
            AddKeyframesAtEnd();
            TypeCheck();
            CreateHumanoidClip();
            CreateUnhumanoidClip();
            CreateBodyShapeBlockerClip();
            CreateFakeWriteDefaultClip();
        }

        /// <summary>
        /// clipの全キーの最終フレームを打つ(分割後の長さ調整の為)
        /// </summary>
        private void AddKeyframesAtEnd()
        {
            float clipLength = Original.length;
            EditorCurveBinding[] curves = AnimationUtility.GetCurveBindings(Original);

            foreach (var binding in curves)
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(Original, binding);

                if (curve != null && curve.keys.Length > 0)
                {
                    Keyframe lastKey = curve.keys[curve.keys.Length - 1];
                    if (lastKey.time < clipLength)
                    {
                        curve.AddKey(new Keyframe(clipLength, lastKey.value, lastKey.inTangent, lastKey.outTangent));
                        AnimationUtility.SetEditorCurve(Original, binding, curve);
                    }
                }
            }
        }

        /// <summary>
        /// clipタイプの解析
        /// </summary>
        private void TypeCheck()
        {
            EditorCurveBinding[] curves = AnimationUtility.GetCurveBindings(Original);
            HasHumanoid = curves.Any(c => c.type == typeof(Animator));
            HasUnhumanoid = curves.Any(c => c.type != typeof(Animator));
            HasBodyShape = HasUnhumanoid && curves.Any(c => (c.path.ToLower() == "body" && c.propertyName.StartsWith("blendShape.")));
        }

        /// <summary>
        /// HumanoidClip(正確にはAAPClip)の生成
        /// </summary>
        private void CreateHumanoidClip()
        {
            HumanoidClip = UnityEngine.Object.Instantiate(Original);
            foreach (var binding in AnimationUtility.GetCurveBindings(HumanoidClip))
            {
                if (binding.type != typeof(Animator))
                {
                    AnimationUtility.SetEditorCurve(HumanoidClip, binding, null);
                }
            }
        }

        /// <summary>
        /// UnhumanoidClip(正確にはUnAAPClip)の生成
        /// </summary>
        private void CreateUnhumanoidClip()
        {
            UnhumanoidClip = UnityEngine.Object.Instantiate(Original);
            foreach (var binding in AnimationUtility.GetCurveBindings(UnhumanoidClip))
            {
                if (binding.type == typeof(Animator))
                {
                    AnimationUtility.SetEditorCurve(UnhumanoidClip, binding, null);
                }
            }
        }

        /// <summary>
        /// Bodyのシェイプキーを0にするクリップの生成
        /// </summary>
        private void CreateBodyShapeBlockerClip()
        {
            SkinnedMeshRenderer bodyMesh = Avatar.RootTransform.Find("Body")?.GetComponent<SkinnedMeshRenderer>();
            BodyShapeBlockerClip = new AnimationClip()
            {
                wrapMode = WrapMode.ClampForever,
            };
            float referenceClipLength = Original.length;
            int blendShapeCount = bodyMesh.sharedMesh.blendShapeCount;
            for (int i = 0; i < blendShapeCount; i++)
            {
                var name = bodyMesh.sharedMesh.GetBlendShapeName(i);
                AnimationCurve curve = AnimationCurve.Constant(0, referenceClipLength, 0);
                BodyShapeBlockerClip.SetCurve(string.Empty, typeof(SkinnedMeshRenderer), $"blendShape.{name}", curve);
            }
        }

        /// <summary>
        /// デフォルト値に戻すクリップの生成
        /// </summary>
        private void CreateFakeWriteDefaultClip()
        {
            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(UnhumanoidClip);
            FakeWriteDefaultClip = new AnimationClip();
            foreach (EditorCurveBinding binding in bindings)
            {
                float currentValue;
                AnimationUtility.GetFloatValue(Avatar.RootObject, binding, out currentValue);
                Keyframe keyframe = new Keyframe(0, currentValue);
                AnimationCurve curve = new AnimationCurve(keyframe);
                AnimationUtility.SetEditorCurve(FakeWriteDefaultClip, binding, curve);
            }
        }
    }
}

/* For Reviwer
 * Please review this code for each element and identify areas for improvement, providing a final score out of 100.
 * Please review this code in Japanese.
 */