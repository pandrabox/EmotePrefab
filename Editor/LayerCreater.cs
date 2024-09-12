// <copyright file="LayerCreater.cs"></copyright>

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

namespace com.github.pandrabox.emoteprefab.editor
{
    /// <summary>
    /// レイヤの中身を実装するクラス
    /// </summary>
    public class LayerCreater
    {
        /// <summary>
        /// レイヤの中身実装の初期化
        /// </summary>
        public LayerCreater()
        {
            var actionLayer = new ActionLayer();
            var bodyShapeBlockerLayer = new BodyShapeBlockerLayer();
            var unhumanoidLayer = new UnhumanoidLayer();
            EmoteManager.MoveFirst();
            while (EmoteManager.Enable)
            {
                actionLayer.AddEmote();
                bodyShapeBlockerLayer.AddEmote();
                unhumanoidLayer.AddEmote();
                EmoteManager.Next();
            }
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1128
#pragma warning disable SA1502

    public class ActionLayer : EmoteLayer
    {
        public ActionLayer() : base(LayerType.Action) { }

        public void AddEmote()
        {
            CreateState(EmoteManager.StateName, EmoteManager.HumanoidClip, true);
            Transition_PrepareToCurrent();
            Transition_CurrentToRegularExit("Recovery standing");
            Transition_CurrentToForceExit("Force Exit");
        }
    }

    public class BodyShapeBlockerLayer : EmoteLayer
    {
        public BodyShapeBlockerLayer() : base(LayerType.BodyShapeBlocker) { }

        public void AddEmote()
        {
            if (EmoteManager.HasBodyShape)
            {
                CreateState(EmoteManager.StateName, EmoteManager.BodyShapeBlockerClip, true);
                Transition_PrepareToCurrent();
                Transition_CurrentToRegularExit("Exit");
                Transition_CurrentToForceExit("Exit");
            }
            else
            {
                Transition_PrepareToExit();
            }
        }
    }

    public class UnhumanoidLayer : EmoteLayer
    {
        public UnhumanoidLayer() : base(LayerType.Unhumanoid) { }

        public void AddEmote()
        {
            if (EmoteManager.HasUnhumanoid)
            {
                CreateState(EmoteManager.WDStateName, EmoteManager.FakeWriteDefaultClip, false);
                CreateState(EmoteManager.StateName, EmoteManager.UnhumanoidClip, true);
                Transition_PrepareToCurrent();
                Transition_CurrentToRegularExit(EmoteManager.WDStateName);
                Transition_CurrentToForceExit(EmoteManager.WDStateName);
                Transition_WDtoExit();
            }
            else
            {
                Transition_PrepareToExit();
            }
        }
    }
}
