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
    public class PanActionLayer
    {
        VRCAvatarDescriptor AvatarDescriptor;
        AnimatorController RootController;
        AnimatorStateMachine EmoteStateMachine;
        AnimatorState CurrentState;
        EmotePrefab CurrentEmotePrefab;
        SplittedAnimation CurrentSplittedAnimation;
        int CurrentID;
        public PanActionLayer(VRCAvatarDescriptor AvatarDescriptor)
        {
            this.AvatarDescriptor = AvatarDescriptor;
            RootController = (AnimatorController)(AvatarDescriptor.baseAnimationLayers[3].animatorController);
            var TopLevelStateMachines = RootController.layers[0].stateMachine.stateMachines;
            EmoteStateMachine = TopLevelStateMachines.FirstOrDefault(sm => sm.stateMachine.name == "Emote").stateMachine;
        }
        public void AddEmote(int EmoteID, EmotePrefab EP)
        {
            this.CurrentID = EmoteID;
            this.CurrentEmotePrefab = EP;
            CurrentSplittedAnimation = new SplittedAnimation((AnimationClip)EP.Motion);
            if (EP.IsOneShot)
            {
                AddOneShotEmote();
            }
            else
            {
                AddLoopEmote();
            }
        }

        public AnimatorState GetEmoteState(string name)
        {
            return EmoteStateMachine.states.FirstOrDefault(s => s.state.name == name).state;
        }
        public void AddLoopEmote()
        {
            CurrentState = EmoteStateMachine.AddState($@"E{CurrentID:D3}");
            CurrentState.motion = CurrentSplittedAnimation.AAPClip;
            CurrentState.writeDefaultValues = false;
            TranditionFromPrepare();
            TranditionToRecovery_LoopHold();
            TranditionToForceExit();
        }
        public void AddOneShotEmote()
        {
            CurrentState = EmoteStateMachine.AddState($@"E{CurrentID:D3}");
            CurrentState.motion = CurrentSplittedAnimation.AAPClip;
            CurrentState.writeDefaultValues = false;
            TranditionFromPrepare();
            TranditionToRecovery_OneShot();
            TranditionToForceExit();
        }
        private void TranditionFromPrepare()
        {
            AnimatorState FromState = GetEmoteState("Prepare standing");
            AnimatorStateTransition T = FromState.AddTransition(CurrentState);
            T.hasExitTime = false;
            T.exitTime = 0.75f;
            T.hasFixedDuration = true;
            T.duration = 0.25f;
            T.offset = 0;
            T.AddCondition(AnimatorConditionMode.Equals, CurrentID, "VRCEmote");
        }
        private void TranditionToRecovery_OneShot()
        {
            AnimatorState ToState = GetEmoteState("Recovery standing");
            AnimatorStateTransition T = CurrentState.AddTransition(ToState);
            T.hasExitTime = true;
            T.exitTime = 0.75f;
            T.hasFixedDuration = true;
            T.duration = 0.25f;
            T.offset = 0;
        }
        private void TranditionToRecovery_LoopHold()
        {
            AnimatorState ToState = GetEmoteState("Recovery standing");
            AnimatorStateTransition T = CurrentState.AddTransition(ToState);
            T.hasExitTime = false;
            T.exitTime = 0.75f;
            T.hasFixedDuration = true;
            T.duration = 0.25f;
            T.offset = 0;
            T.AddCondition(AnimatorConditionMode.NotEqual, CurrentID, "VRCEmote");
        }
        private void TranditionToForceExit()
        {
            AnimatorState ToState = GetEmoteState("Force Exit");
            AnimatorStateTransition T = CurrentState.AddTransition(ToState);
            T.hasExitTime = false;
            T.exitTime = 0.75f;
            T.hasFixedDuration = true;
            T.duration = 0;
            T.offset = 0;
            T.AddCondition(AnimatorConditionMode.If, 0, "Seated");
        }
    }
}