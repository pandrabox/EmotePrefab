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
            if (CurrentSplittedAnimation.IsBlendShapeClip)
            {
                AddBlendShape0();
            }
            if (CurrentSplittedAnimation.IsBlendShapeClip || CurrentSplittedAnimation.IsOtherClip)
            {
                AddNonAAPAnim();
            }
        }
        private void AddBlendShape0()
        {
            CurrentStateMachine = BlendShape0.stateMachine;
        }
        private void AddNonAAPAnim()
        {
            CurrentStateMachine = NonAAPAnim.stateMachine;
            CurrentState = CurrentStateMachine.AddState($@"E{CurrentID:D3}");
            CurrentState.motion = CurrentSplittedAnimation.NotAAPClip;
            CurrentState.writeDefaultValues = false;
            TranditionFromPrepare();
            TranditionToExit();
        }
        private void SetMergeAnimator()
        {

        }
        private void TranditionFromPrepare()
        {

        }
        private void TranditionToExit()
        {

        }
    }
}