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
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;

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
        private UnitMotion _unit;
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
                    _unit = _emote.UnitMotions[n];
                    _clip = _unit.Clip;
                    _original = UnityEngine.Object.Instantiate(_clip.Original);
                    SetLoop(_original, _unit.MotionType == MotionType.Loop);
                    AddOnEmoteObj();
                    CreateHumanoidClip();
                    CreateUnhumanoidClip();
                    CreateBodyShapeBlockerClip();
                }
                var clip0 = _emote.UnitMotions[0].Clip;
                CreateShrinkPhysBonesClip();
                CreateShrinkPhysBonesWriteDefaultClip();
                clip0.FakeWD = CreateFakeWriteDefaultClip(false);
                clip0.FakeWDR = CreateFakeWriteDefaultClip(true);
            }
        }

        private void AddOnEmoteObj()
        {
            foreach (var obj in _emote.OnEmoteObject.GameObjects)
            {
                if (obj == null) continue;
                var path = FindPathRecursive(_emote.transform, obj.transform) ?? FindPathRecursive(Descriptor.transform, obj.transform);
                AnimationCurve curve = AnimationCurve.Constant(0, _original.length, 1);
                _original.SetCurve(path, typeof(GameObject), "m_IsActive", curve);
                obj.SetActive(false);
            }
        }

        /// <summary>
        /// 1モーションの長さを合わせるためのキーを生成する
        /// </summary>
        /// <param name="target"></param>
        private void AddLengthHolder(AnimationClip target)
        {
            float length = _original.length > 4/60 ? _original.length : 4 / 60; // PBの切り替えに最小4フレーム必要
            AnimationCurve curve = AnimationCurve.Constant(0, length, 0);
            target.SetCurve(string.Empty, typeof(Animator), $"EmotePrefab/dummy", curve);
        }

        /// <summary>
        /// 1チェインの長さを合わせるためのキーを生成する
        /// </summary>
        private void AddLengthHolderForChain(AnimationClip target)
        {
            float length = 0;
            for (int n = 0; n < _emote.UnitMotions.Count; n++)
            {
                length += _emote.UnitMotions[n].Clip.Humanoid.length;
            }
            AnimationCurve curve = AnimationCurve.Constant(0, length, 0);
            target.SetCurve(string.Empty, typeof(Animator), $"EmotePrefab/dummy", curve);
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
            CreateHumanoidBlendTree();
        }

        private void CreateHumanoidBlendTree()
        {
            _clip.HumanoidH = CreateOtherHeightClip(_clip.Humanoid, 3);
            _clip.HumanoidL = CreateOtherHeightClip(_clip.Humanoid, -3);
            _clip.HumanoidBlendTree = new BlendTree()
            {
                blendType = BlendTreeType.Simple1D,
                blendParameter ="EmotePrefab/Height",
            };
            _clip.HumanoidBlendTree.AddChild(_clip.HumanoidL, 0f);
            _clip.HumanoidBlendTree.AddChild(_clip.HumanoidH, 1f); 
        }

        private AnimationClip CreateOtherHeightClip(AnimationClip original, float delta) { 
            var clip  = UnityEngine.Object.Instantiate(original);
            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
            {
                if (binding.propertyName == "RootT.y")
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                    for (int i = 0; i < curve.keys.Length; i++)
                    {
                        Keyframe key = curve.keys[i];
                        key.value += delta;
                        curve.MoveKey(i, key);
                    }
                    AnimationUtility.SetEditorCurve(clip, binding, curve);
                }
            }
            return clip;
        }

        /// <summary>
        /// UnhumanoidClip(正確にはUnAAPClip)の生成
        /// </summary>
        private void CreateUnhumanoidClip()
        {
            _clip.UnHumanoid = UnityEngine.Object.Instantiate(_original);
            _clip.UnHumanoidR = UnityEngine.Object.Instantiate(_original);
            foreach (var binding in AnimationUtility.GetCurveBindings(_clip.UnHumanoid))
            {
                // Delete All(Relative)
                AnimationUtility.SetEditorCurve(_clip.UnHumanoidR, binding, null);

                // Delete HumanPart(Abs)
                if (binding.type == typeof(Animator))
                {
                    AnimationUtility.SetEditorCurve(_clip.UnHumanoid, binding, null);
                    continue;
                }

                // Divide Absolute/Relative
                AnimationCurve curve = AnimationUtility.GetEditorCurve(_clip.UnHumanoid, binding);
                var RootPath = FindPathRecursive(Descriptor.transform, _emote.transform);
                var path = binding.path;
                if (_emote.transform.Find(path) && path.Length > 0)
                {
                    // If Relative, Set RelativePart
                    path = $@"{RootPath}/{path}";
                    _clip.UnHumanoidR.SetCurve(path, binding.type, binding.propertyName, curve);
                    // Delete AbsPart
                    AnimationUtility.SetEditorCurve(_clip.UnHumanoid, binding, null);
                }
            }
            AddLengthHolder(_clip.UnHumanoid);
            AddLengthHolder(_clip.UnHumanoidR);
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
        private AnimationClip CreateFakeWriteDefaultClip(bool IsRelative=false)
        {
            var clip = new AnimationClip();
            for (int n = 0; n < _emote.UnitMotions.Count; n++)
            {
                AnimationClip fromClip = IsRelative ? _emote.UnitMotions[n].Clip.UnHumanoidR : _emote.UnitMotions[n].Clip.UnHumanoid;
                EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(fromClip);
                foreach (EditorCurveBinding binding in bindings)
                {
                    float currentValue;
                    AnimationUtility.GetFloatValue(Descriptor.gameObject, binding, out currentValue);
                    Keyframe keyframe1 = new Keyframe(0, currentValue);
                    Keyframe keyframe2 = new Keyframe(4f / 60f, currentValue); // あまり根拠はないが気持ち伸ばす
                    AnimationCurve curve = new AnimationCurve(keyframe1, keyframe2);
                    AnimationUtility.SetEditorCurve(clip, binding, curve);
                }
            }
            return clip;
        }

        /// <summary>
        /// ShrinkPhysBones関連のクリップを作成
        /// </summary>
        /// <param name="inverce">Shrinkするときはfalse, 戻すときはtrue</param>
        /// <returns></returns>
        private AnimationClip CreateShrinkPhysBonesClipGeneral(bool inverce)
        {
            var clip = new AnimationClip();
            var aShrinkBones = ShrinkBones();
            if (aShrinkBones.Length == 0) return clip;

            float setValue;
            var PhysBoneType = typeof(VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone);
            float exitFlame;
            float frameRate = 60;
            foreach (var PB in aShrinkBones)
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

        private VRCPhysBone[] ShrinkBones()
        {
            if (_emote.ShrinkPhysBone.All)
            {
                return Descriptor.transform.GetComponentsInChildren<VRCPhysBone>(true);
            }
            else
            {
                return _emote.ShrinkPhysBone.PhysBones?
                    .Where(bone => bone != null)
                    .Distinct()
                    .ToArray();
            }
        }

        private void CreateShrinkPhysBonesClip()
        {
            _emote.UnitMotions[0].Clip.ShrinkPB = CreateShrinkPhysBonesClipGeneral(false);
            AddLengthHolderForChain(_emote.UnitMotions[0].Clip.ShrinkPB);
        }

        private void CreateShrinkPhysBonesWriteDefaultClip()
        {
            _emote.UnitMotions[0].Clip.ShrinkWD = CreateShrinkPhysBonesClipGeneral(true);
        }

        private void SetLoop(AnimationClip clip, bool key)
        {
            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = key;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
        }
    }
}