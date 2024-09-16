// <copyright file="EmoteLayer.cs"></copyright>

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
using static com.github.pandrabox.emoteprefab.runtime.Generic;

#pragma warning disable SA1401 // Fields should be private
#pragma warning disable SA1600 // Elements should be documented
namespace com.github.pandrabox.emoteprefab.editor
{
    /// <summary>
    /// Emoteに関するレイヤを扱う基底クラス
    /// </summary>
    public class EmoteLayer
    {
        protected AnimatorControllerLayer _currentLayer;
        protected AnimatorState _prepareState;
        protected AnimatorStateMachine _currentStateMachine;
        protected AnimatorState _currentState;
#pragma warning restore SA1600

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="avatarDescriptor">対象のVRCAvatarDescriptor</param>
        /// <param name="layerType">レイヤータイプ</param>
        protected EmoteLayer(LayerType layerType)
        {
            SetCurrentLayer(layerType);
            _prepareState = GetState("Prepare standing");
        }

        /// <summary>
        /// レイヤタイプ
        /// </summary>
        protected enum LayerType
        {
            /// <summary>
            /// EmoteのHumanoid部を再生するレイヤ
            /// </summary>
            Action,

            /// <summary>
            /// Emoteに表情が含まれる場合、FXの表情をブロックするレイヤ
            /// </summary>
            BodyShapeBlocker,

            /// <summary>
            /// EmoteのUnhumanoid部を再生するレイヤ
            /// </summary>
            Unhumanoid,

            /// <summary>
            /// PhysBoneのサイズを0にするレイヤ
            /// </summary>
            ShrinkPhysBones,
        }

        /// <summary>
        /// レイヤの設定
        /// </summary>
        /// <param name="layerType">対象レイヤ</param>
        protected void SetCurrentLayer(LayerType layerType)
        {
            AnimatorControllerLayer target;
            if (layerType == LayerType.Action)
            {
                target = Avatar.ActionController.layers[Config.ActionBaseIndex];
            }
            else if (layerType == LayerType.BodyShapeBlocker)
            {
                target = Avatar.FXController.layers[Config.FXBodyShapeBlockerIndex];
            }
            else if (layerType == LayerType.Unhumanoid)
            {
                target = Avatar.FXController.layers[Config.FXUnhumanoidIndex];
            }
            else
            {
                target = Avatar.FXController.layers[Config.ShrinkPhysBonesIndex];
            }

            _currentLayer = target ?? throw new System.IO.FileNotFoundException($@"Layer {Enum.GetName(typeof(LayerType), layerType)}({layerType}) Not Found");
            _currentStateMachine = target.stateMachine.stateMachines.FirstOrDefault(sm => sm.stateMachine.name == Config.EmoteStatemachineName).stateMachine;
        }

        /// <summary>
        /// _currentStateMachineにステートを作成
        /// </summary>
        /// <param name="name">作成ステート名</param>
        /// <param name="clip">AnimationClip</param>
        /// <param name="setCurrent">_currentStateにセットする場合true</param>
        /// <returns>作成したステート</returns>
        protected AnimatorState CreateState(string name, AnimationClip clip, bool setCurrent = false)
        {
            AnimatorState animatorState = _currentStateMachine.AddState(name);
            animatorState.motion = clip;
            animatorState.writeDefaultValues = false;
            if (setCurrent)
            {
                _currentState = animatorState;
            }

            return animatorState;
        }

        /// <summary>
        /// ステートの取得
        /// </summary>
        /// <param name="name">取得するステート名 *** "Exit"の場合nullを返します ***</param>
        /// <returns>取得したステート</returns>
        protected AnimatorState GetState(string name)
        {
            if (name == "Exit")
            {
                return null;
            }

            var state = _currentStateMachine.states.FirstOrDefault(s => s.state.name == name).state;
            if (state == null)
            {
                WriteWarning("GetState", $@"state[{name}] not found");
            }

            return state;
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
            if (fromState == null)
            {
                WriteWarning("SetTransition", "fromStateがnullです。これは予期されていません。");
            }

            AnimatorStateTransition transition = toState == null ? fromState.AddExitTransition() : fromState.AddTransition(toState);
            transition.hasExitTime = transitionInfo.HasExitTime;
            transition.exitTime = transitionInfo.ExitTime;
            transition.hasFixedDuration = transitionInfo.HasFixedDuration;
            transition.duration = transitionInfo.Duration;
            return transition;
        }

