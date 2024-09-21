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
    /// 各StateMachineWriterの基底クラス
    /// </summary>
    public abstract class StateMachineWriter0
    {
        protected AnimatorController _currentController;
        protected AnimatorControllerLayer _currentLayer;
        protected AnimatorStateMachine _currentStateMachine;
        protected AnimatorState _initialState;
        protected AnimatorState _exitState = null;
        protected UnitMotion _unit;
        protected UnitMotionClips _clip;
        protected UnitMotionTransitionInfo _trans;

        protected StateMachineWriter0(AnimatorController controller, string layerName, string stateMachineName, string initialStateName)
        {
            _currentController = controller;
            _currentLayer = _currentController.layers.FirstOrDefault(layer => layer.name == layerName);
            _currentStateMachine = _currentLayer.stateMachine.stateMachines.FirstOrDefault(s => s.stateMachine.name == stateMachineName).stateMachine;
            _initialState = GetState(initialStateName);
            OnInstantiate();
        }

        protected abstract void OnInstantiate();

        /// <summary>
        /// CurrentStateMachineにおけるステートの取得
        /// </summary>
        /// <param name="name">取得するステート名 *** "Exit"の場合nullを返します ***</param>
        /// <returns>取得したステート</returns>
        protected AnimatorState GetState(string name)
        {
            if (name == "Exit") return null;
            var state = _currentStateMachine.states.FirstOrDefault(s => s.state.name == name).state;
            return state;
        }

        /// <summary>
        /// CurrentStateMachineにステートを作成
        /// </summary>
        /// <param name="name">作成ステート名</param>
        /// <param name="clip">AnimationClip</param>
        /// <param name="setCurrent">CurrentStateにセットする場合true</param>
        /// <returns>作成したステート</returns>
        protected AnimatorState CreateState(string name, AnimationClip clip)
        {
            AnimatorState animatorState = _currentStateMachine.AddState(name);
            animatorState.motion = clip;
            animatorState.writeDefaultValues = false;
            return animatorState;
        }

        /// <summary>
        /// Transitionの設定
        /// </summary>
        /// <param name="fromState">From</param>
        /// <param name="toState">To。*** nullのときExitStateへ移動 ***</param>
        /// <param name="transitionInfo">遷移条件</param>
        /// <returns>設定したTransition</returns>
        protected AnimatorStateTransition SetTransition(AnimatorState fromState, AnimatorState toState, TransitionInfo transitionInfo)
        {
            if (fromState == null) WriteWarning("SetTransition", "fromStateがnullです。これは予期されていません。");
            AnimatorStateTransition transition = toState == null ? fromState.AddExitTransition() : fromState.AddTransition(toState);
            transition.hasExitTime = transitionInfo.HasExitTime;
            transition.exitTime = transitionInfo.ExitTime;
            transition.hasFixedDuration = transitionInfo.HasFixedDuration;
            transition.duration = transitionInfo.Duration;
            return transition;
        }



        protected int ID(int m)
        {
            return m + 1;
        }
        protected string StateName(string prefix, int index)
        {
            return $@"{prefix}{ID(index):D3}";
        }
        protected AnimatorState GetState(string prefix, int m)
        {
            string name = StateName(prefix, m);
            return GetState(name);
        }
        protected string StateName(string prefix, int m, int n)
        {
            return $@"{prefix}{(m + 1):D3}_{n}";
        }
        protected AnimatorState GetState(string prefix, int m, int n)
        {
            string name = StateName(prefix, m, n);
            return GetState(name);
        }
    }
}