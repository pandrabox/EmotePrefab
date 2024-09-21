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

[assembly: ExportsPlugin(typeof(ReplaceActionPlugin))]

namespace com.github.pandrabox.emoteprefab.editor
{
    public class ReplaceActionPlugin : Plugin<ReplaceActionPlugin>
    {
        protected override void Configure()
        {
            InPhase(BuildPhase.Transforming).AfterPlugin("nadena.dev.modular-avatar").Run("com.github.pandrabox.emoteprefab", ctx =>
            {
                var ReplaceActions = ctx.AvatarRootTransform.GetComponentsInChildren<ReplaceAction>(false).Where(c => c.ActionController != null).ToArray();
                if (ReplaceActions.Length == 0) return;
                if (ReplaceActions.Length > 1) WriteWarning("ReplaceAction", "ReplaceActionが複数見つかりました。1つであるべきです。最初に見つかったものを実行します。");
                ctx.AvatarDescriptor.baseAnimationLayers[3].animatorController = ReplaceActions.FirstOrDefault().ActionController;
                ctx.AvatarDescriptor.baseAnimationLayers[3].isDefault = false;
            });
        }
    }
}