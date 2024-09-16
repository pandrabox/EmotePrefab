// <copyright file="DividedClip.cs"></copyright>
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1401 // Fields should be private

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static com.github.pandrabox.emoteprefab.runtime.Generic;

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
        public AnimationClip ShrinkPhysBonesClip;
        public AnimationClip ShrinkPhysBonesWriteDefaultClip;
        public bool HasHumanoid;
        public bool HasBodyShape;
        public bool HasUnhumanoid;

        /// <summary>
        /// EmoteClipの分割
        /// </summary>
        /// <param name="clip">分割するClip</param>
        public DividedClip(int eI)
        {
            Original = UnityEngine.Object.Instantiate(EmoteManager.EmotePrefab(eI).Motion);
            AddKeyframesAtEnd();
            TypeCheck();
            CreateHumanoidClip();
            CreateUnhumanoidClip();
            CreateBodyShapeBlockerClip();
            CreateFakeWriteDefaultClip();
            CreateShrinkPhysBonesClip(eI);
            CreateShrinkPhysBonesWriteDefaultClip(eI);
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

        /// <summary>
        /// ShrinkPhysBones関連のクリップを作成
        /// </summary>
        /// <param name="eI">Emote番号</param>
        /// <param name="inverce">Shrinkするときはfalse, 戻すときはtrue</param>
        /// <returns></returns>
        private AnimationClip CreateShrinkPhysBonesClipGeneral(int eI, bool inverce)
        {
            var clip = new AnimationClip();
            float setValue;
            var PhysBoneType = typeof(VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone);
            float exitFlame;
            float frameRate = 60;
            foreach (var PB in EmoteManager.ShrinkBones(eI))
            {
                var PBPath = FindPathRecursive(Avatar.RootTransform, PB.transform);
                var bindingPhysBoneRadius = new EditorCurveBinding() { path = PBPath, propertyName = "radius", type = PhysBoneType };
                if (inverce)
                {
                    AnimationUtility.GetFloatValue(Avatar.RootObject, bindingPhysBoneRadius, out setValue);
                    exitFlame = 4f / frameRate;
                }
                else 
                {
                    setValue = 0.0001f;
                    exitFlame = Original.length;
                }
                AnimationCurve curve = new AnimationCurve(new Keyframe(0, setValue), new Keyframe(exitFlame, setValue));
                AnimationUtility.SetEditorCurve(clip, bindingPhysBoneRadius, curve);

                var bindingPhysBoneActive = new EditorCurveBinding() { path = PBPath, propertyName = "m_Enabled", type = PhysBoneType };
                AnimationCurve curve2 = new AnimationCurve(new Keyframe(1f / frameRate, 1), new Keyframe(2f / frameRate, 0), new Keyframe(3f / frameRate, 0), new Keyframe(4f / frameRate, 1));
                AnimationUtility.SetEditorCurve(clip, bindingPhysBoneActive, curve2);
            }

            return clip;
        }

        private void CreateShrinkPhysBonesClip(int eI)
        {
            ShrinkPhysBonesClip = CreateShrinkPhysBonesClipGeneral(eI, false);
            //AssetDatabase.CreateAsset(ShrinkPhysBonesClip, $@"assets/ShrinkPhysBone{eI}ON.anim");
        }

        private void CreateShrinkPhysBonesWriteDefaultClip(int eI)
        {
            ShrinkPhysBonesWriteDefaultClip = CreateShrinkPhysBonesClipGeneral(eI, true);
            // AssetDatabase.CreateAsset(ShrinkPhysBonesWriteDefaultClip, $@"assets/ShrinkPhysBone{eI}OFF.anim");
        }
    }
}

/* For Reviwer
 * Please review this code for each element and identify areas for improvement, providing a final score out of 100.
 * Please review this code in Japanese.
 */