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
    public class StateMachineWriterEmotePorkerFace : StateMachineWriterEmote0
    {
        AnimatorState _emoteState;
        public StateMachineWriterEmotePorkerFace() : base(ActionController, "EmotePrefab/PorkerFace", "Emote", "Normal")
        {

            int[] emotionLayers = new int[]{ 1,2 };
            foreach(int layer in emotionLayers)
            {
                var beh = _initialState.AddStateMachineBehaviour<VRCAnimatorLayerControl>();
                beh.playable = VRC.SDKBase.VRC_AnimatorLayerControl.BlendableLayer.FX;
                beh.layer = layer;
                beh.goalWeight = 1;
                beh.blendDuration = 0.1f;
            }
            foreach (int layer in emotionLayers)
            {
                var beh = _emoteState.AddStateMachineBehaviour<VRCAnimatorLayerControl>();
                beh.playable = VRC.SDKBase.VRC_AnimatorLayerControl.BlendableLayer.FX;
                beh.layer = layer;
                beh.goalWeight = 0;
                beh.blendDuration = 0.1f;
            }
        }

        protected override void CreateState()
        {
            _emoteState = GetState("Emote");
        }

        protected override void CreateStates() { }

        protected override void CreateTransition()
        {
            EditorCurveBinding[] curves = AnimationUtility.GetCurveBindings(_clip.Original);
            if (curves.Any(c => (c.path.ToLower() == "body" && c.propertyName.StartsWith("blendShape."))))
            {
                SetTransition(_initialState, _emoteState, TransitionInfo.Quick).AddCondition(AnimatorConditionMode.Equals, _id, "NBitVRCEmote");
            }
        }
    }
}