        /// <summary>
        /// PrepareからCurrentへの遷移設定
        /// </summary>
        protected void Transition_PrepareToCurrent(int eI, bool instant = false)
        {
            TransitionInfo T = instant ? new TransitionInfo(false, 0, false, 0, 0) : EmoteManager.StartTransitionInfo(eI);
            SetTransition(_prepareState, _currentState, T)
             .AddCondition(AnimatorConditionMode.Equals, EmoteManager.ID(eI), "VRCEmote");
        }

        /// <summary>
        /// PrepareからExitへの遷移設定
        /// </summary>
        protected void Transition_PrepareToExit(int eI)
        {
            SetTransition(_prepareState, null, EmoteManager.ForceExitTransitionInfo(eI))
             .AddCondition(AnimatorConditionMode.Equals, EmoteManager.ID(eI), "VRCEmote");
        }

        /// <summary>
        /// Currentが正常終了時の遷移設定
        /// </summary>
        /// <param name="toStateName">遷移先Stateの名称 ("Exit"の場合Unity標準Exit)</param>
        protected void Transition_CurrentToRegularExit(string toStateName, int eI, bool instant=false)
        {
            TransitionInfo T = instant ? new TransitionInfo(false, 0, false, 0, 0) : EmoteManager.RegularExitTransitionInfo(eI);
            var transition = SetTransition(_currentState, GetState(toStateName), T);
            if (!EmoteManager.IsOneShot(eI))
            {
                transition.AddCondition(AnimatorConditionMode.NotEqual, EmoteManager.ID(eI), "VRCEmote");
            }
        }

        /// <summary>
        /// Currentが異常終了時の遷移設定
        /// </summary>
        /// <param name="toStateName">遷移先Stateの名称 ("Exit"の場合Unity標準Exit)</param>
        protected void Transition_CurrentToForceExit(string toStateName, int eI, bool instant=false)
        {
            TransitionInfo T = instant ? new TransitionInfo(false, 0, false, 0, 0) : EmoteManager.ForceExitTransitionInfo(eI);
            SetTransition(_currentState, GetState(toStateName), T)
             .AddCondition(AnimatorConditionMode.If, 0, "Seated");
        }

        protected void Transition_OneshotCancel(string toStateName, int eI, bool instant = false)
        {
            TransitionInfo T = instant ? new TransitionInfo(false, 0, false, 0, 0) : EmoteManager.ForceExitTransitionInfo(eI);
            if (EmoteManager.IsOneShot(eI))
            {
                SetTransition(_currentState, GetState(toStateName), T)
                 .AddCondition(AnimatorConditionMode.NotEqual, EmoteManager.ID(eI), "VRCEmote");
            }
        }

        /// <summary>
        /// WDからExitへの遷移設定
        /// </summary>
        protected void Transition_WDtoExit(int eI, bool WaitAnimEnd=false)
        {
            if (WaitAnimEnd)
            {
                SetTransition(GetState(EmoteManager.WDStateName(eI)), null, new TransitionInfo(true, 1, false, 0));
            }
            else
            {
                SetTransition(GetState(EmoteManager.WDStateName(eI)), null, EmoteManager.ForceExitTransitionInfo(eI))
                 .AddCondition(AnimatorConditionMode.IfNot, 0, "Dummy");
            }
        }
    }
}


/* For Reviwer
 * Project policy : To set WriteDefault to OFF for all AnimatorStates.
 * Please review this code for each element and identify areas for improvement, providing a final score out of 100.
 * Please review this code in Japanese.
 */