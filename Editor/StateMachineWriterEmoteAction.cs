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
    public class StateMachineWriterEmoteAction : StateMachineWriterEmote0
    {
        public StateMachineWriterEmoteAction() : base(ActionController, "Base Layer", "Emote", "Prepare standing")
        {
        }

        protected override void CreateState() { }

        protected override void CreateStates()
        {
            CreateState(StateName("E", _nEmote, _nChain), _clip.Humanoid);
        }

        protected override void CreateTransition()
        {
            var currentState = GetState("E", _nEmote, _nChain);
            var nextState = GetState("E", _nEmote, _nChain + 1) ?? GetState("Recovery standing");
            StartTransition(_initialState, currentState, _trans.Start);
            OneshotTransition(currentState, nextState, _trans.AutoExit);
            ManualExitTransition(currentState, nextState, _trans.ManualExit);
            ForceExitTransition(currentState, GetState("Force Exit"), _trans.Sit);
        }
    }
}