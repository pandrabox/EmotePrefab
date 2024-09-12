// <copyright file="Avatar.cs"></copyright>
#pragma warning disable SA1600 // Elements should be documented

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
    /// Avatarを管理するクラス
    /// </summary>
    public static class Avatar
    {
        private static VRCAvatarDescriptor _descriptor;
        private static bool _initialized = false;
        private static AnimatorController _actionController;
        private static AnimatorController _fXController;

        public static AnimatorController ActionController
        {
            get
            {
                InitCheck();
                return _actionController;
            }
        }
        public static AnimatorController FXController
        {
            get
            {
                InitCheck();
                return _fXController;
            }
        }

        public static VRCAvatarDescriptor Descriptor
        {
            get
            {
                InitCheck();
                return _initialized ? _descriptor : null;
            }
        }

        public static GameObject RootObject
        {
            get
            {
                InitCheck();
                return _descriptor.gameObject;
            }
        }

        public static Transform RootTransform
        {
            get
            {
                InitCheck();
                return _descriptor.transform;
            }
        }

        public static VRCAvatarDescriptor.CustomAnimLayer ActionLayer
        {
            get
            {
                InitCheck();
                return _descriptor.baseAnimationLayers[3];
            }
        }

        public static void clear()
        {
            _initialized = false;
        }

        public static void Init(VRCAvatarDescriptor descriptor)
        {
            if (!_initialized)
            {
                _initialized = true;
                _descriptor = descriptor;
                ActionReplace();
                MergeGeneratedFX();
                EmoteLayersSync();
            }
            else
            {
                WriteWarning("Avatar.Init", "Initial twice time");
            }
        }

        private static void InitCheck()
        {
            if (!_initialized)
            {
                WriteWarning("Avatar", "Avatar class was accessed before it was initialized");
            }
        }

        /// <summary>
        /// Actionレイヤの置き換え
        /// </summary>
        private static void ActionReplace()
        {
            _actionController = AssetDatabase.LoadAssetAtPath<AnimatorController>(Config.GeneratedActionLayer);
            _descriptor.baseAnimationLayers[3].animatorController = _actionController ?? throw new Exception("EmotePrefab ActionLayerReplace AssignController Not Found");
            _descriptor.baseAnimationLayers[3].isDefault = false;
        }

        /// <summary>
        /// GeneratedFXをMergeするプレハブ生成
        /// </summary>
        private static void MergeGeneratedFX()
        {
            GameObject targetObj = new GameObject("EmotePrefab_FX");
            targetObj.transform.SetParent(Avatar.RootTransform);
            var mergeAnimator = targetObj.AddComponent<ModularAvatarMergeAnimator>();
            _fXController = AssetDatabase.LoadAssetAtPath<AnimatorController>(Config.GeneratedFXLayer);
            if (_fXController == null)
            {
                WriteWarning("MergeGeneratedFX", "GeneratedFX Not Found");
            }

            mergeAnimator.animator = _fXController;
            mergeAnimator.pathMode = MergeAnimatorPathMode.Absolute;
            mergeAnimator.matchAvatarWriteDefaults = true;
            mergeAnimator.layerPriority = 9999999;
        }

        /// <summary>
        /// EmoteLayersを同期確保するプレハブ生成
        /// </summary>
        private static void EmoteLayersSync()
        {
            GameObject obj = new GameObject("EmotePrefab_EmoteLayersSync");
            obj.transform.SetParent(Avatar.RootTransform);
            ModularAvatarParameters mparams = obj.AddComponent<ModularAvatarParameters>();
            mparams.parameters.Add(new ParameterConfig()
            {
                nameOrPrefix = "CN_IS_ACTION_ACTIVE",
                syncType = ParameterSyncType.Bool,
                localOnly = true,
            });
            mparams.parameters.Add(new ParameterConfig()
            {
                nameOrPrefix = "CN_IS_ACTION_ACTIVE_FX1",
                syncType = ParameterSyncType.Bool,
                localOnly = true,
            });
            mparams.parameters.Add(new ParameterConfig()
            {
                nameOrPrefix = "CN_IS_ACTION_ACTIVE_FX2",
                syncType = ParameterSyncType.Bool,
                localOnly = true,
            });
        }
    }
}



/* For Reviwer
 * Please review this code for each element and identify areas for improvement, providing a final score out of 100.
 * Please review this code in Japanese.
 */