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
    public class StateMachineWriterEmoteShrinkPB : StateMachineWriterEmote0
    {
        public StateMachineWriterEmoteShrinkPB() : base(FXController, "EmotePrefab/ShrinkPB", "Emote", "Prepare standing")
        {
        }

        protected override void CreateState()
        {
            CreateState(StateName("W", _nEmote), EmotePrefabs[_nEmote].UnitMotions[0].Clip.ShrinkWD);
        }

        protected override void CreateStates()
        {
            if (_clip.ShrinkPB == null) return;
            if (_nChain > 0) return;
            CreateState(StateName("E", _nEmote, _nChain), _clip.ShrinkPB);
        }

        protected override void CreateTransition()
        {
            if (_nChain > 0) return;
            if (_clip.ShrinkPB != null)
            {
                var currentState = GetState("E", _nEmote, _nChain);
                var wdState = GetState("W", _nEmote);
                var nextState = GetState("E", _nEmote, _nChain + 1) ?? wdState;
                StartTransition(_initialState, currentState, _trans.Quick);
                OneshotTransition(currentState, nextState, _trans.Quick);
                ManualExitTransition(currentState, nextState, _trans.Quick);
                ForceExitTransition(currentState, wdState, _trans.Quick);
                if(_nChain==0)  WDExitTransition(wdState,_exitState, _trans.Quick);
            }
            else
            {
                QuickExitTransition(_initialState, _exitState, _trans.Quick);
            }
        }
    }
}