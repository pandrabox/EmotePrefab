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
        AnimatorControllerLayer Layer_BlendShape0, Layer_NonAAPAnim;
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
            Layer_BlendShape0 = NonAAPFX.layers.FirstOrDefault(layer => layer.name == "EmotePrefab/BodyBlendShape0");
            Layer_NonAAPAnim = NonAAPFX.layers.FirstOrDefault(layer => layer.name == "EmotePrefab/NonAAPPart");
        }
        public void Run()
        {
            for (int i = 0; i < SortedEPs.Length; i++)
            {
                CurrentID = i + 1;
                CurrentEmotePrefab = SortedEPs[i];
                CurrentSplittedAnimation = new SplittedAnimation(AvatarDescriptor.gameObject, (AnimationClip)CurrentEmotePrefab.Motion);
                LayerCreate_BlendShape0();
                LayerCreate_NonAAPAnim();
            }
            SetMergeAnimator();
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

        ///BS0--------------------------------
        private void LayerCreate_BlendShape0() //EmoteがBlendShapeを使っているならBlendShape0を上レイヤで実行
        {
            CurrentStateMachine = GetEmoteStateMachine(Layer_BlendShape0);
            if (CurrentSplittedAnimation.IsBlendShapeClip)
            {
                CurrentState = CurrentStateMachine.AddState($@"E{CurrentID:D3}");
                CurrentState.motion = BodyBlendShape0Anim(CurrentSplittedAnimation.AAPClip);
                CurrentState.writeDefaultValues = false;
                TransitionFromPrepare();
                TransitionToExit();
            }
            else
            {
                TransitionFromPrepareToForceExit();
            }
        }
        private AnimationClip BodyBlendShape0Anim(AnimationClip referenceClip) //BlendShape0アニメの生成
        {
            SkinnedMeshRenderer BodyMesh = AvatarDescriptor.transform?.Find("Body")?.GetComponent<SkinnedMeshRenderer>();
            AnimationClip clip = new AnimationClip();
            clip.wrapMode = WrapMode.ClampForever;
            float referenceClipLength = referenceClip.length; 
            int blendShapeCount = BodyMesh.sharedMesh.blendShapeCount;
            for (int i = 0; i < blendShapeCount; i++)
            {
                var name = BodyMesh.sharedMesh.GetBlendShapeName(i);
                AnimationCurve curve = AnimationCurve.Constant(0, referenceClipLength, 0); 
                clip.SetCurve("", typeof(SkinnedMeshRenderer), $"blendShape.{name}", curve);
            }
            return clip;
        }
        //Generic--------------------------------
        private AnimatorStateMachine GetEmoteStateMachine(AnimatorControllerLayer Target)
        {
            return Target.stateMachine.stateMachines.FirstOrDefault(sm => sm.stateMachine.name == "Emote").stateMachine;
        }
        public AnimatorState GetState(string name)
        {
            return CurrentStateMachine.states.FirstOrDefault(s => s.state.name == name).state;
        }
        private AnimatorStateTransition SetTransition(AnimatorState FromState, AnimatorState ToState, bool hasExitTime, float exitTime, bool hasFixedDuration, float duration, float offset)
        {
            AnimatorStateTransition transition = FromState.AddTransition(ToState);
            transition.hasExitTime = hasExitTime;
            transition.exitTime = exitTime;
            transition.hasFixedDuration = hasFixedDuration;
            transition.duration = duration;
            transition.offset = offset;
            return transition;
        }
        private AnimatorState CreateState(string name, AnimationClip clip)
        {
            AnimatorState animatorState = CurrentStateMachine.AddState(name);
            animatorState.motion = clip;
            animatorState.writeDefaultValues = false;
            return animatorState;
        }

        //NONAAPPartレイヤ関連--------------------------------
        private void LayerCreate_NonAAPAnim() //AAPでない部分の定義
        {
            bool IsOthers = CurrentSplittedAnimation.IsBlendShapeClip || CurrentSplittedAnimation.IsOtherClip;
            CurrentStateMachine = GetEmoteStateMachine(Layer_NonAAPAnim);
            if (IsOthers)
            {
                CurrentState = CurrentStateMachine.AddState($@"E{CurrentID:D3}");
                CurrentState.motion = CurrentSplittedAnimation.NotAAPClip;
                CurrentState.writeDefaultValues = false;
                TransitionFromPrepare();
                TransitionToExit();
            }
            else
            {
                TransitionFromPrepareToForceExit();
            }
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
        private void TransitionToExit()
        {
            var WDState = CreateState($@"WD{CurrentID:D3}", CurrentSplittedAnimation.DefaultValueClip);
            if (CurrentEmotePrefab.IsOneShot)
            {
                SetTransition(CurrentState, WDState, true, 0.75f, true, 0.25f, 0);
            }
            else
            {
                SetTransition(CurrentState, WDState, false, 0.75f, true, 0.25f, 0)
                    .AddCondition(AnimatorConditionMode.NotEqual, CurrentID, "VRCEmote");
            }
            SetTransition(CurrentState, WDState, false, 0.75f, true, 0.25f, 0)
                .AddCondition(AnimatorConditionMode.If, 0, "Seated");

            //WDStateで元の状態に戻してからEmoteを終了する
            var ExitTransition = WDState.AddExitTransition();
            ExitTransition.hasExitTime = false;
            ExitTransition.exitTime = 0f;
            ExitTransition.hasFixedDuration = false;
            ExitTransition.duration = 0f;
            ExitTransition.offset = 0;
            ExitTransition.AddCondition(AnimatorConditionMode.IfNot, 0, "Dummy");
        }
        private void TransitionFromPrepareToForceExit()
        {
            var ExitTransition = GetState("Prepare standing").AddExitTransition();
            ExitTransition.hasExitTime = false;
            ExitTransition.exitTime = 0f;
            ExitTransition.hasFixedDuration = false;
            ExitTransition.duration = 0f;
            ExitTransition.offset = 0;
            ExitTransition.AddCondition(AnimatorConditionMode.Equals, CurrentID, "VRCEmote");
        }
    }
}