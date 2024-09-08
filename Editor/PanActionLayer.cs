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


public class PanActionLayer
{
    VRCAvatarDescriptor AvatarDescriptor;
    AnimatorController RootController;
    AnimatorStateMachine EmoteStateMachine;
    AnimatorState CurrentState;
    public PanActionLayer(VRCAvatarDescriptor AvatarDescriptor)
    {
        this.AvatarDescriptor = AvatarDescriptor;
        RootController = (AnimatorController)(AvatarDescriptor.baseAnimationLayers[3].animatorController);
        var TopLevelStateMachines = RootController.layers[0].stateMachine.stateMachines;
        EmoteStateMachine = TopLevelStateMachines.FirstOrDefault(sm => sm.stateMachine.name == "Emote").stateMachine;
    }
    public AnimatorState GetEmoteState(string name)
    {
        return EmoteStateMachine.states.FirstOrDefault(s => s.state.name == name).state;
    }
    public void AddLoopEmote(int EmoteID, string MotionPath)
    {
        Motion CurrentMotion = AssetDatabase.LoadAssetAtPath<Motion>(MotionPath);
        CurrentState = EmoteStateMachine.AddState($@"E{EmoteID:D3}");
        CurrentState.motion = CurrentMotion;
        CurrentState.writeDefaultValues = false;
        TranditionFromPrepare(EmoteID);
        TranditionToRecovery_LoopHold(EmoteID);
        TranditionToForceExit();
    }
    public void AddOneShotEmote(int EmoteID, string MotionPath)
    {
        Motion CurrentMotion = AssetDatabase.LoadAssetAtPath<Motion>(MotionPath);
        CurrentState = EmoteStateMachine.AddState($@"E{EmoteID:D3}");
        CurrentState.motion = CurrentMotion;
        CurrentState.writeDefaultValues = false;
        TranditionFromPrepare(EmoteID);
        TranditionToRecovery_OneShot();
        TranditionToForceExit();
    }
    private void TranditionFromPrepare(int EmoteID)
    {
        AnimatorState FromState = GetEmoteState("Prepare standing");
        AnimatorStateTransition T = FromState.AddTransition(CurrentState);
        T.hasExitTime = false;
        T.exitTime = 0.75f;
        T.hasFixedDuration = true;
        T.duration = 0.25f;
        T.offset = 0;
        T.AddCondition(AnimatorConditionMode.Equals, EmoteID,"VRCEmote");
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
    private void TranditionToRecovery_LoopHold(int EmoteID)
    {
        AnimatorState ToState = GetEmoteState("Recovery standing");
        AnimatorStateTransition T = CurrentState.AddTransition(ToState);
        T.hasExitTime = false;
        T.exitTime = 0.75f;
        T.hasFixedDuration = true;
        T.duration = 0.25f;
        T.offset = 0;
        T.AddCondition(AnimatorConditionMode.NotEqual, EmoteID, "VRCEmote");
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