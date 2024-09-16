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
            var shrinkPhysBoneLayer = new ShrinkPhysBoneLayer();
            for (int i = 0; i < EmoteManager.Length; i++)
            {
                actionLayer.AddEmote(i);
                bodyShapeBlockerLayer.AddEmote(i);
                unhumanoidLayer.AddEmote(i);
                shrinkPhysBoneLayer.AddEmote(i);
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

        public void AddEmote(int eI)
        {
            CreateState(EmoteManager.StateName(eI), EmoteManager.HumanoidClip(eI), true);
            Transition_PrepareToCurrent(eI);
            Transition_CurrentToRegularExit("Recovery standing",eI);
            Transition_OneshotCancel("Recovery standing", eI);
            Transition_CurrentToForceExit("Force Exit", eI);
        }
    }

    public class BodyShapeBlockerLayer : EmoteLayer
    {
        public BodyShapeBlockerLayer() : base(LayerType.BodyShapeBlocker) { }

        public void AddEmote(int eI)
        {
            if (EmoteManager.IsOneShot(eI))
            {
                CreateState(EmoteManager.StateName(eI), EmoteManager.BodyShapeBlockerClip(eI), true);
                Transition_PrepareToCurrent(eI);
                Transition_CurrentToRegularExit("Exit", eI);
                Transition_OneshotCancel("Exit", eI);
                Transition_CurrentToForceExit("Exit", eI);
            }
            else
            {
                Transition_PrepareToExit(eI);
            }
        }
    }

    public class UnhumanoidLayer : EmoteLayer
    {
        public UnhumanoidLayer() : base(LayerType.Unhumanoid) { }

        public void AddEmote(int eI)
        {
            if (EmoteManager.HasUnhumanoid(eI))
            {
                CreateState(EmoteManager.WDStateName(eI), EmoteManager.FakeWriteDefaultClip(eI), false);
                CreateState(EmoteManager.StateName(eI), EmoteManager.UnhumanoidClip(eI), true);
                Transition_PrepareToCurrent(eI);
                Transition_CurrentToRegularExit(EmoteManager.WDStateName(eI), eI);
                Transition_OneshotCancel(EmoteManager.WDStateName(eI), eI);
                Transition_CurrentToForceExit(EmoteManager.WDStateName(eI), eI);
                Transition_WDtoExit(eI);
            }
            else
            {
                Transition_PrepareToExit(eI);
            }
        }
    }
    public class ShrinkPhysBoneLayer : EmoteLayer
    {
        public ShrinkPhysBoneLayer() : base(LayerType.ShrinkPhysBones) { }

        public void AddEmote(int eI)
        {
            if (EmoteManager.HasShrinkPhysBones(eI))
            {
                CreateState(EmoteManager.WDStateName(eI), EmoteManager.ShrinkPhysBoneWriteDefaultClip(eI), false);
                CreateState(EmoteManager.StateName(eI), EmoteManager.ShrinkPhysBoneClip(eI), true);
                Transition_PrepareToCurrent(eI, true);
                Transition_CurrentToRegularExit(EmoteManager.WDStateName(eI), eI, true);
                Transition_OneshotCancel(EmoteManager.WDStateName(eI), eI, true);
                Transition_CurrentToForceExit(EmoteManager.WDStateName(eI), eI, true);
                Transition_WDtoExit(eI, true);
            }
            else
            {
                Transition_PrepareToExit(eI);
            }
        }
    }
}
