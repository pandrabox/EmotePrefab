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
    public abstract class StateMachineWriterEmote0 : StateMachineWriter0
    { 
        protected int _nEmote;
        protected int _nChain;
        protected AnimationState _nextState;
        protected int _id => _nEmote + 1;

        protected StateMachineWriterEmote0(AnimatorController controller, string layerName, string stateMachineName, string initialStateName)
            : base(controller, layerName, stateMachineName, initialStateName)
        {
        }

        protected override void OnInstantiate()
        {
            for (_nEmote = 0; _nEmote < EmotePrefabs.Length; _nEmote++)
            {
                CreateState();
                for (_nChain = 0; _nChain < EmotePrefabs[_nEmote].UnitMotions.Count; _nChain++)
                {
                    RenewUnit();
                    CreateStates();
                }
                for (_nChain = 0; _nChain < EmotePrefabs[_nEmote].UnitMotions.Count; _nChain++)
                {
                    RenewUnit();
                    CreateTransition();
                }
            }
        }

        protected void RenewUnit()
        {
            _unit = EmotePrefabs[_nEmote].UnitMotions[_nChain];
            _clip = _unit.Clip;
            _trans = _unit.TransitionInfo;
        }
        protected void StartTransition(AnimatorState from, AnimatorState to, TransitionInfo trans)
        {
            if (_nChain==0) SetTransition(from, to, trans).AddCondition(AnimatorConditionMode.Equals, _id, "VRCEmote");
        }
        protected void OneshotTransition(AnimatorState from, AnimatorState to, TransitionInfo trans)
        {
            if (_unit.MotionType==MotionType.OneShot || _unit.MotionType == MotionType.UnstoppableOneshot) SetTransition(from, to, trans);
        }
        protected void ManualExitTransition(AnimatorState from, AnimatorState to, TransitionInfo trans)
        {
            if(_unit.MotionType != MotionType.UnstoppableOneshot) SetTransition(from, to, trans).AddCondition(AnimatorConditionMode.NotEqual, _id, "VRCEmote");
        }
        protected void ForceExitTransition(AnimatorState from, AnimatorState to, TransitionInfo trans)
        {
            SetTransition(from, to, trans).AddCondition(AnimatorConditionMode.If, 0, "Seated");
        }
        protected void QuickExitTransition(AnimatorState from, AnimatorState to, TransitionInfo trans)
        {
            SetTransition(from, to, trans).AddCondition(AnimatorConditionMode.IfNot, 1, "Dummy");
        }
        protected void WDExitTransition(AnimatorState from, AnimatorState to, TransitionInfo trans)
        {
            SetTransition(from, to, trans);
        }

        /// <summary>
        /// ChainEmote全体で1つのState (WD関連)の生成
        /// </summary>
        protected abstract void CreateState();
        /// <summary>
        /// 1Unitで1つのStateの生成
        /// </summary>
        protected abstract void CreateStates();  
        protected abstract void CreateTransition();
    }
}