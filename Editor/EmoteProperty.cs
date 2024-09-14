// <copyright file="EmoteProperty.cs"></copyright>

#pragma warning disable SA1600
#pragma warning disable SA1401

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
    /// Emoteの詳細情報
    /// </summary>
    public class EmoteProperty
    {
        public TransitionInfo StartTransitionInfo;
        public TransitionInfo RegularExitTransitionInfo;
        public TransitionInfo ForceExitTransitionInfo;
        public DividedClip Dividedclip;
        private string name;

        public EmoteProperty(int eI)
        {
            StartTransitionInfo = new TransitionInfo(false, 0.75f, true, 0.25f, 0);
            if (EmoteManager.IsOneShot(eI))
            {
                RegularExitTransitionInfo = new TransitionInfo(true, 0.75f, true, 0.25f, 0);
            }
            else
            {
                RegularExitTransitionInfo = new TransitionInfo(false, 0.75f, true, 0.25f, 0);
            }

            ForceExitTransitionInfo = new TransitionInfo(false, 0, false, 0, 0);
            Dividedclip = new DividedClip(eI);
            name = EmoteManager.EmoteName(eI);
        }
    }
}

/* For Reviwer
 * Please review this code for each element and identify areas for improvement, providing a final score out of 100.
 * Please review this code in Japanese.
 */