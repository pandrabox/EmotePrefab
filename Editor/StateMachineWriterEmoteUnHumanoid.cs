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
    public class StateMachineWriterEmoteUnHumanoid : StateMachineWriterEmote0
    {
        public StateMachineWriterEmoteUnHumanoid() : base(FXController, "EmotePrefab/NonAAP", "Emote", "Prepare standing")
        {
        }

        protected override void CreateState()
        {
            CreateState(StateName("W", _nEmote), EmotePrefabs[_nEmote].UnitMotions[0].Clip.FakeWD);
        }

        protected override void CreateStates()
        {
            if (_clip.UnHumanoid == null) return;
            CreateState(StateName("E", _nEmote, _nChain), _clip.UnHumanoid);
        }

        protected override void CreateTransition()
        {
            if (_clip.UnHumanoid != null)
            {
                var currentState = GetState("E", _nEmote, _nChain);
                var wdState = GetState("W", _nEmote);
                var nextState = GetState("E", _nEmote, _nChain + 1) ?? wdState;
                StartTransition(_initialState, currentState, _trans.Start);
                OneshotTransition(currentState, nextState, _trans.AutoExit);
                ManualExitTransition(currentState, nextState, _trans.ManualExit);
                ForceExitTransition(currentState, wdState, _trans.Sit);
                if (_nChain==0) WDExitTransition(wdState, _exitState, TransitionInfo.HasQuick);
            }
            else
            {
                QuickExitTransition(_initialState, _exitState, _trans.Quick);
            }
        }
    }
}