// <copyright file="AnimatePhysBones.cs"></copyright>

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
using VRC.SDK3.Dynamics.PhysBone.Components;
using VRC.SDKBase;
using static com.github.pandrabox.emoteprefab.runtime.Generic;

namespace com.github.pandrabox.emoteprefab.editor
{
    /// <summary>
    /// PhysBoneのIsAnimatedを必要に応じtrueにする
    /// </summary>
    public class AnimatePhysBones
    {
        public AnimatePhysBones()
        {
            VRCPhysBone[] targetPhysBones;
            if (EmoteManager.HasAnimateAllPhysBones)
            {
                targetPhysBones = Avatar.RootTransform.GetComponentsInChildren<VRCPhysBone>(true);
            }
            else
            {
                targetPhysBones = EmoteManager.AnimatePhysBones;
            }
            foreach(var targetPhysBone in targetPhysBones)
            {
                targetPhysBone.isAnimated= true;
            }
        }
    }
}