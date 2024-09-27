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
using static com.github.pandrabox.emoteprefab.runtime.Generic;
using Avatar = com.github.pandrabox.emoteprefab.runtime.TargetAvatar;
using static com.github.pandrabox.emoteprefab.editor.EmoteManager;

namespace com.github.pandrabox.emoteprefab.editor
{
    public class EmotePrefabProcessor
    {
        public EmotePrefabProcessor(VRCAvatarDescriptor descriptor)
        {
            new EmotePrefabInitializer(descriptor);
            if (!HasTask) return;
            new CreateClip();
             new DbgOutpAnim();
            new StateMachineWriter();
            new ExpressionCreater();
        }
    }
}

