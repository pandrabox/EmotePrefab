// <copyright file="EmotePrefabPass.cs"></copyright>

#pragma warning disable SA1402 // File may only contain a single type
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

[assembly: ExportsPlugin(typeof(EmotePrefabPass))]

namespace com.github.pandrabox.emoteprefab.editor
{
    /// <summary>
    /// To call from NDMF
    /// </summary>
    public class EmotePrefabPass : Plugin<EmotePrefabPass>
    {
        /// <summary>
        /// NDMFオーバーライド
        /// </summary>
        protected override void Configure()
        {
            InPhase(BuildPhase.Transforming).BeforePlugin("nadena.dev.modular-avatar").Run("PanEmotePrefab", ctx =>
            {
                new EmotePrefabMain().Run(ctx.AvatarDescriptor);
            });
        }
    }

    /// <summary>
    /// 適当なアバターで実行する(デバッグ専用)
    /// </summary>
    public class EmotePrefabUnityMenu : MonoBehaviour
    {
        /// <summary>
        /// To call from Unity menu
        /// </summary>
        [MenuItem("PanDev/EmotePrefab")]
        public static void GenEmotePrefab()
        {
            var avatarDescriptor = FindObjectOfType<VRCAvatarDescriptor>();
            new EmotePrefabMain().Run(avatarDescriptor);
        }
    }

    /// <summary>
    /// 実際の処理
    /// </summary>
    public class EmotePrefabMain : MonoBehaviour
    {
        /// <summary>
        /// トリガ用
        /// </summary>
        /// <param name="avatarDescriptor">ターゲットアバターのDescriptor</param>
        public void Run(VRCAvatarDescriptor avatarDescriptor)
        {
            if (avatarDescriptor == null)
            {
                WriteWarning("EmotePrefabMain.Run", "AvatarDescriptor Not found");
                return;
            }

            if (!avatarDescriptor.transform.GetComponentsInChildren<EmotePrefab>(false)
                .Where(emote => emote.Motion != null)
                .Any())
            {
                WriteWarning("EmotePrefabMain.Run", "Nothing to do");
                return;
            }

            WorkSpace.Create();
            Avatar.clear();
            EmoteManager.clear();
            Avatar.Init(avatarDescriptor);
            EmoteManager.Init();
            new LayerCreater();
            new ExpressionCreater();
        }
    }
}

/* For Reviwer
 * Please review this code for each element and identify areas for improvement, providing a final score out of 100.
 * Please review this code in Japanese.
 */