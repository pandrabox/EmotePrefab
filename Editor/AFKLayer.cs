// <copyright file="AFKLayer.cs"></copyright>

using System;
using System.Collections.Generic;
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
using VRC.SDKBase;
using static com.github.pandrabox.emoteprefab.runtime.Generic;

namespace com.github.pandrabox.emoteprefab.editor
{
    /// <summary>
    /// AFKLayerを構築するクラス
    /// </summary>
    public class AFKLayer
    {
        private readonly AnimatorControllerLayer _afkLayer;
        private readonly AnimatorStateMachine _afkStateMachine;
        private readonly AnimationClip _dummy2FClip;
        private readonly TransitionInfo _afkTransitionInfo;
        private readonly int _currentIndex;
        private AnimatorState _randomizeState;

        /// <summary>
        /// AFKLayerの構築
        /// </summary>
        public AFKLayer()
        {
            _afkLayer = Avatar.ActionController.layers[Config.ActionAFKControllerIndex];
            _afkStateMachine = _afkLayer.stateMachine.stateMachines.FirstOrDefault(sm => sm.stateMachine.name == "AFK").stateMachine;
            _randomizeState = GetState("Randomize");
            ((VRC_AvatarParameterDriver)_randomizeState.behaviours[0]).parameters[0].valueMax = EmoteManager.AFKCount-1;
            _currentIndex = 0;
            _dummy2FClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(Config.Dummy2FClip);
            if(_dummy2FClip == null)
            {
                WriteWarning("AFKLayer", "Dummy2FClip Not Found");
            }
            _afkTransitionInfo = new TransitionInfo(false, 0, false, 0, 0);
            for (int i = 0; i < EmoteManager.Length; i++)
            {
                if (!EmoteManager.IsAFK(i)) continue;    
                CreateUnitAFKState(i);
                _currentIndex++;
            }
        }

        /// <summary>
        /// 単体AFKの生成
        /// </summary>
        private void CreateUnitAFKState(int eI)
        {
            AnimatorState currentState = _afkStateMachine.AddState(EmoteManager.AFKName(eI));
            currentState.motion = _dummy2FClip;
            SetTransition(_randomizeState, currentState)
                .AddCondition(AnimatorConditionMode.Equals, _currentIndex, "Pandrabox/EmotePrefab/AFKIndex");
            SetTransition(currentState, null)
                .AddCondition(AnimatorConditionMode.IfNot, 0, "AFK");
            currentState.behaviours = new StateMachineBehaviour[]
            {
                ScriptableObject.CreateInstance<VRCAvatarParameterDriver>()
            };
            ((VRC_AvatarParameterDriver)currentState.behaviours[0]).localOnly = true;
            ((VRC_AvatarParameterDriver)currentState.behaviours[0]).parameters.Add
                (
                    new VRC_AvatarParameterDriver.Parameter()
                    {
                        type = VRC_AvatarParameterDriver.ChangeType.Set,
                        name = "VRCEmote",
                        value = EmoteManager.ID(eI),
                    }
                );
        }

        /// <summary>
        /// ステートの取得
        /// </summary>
        /// <param name="name">取得するステート名 *** "Exit"の場合nullを返します ***</param>
        /// <returns>取得したステート</returns>
        private AnimatorState GetState(string name)
        {
            if (name == "Exit")
            {
                return null;
            }

            var state = _afkStateMachine.states.FirstOrDefault(s => s.state.name == name).state;
            if (state == null)
            {
                WriteWarning("AFKLayer.GetState", $@"state[{name}] not found");
            }

            return state;
        }

        /// <summary>
        /// Transitionの設定
        /// </summary>
        /// <param name="fromState">From</param>
        /// <param name="toState">To。*** nullのときExitStateへ移動 ***</param>
        /// <returns>設定したTransition</returns>
        private AnimatorStateTransition SetTransition(AnimatorState fromState, AnimatorState toState)
        {
            if (fromState == null)
            {
                WriteWarning("SetTransition", "fromStateがnullです。これは予期されていません。");
            }

            AnimatorStateTransition transition = toState == null ? fromState.AddExitTransition() : fromState.AddTransition(toState);
            transition.hasExitTime = _afkTransitionInfo.HasExitTime;
            transition.exitTime = _afkTransitionInfo.ExitTime;
            transition.hasFixedDuration = _afkTransitionInfo.HasFixedDuration;
            transition.duration = _afkTransitionInfo.Duration;
            return transition;
        }
    }
}
