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
    /// <summary>
    /// それぞれのStateMachineに適切な内容を書き込み
    /// </summary>
    public class StateMachineWriter
    {
        public StateMachineWriter()
        {
            new StateMachineWriterAFK();
            new StateMachineWriterEmoteAction();
            new StateMachineWriterEmotePorkerFace();
            new StateMachineWriterEmoteShrinkPB();
            new StateMachineWriterEmoteUnHumanoid();
            new StateMachineWriterEmoteUnHumanoidRelative();
        }
    }
}

