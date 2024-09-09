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

// 次の2レイヤを持つFX(CurrentFX)を定義し、レイヤが一番下になるようにMAMergeAnimatorを設定する
// ・下がBlendShapeを含む場合、BodyのBlendShapeを全て0
// ・Emoteの非Humanoid(正確には非AAP)部分のカーブを再生するFXレイヤ

namespace com.github.pandrabox.emoteprefab.editor
{
    public class NonAAPPart
    {
        VRCAvatarDescriptor AvatarDescriptor;
        EmotePrefab[] SortedEPs;
        SplittedAnimation CurrentSplittedAnimation;
        AnimatorControllerLayer BlendShape0, NonAAPAnim;
        AnimatorController NonAAPFX;
        AnimatorStateMachine CurrentStateMachine;
        AnimatorState CurrentState;
        int CurrentID;
        EmotePrefab CurrentEmotePrefab;
        public NonAAPPart(VRCAvatarDescriptor AvatarDescriptor, EmotePrefab[] SortedEPs)
        {
            this.AvatarDescriptor = AvatarDescriptor;
            this.SortedEPs = SortedEPs;
            string WorkNonAAPFXPath = $@"{CONST.WORKDIR}NonAAPPart.controller";
            AssetDatabase.CopyAsset(CONST.OriginalNonAAPFXPath, WorkNonAAPFXPath);
            NonAAPFX = AssetDatabase.LoadAssetAtPath<AnimatorController>(WorkNonAAPFXPath);
            BlendShape0 = NonAAPFX.layers.FirstOrDefault(layer => layer.name == "EmotePrefab/BodyBlendShape0");
            NonAAPAnim = NonAAPFX.layers.FirstOrDefault(layer => layer.name == "EmotePrefab/NonAAPPart");
        }
        public void Run()
        {
            AddBlendShape0State();
            for (int i = 0; i < SortedEPs.Length; i++)
            {
                CurrentID = i + 1;
                CurrentEmotePrefab = SortedEPs[i];
                CurrentSplittedAnimation = new SplittedAnimation((AnimationClip)CurrentEmotePrefab.Motion);
                AddFX();
            }
            SetMergeAnimator();
        }
        private void AddFX()
        {
            AddBlendShape0(CurrentSplittedAnimation.IsBlendShapeClip);
            bool IsOthers = CurrentSplittedAnimation.IsBlendShapeClip || CurrentSplittedAnimation.IsOtherClip;
            AddNonAAPAnim(IsOthers);
        }
        private void AddBlendShape0State()
        {
            CurrentStateMachine = GetEmoteStateMachine(BlendShape0);
            CurrentState = CurrentStateMachine.AddState($@"BlendShape0");
            CurrentState.motion = BodyBlendShape0Anim();
            CurrentState.writeDefaultValues = false;
        }
        private AnimatorStateMachine GetEmoteStateMachine(AnimatorControllerLayer Target)
        {
            return Target.stateMachine.stateMachines.FirstOrDefault(sm => sm.stateMachine.name == "Emote").stateMachine;
        }
        private AnimationClip BodyBlendShape0Anim()
        {
            SkinnedMeshRenderer BodyMesh = AvatarDescriptor.transform?.Find("Body")?.GetComponent<SkinnedMeshRenderer>();
            AnimationClip clip = new AnimationClip();
            clip.wrapMode = WrapMode.ClampForever;
            int blendShapeCount = BodyMesh.sharedMesh.blendShapeCount;
            for (int i = 0; i < blendShapeCount; i++)
            {
                var name = BodyMesh.sharedMesh.GetBlendShapeName(i);
                AnimationCurve curve = AnimationCurve.Constant(0, clip.length, 0);
                clip.SetCurve("", typeof(SkinnedMeshRenderer), $"blendShape.{name}", curve);
            }
            return clip;
        }
        private void AddBlendShape0(bool IsBlandShape)
        {
            CurrentStateMachine = GetEmoteStateMachine(BlendShape0);
            if (IsBlandShape)
            {
                CurrentState = GetState("BlendShape0");
                TransitionFromPrepare();
                TransitionToRecovery();
                TransitionToForceExit();
            }
            else
            {
                TransitionFromPrepareToForceExit();
            }
        }
        private void AddNonAAPAnim(bool IsOthers)
        {
            CurrentStateMachine = GetEmoteStateMachine(NonAAPAnim);
            if (IsOthers)
            {
                CurrentState = CurrentStateMachine.AddState($@"E{CurrentID:D3}");
                CurrentState.motion = CurrentSplittedAnimation.NotAAPClip;
                CurrentState.writeDefaultValues = false;
                TransitionFromPrepare();
                TransitionToRecovery();
                TransitionToForceExit();
            }
            else
            {
                TransitionFromPrepareToForceExit();
            }
        }
        private void SetMergeAnimator()
        {
            GameObject TargetObj = new GameObject("EmotePrefabNonAAPPart");
            TargetObj.transform.SetParent(AvatarDescriptor.transform);
            var MergeAnimator = TargetObj.AddComponent<ModularAvatarMergeAnimator>();
            MergeAnimator.animator = NonAAPFX;
            MergeAnimator.pathMode = MergeAnimatorPathMode.Absolute;
            MergeAnimator.matchAvatarWriteDefaults = true;
            MergeAnimator.layerPriority = 9999999;
        }
        public AnimatorState GetState(string name)
        {
            return CurrentStateMachine.states.FirstOrDefault(s => s.state.name == name).state;
        }
        private void TransitionFromPrepare()
        {
            AnimatorState FromState = GetState("Prepare standing");
            AnimatorStateTransition T = FromState.AddTransition(CurrentState);
            T.hasExitTime = false;
            T.exitTime = 0.75f;
            T.hasFixedDuration = true;
            T.duration = 0.25f;
            T.offset = 0;
            T.AddCondition(AnimatorConditionMode.Equals, CurrentID, "VRCEmote");
        }
        private void TransitionToRecovery()
        {
            if (CurrentEmotePrefab.IsOneShot)
            {
                TransitionToRecovery_OneShot();
            }
            else
            {
                TransitionToRecovery_LoopHold();
            }
        }
        private void TransitionToRecovery_OneShot()
        {
            AnimatorState ToState = GetState("Recovery standing");
            AnimatorStateTransition T = CurrentState.AddTransition(ToState);
            T.hasExitTime = true;
            T.exitTime = 0.75f;
            T.hasFixedDuration = true;
            T.duration = 0.25f;
            T.offset = 0;
        }
        private void TransitionToRecovery_LoopHold()
        {
            AnimatorState ToState = GetState("Recovery standing");
            AnimatorStateTransition T = CurrentState.AddTransition(ToState);
            T.hasExitTime = false;
            T.exitTime = 0.75f;
            T.hasFixedDuration = true;
            T.duration = 0.25f;
            T.offset = 0;
            T.AddCondition(AnimatorConditionMode.NotEqual, CurrentID, "VRCEmote");
        }
        private void TransitionToForceExit()
        {
            AnimatorState ToState = GetState("Force Exit");
            AnimatorStateTransition T = CurrentState.AddTransition(ToState);
            T.hasExitTime = false;
            T.exitTime = 0.75f;
            T.hasFixedDuration = true;
            T.duration = 0;
            T.offset = 0;
            T.AddCondition(AnimatorConditionMode.If, 0, "Seated");
        }
        private void TransitionFromPrepareToForceExit()
        {
            AnimatorState FromState = GetState("Prepare standing");
            AnimatorState ToState = GetState("Force Exit");
            AnimatorStateTransition T = FromState.AddTransition(ToState);
            T.hasExitTime = false;
            T.exitTime = 0;
            T.hasFixedDuration = true;
            T.duration = 0;
            T.offset = 0;
            T.AddCondition(AnimatorConditionMode.Equals, CurrentID, "VRCEmote");
        }
    }
}