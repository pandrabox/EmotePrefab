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
    /// <summary>
    /// EmoteClipの分割を管掌するクラス
    /// </summary>
    public class CreateClip
    {
        private AnimationClip _original;
        private UnitMotionClips _clip;
        private EmotePrefab _emote;
        /// <summary>
        /// EmoteClipの分割
        /// </summary>
        public CreateClip()
        {
            for (int m = 0; m < EmotePrefabs.Length; m++)
            {
                _emote = EmotePrefabs[m];
                for (int n = 0; n < _emote.UnitMotions.Count; n++)
                {
                    _clip = EmotePrefabs[m].UnitMotions[n].Clip;
                    _original = UnityEngine.Object.Instantiate(_clip.Original);
                    //AddKeyframesAtEnd();
                    CreateHumanoidClip();
                    CreateUnhumanoidClip();
                    CreateBodyShapeBlockerClip();
                    if (n == 0)
                    {
                        CreateShrinkPhysBonesClip();
                        CreateShrinkPhysBonesWriteDefaultClip();
                    }
                }
                CreateFakeWriteDefaultClip();
            }
        }

        private void AddLengthHolder(AnimationClip target)
        {
            float length = _original.length > 4 ? _original.length : 4; // PBの切り替えに最小4フレーム必要
            AnimationCurve curve = AnimationCurve.Constant(0, length, 0);
            target.SetCurve(string.Empty, typeof(Animator), $"pandrabox/dummy", curve);
        }

        /// <summary>
        /// HumanoidClip(正確にはAAPClip)の生成
        /// </summary>
        private void CreateHumanoidClip()
        {
            _clip.Humanoid = UnityEngine.Object.Instantiate(_original);
            foreach (var binding in AnimationUtility.GetCurveBindings(_clip.Humanoid))
            {
                if (binding.type != typeof(Animator))
                {
                    AnimationUtility.SetEditorCurve(_clip.Humanoid, binding, null);
                }
            }
            AddLengthHolder(_clip.Humanoid);
        }

        /// <summary>
        /// UnhumanoidClip(正確にはUnAAPClip)の生成
        /// </summary>
        private void CreateUnhumanoidClip()
        {
            _clip.UnHumanoid = UnityEngine.Object.Instantiate(_original);
            foreach (var binding in AnimationUtility.GetCurveBindings(_clip.UnHumanoid))
            {
                if (binding.type == typeof(Animator))
                {
                    AnimationUtility.SetEditorCurve(_clip.UnHumanoid, binding, null);
                }
            }
            AddLengthHolder(_clip.UnHumanoid);
        }

        /// <summary>
        /// Bodyのシェイプキーを0にするクリップの生成
        /// </summary>
        private void CreateBodyShapeBlockerClip()
        {
            EditorCurveBinding[] curves = AnimationUtility.GetCurveBindings(_clip.Original);
            if (!curves.Any(c => (c.path.ToLower() == "body" && c.propertyName.StartsWith("blendShape.")))) return;
            SkinnedMeshRenderer bodyMesh = Descriptor.transform.Find("Body")?.GetComponent<SkinnedMeshRenderer>();
            _clip.PokerFace = new AnimationClip()
            {
                wrapMode = WrapMode.ClampForever,
            };
            float referenceClipLength = _original.length;
            int blendShapeCount = bodyMesh.sharedMesh.blendShapeCount;
            for (int i = 0; i < blendShapeCount; i++)
            {
                var name = bodyMesh.sharedMesh.GetBlendShapeName(i);
                AnimationCurve curve = AnimationCurve.Constant(0, referenceClipLength, 0);
                _clip.PokerFace.SetCurve(string.Empty, typeof(SkinnedMeshRenderer), $"blendShape.{name}", curve);
            }
            AddLengthHolder(_clip.PokerFace);
        }

        /// <summary>
        /// デフォルト値に戻すクリップの生成
        /// </summary>
        private void CreateFakeWriteDefaultClip()
        {
            _emote.UnitMotions[0].Clip.FakeWD = new AnimationClip();
            for (int n = 0; n < _emote.UnitMotions.Count; n++)
            {
                EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(_emote.UnitMotions[n].Clip.UnHumanoid);
                foreach (EditorCurveBinding binding in bindings)
                {
                    float currentValue;
                    AnimationUtility.GetFloatValue(Descriptor.gameObject, binding, out currentValue);
                    Keyframe keyframe = new Keyframe(0, currentValue);
                    AnimationCurve curve = new AnimationCurve(keyframe);
                    AnimationUtility.SetEditorCurve(_emote.UnitMotions[0].Clip.FakeWD, binding, curve);
                }
            }
        }

        /// <summary>
        /// ShrinkPhysBones関連のクリップを作成
        /// </summary>
        /// <param name="inverce">Shrinkするときはfalse, 戻すときはtrue</param>
        /// <returns></returns>
        private AnimationClip CreateShrinkPhysBonesClipGeneral(bool inverce)
        {
            var clip = new AnimationClip();
            float setValue;
            var PhysBoneType = typeof(VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone);
            float exitFlame;
            float frameRate = 60;
            foreach (var PB in ShrinkBones())
            {
                var PBPath = FindPathRecursive(Descriptor.transform, PB.transform);
                var bindingPhysBoneRadius = new EditorCurveBinding() { path = PBPath, propertyName = "radius", type = PhysBoneType };
                if (inverce)
                {
                    AnimationUtility.GetFloatValue(Descriptor.gameObject, bindingPhysBoneRadius, out setValue);
                    exitFlame = 4f / frameRate;
                }
                else
                {
                    setValue = 0.0001f;
                    exitFlame = _original.length;
                }
                AnimationCurve curve = new AnimationCurve(new Keyframe(0, setValue), new Keyframe(exitFlame, setValue));
                AnimationUtility.SetEditorCurve(clip, bindingPhysBoneRadius, curve);

                var bindingPhysBoneActive = new EditorCurveBinding() { path = PBPath, propertyName = "m_Enabled", type = PhysBoneType };
                AnimationCurve curve2 = new AnimationCurve(new Keyframe(1f / frameRate, 1), new Keyframe(2f / frameRate, 0), new Keyframe(3f / frameRate, 0), new Keyframe(4f / frameRate, 1));
                AnimationUtility.SetEditorCurve(clip, bindingPhysBoneActive, curve2);
            }
            return clip;
        }

        static VRCPhysBone[] ShrinkBones()
        {
            if (EmotePrefabs.Any(e => e.ShrinkPhysBone.All))
            {
                return Descriptor.transform.GetComponentsInChildren<VRCPhysBone>(true);
            }
            else
            {
                return EmotePrefabs
                    .Where(prefab => prefab.ShrinkPhysBone.PhysBones != null)
                    .SelectMany(prefab => prefab.ShrinkPhysBone.PhysBones.Where(bone => bone != null))
                    .Distinct()
                    .ToArray();
            }
        }

        private void CreateShrinkPhysBonesClip()
        {
            _clip.ShrinkPB = CreateShrinkPhysBonesClipGeneral(false);
            AddLengthHolder(_clip.ShrinkPB);
        }

        private void CreateShrinkPhysBonesWriteDefaultClip()
        {
            _clip.ShrinkWD = CreateShrinkPhysBonesClipGeneral(true);
        }
    }
}