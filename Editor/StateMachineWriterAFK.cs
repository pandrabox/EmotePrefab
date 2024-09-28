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
using VRC.SDK3.Dynamics.PhysBone;
using static com.github.pandrabox.emoteprefab.runtime.Generic;
using static com.github.pandrabox.emoteprefab.editor.EmoteManager;
using VRC.SDKBase;

namespace com.github.pandrabox.emoteprefab.editor
{
    public class StateMachineWriterAFK : StateMachineWriter0
    {
        private readonly AnimationClip _dummy2FClip;
        private readonly int _currentIndex;
        private readonly AnimatorState _randomizeState;

        public StateMachineWriterAFK() : base(ActionController, "EmotePrefab/AFKController", "AFK", "Randomize")
        {
            _dummy2FClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(Config.Dummy2FClip);
            _currentIndex = 0;
            _randomizeState = GetState("Randomize");
            ((VRC_AvatarParameterDriver)_randomizeState.behaviours[0]).parameters[0].valueMax = AFKCount - 1;
            for (int i = 0; i < EmotePrefabs.Length; i++)
            {
                if (!EmotePrefabs[i].IsAFK) continue;
                CreateUnitAFKState(i);
                _currentIndex++;
            }
        }

        protected override void OnInstantiate() { }

        private void CreateUnitAFKState(int eI)
        {
            var currentState = CreateState(StateName("AFK", eI), DummyClip(18));
            SetTransition(currentState, currentState, TransitionInfo.HasQuick).AddCondition(AnimatorConditionMode.NotEqual, eI + 1, "VRCEmote");
            SetTransition(_randomizeState, currentState, TransitionInfo.Quick).AddCondition(AnimatorConditionMode.Equals, _currentIndex, "EmotePrefab/AFKIndex");
            SetTransition(currentState, _exitState, TransitionInfo.Quick).AddCondition(AnimatorConditionMode.IfNot, 0, "AFK");
            var apdParameter = new VRC_AvatarParameterDriver.Parameter()
            {
                type = VRC_AvatarParameterDriver.ChangeType.Set,
                name = "VRCEmote",
                value = eI + 1,
            };
            currentState.behaviours = new StateMachineBehaviour[] { ScriptableObject.CreateInstance<VRCAvatarParameterDriver>() };
            ((VRC_AvatarParameterDriver)currentState.behaviours[0]).localOnly = true;
            ((VRC_AvatarParameterDriver)currentState.behaviours[0]).parameters.Add(apdParameter);
        }
    }
}