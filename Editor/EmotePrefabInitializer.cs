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
    /// EmotePrefabの実行にあたって必要な初期化処理
    /// </summary>
    public class EmotePrefabInitializer
    {
        public EmotePrefabInitializer(VRCAvatarDescriptor descriptor)
        {
            Descriptor = descriptor;
            GetEmotePrefabs();
            if (!HasTask) return;
            CreateWorkDir();
            CopyControllers();
            CreateRootObject();
            CreateActionObject();
            CreateFXObject();
            CreateFXRelativeObject();
            CreateSyncObject();
            CreateVRCEmote();
            CreateDefaultAFK();
            AnimatePhysBones();
        }

        private static void GetEmotePrefabs()
        {
            EmotePrefabs = Descriptor.transform.GetComponentsInChildren<EmotePrefab>(false).Where(e => e.Enable).ToArray();
            foreach(var prefab in EmotePrefabs)
            {
                prefab.RestorePhysBones();
            }
        }

        /// <summary>
        /// 作業フォルダ生成
        /// </summary>
        private static void CreateWorkDir()
        {
            Directory.CreateDirectory(Config.WorkDir);
        }

        /// <summary>
        /// コントローラアセット生成
        /// </summary>
        private static void CopyControllers()
        {
            if (!AssetDatabase.CopyAsset(Config.OriginalActionLayer, Config.GeneratedActionLayer))
            {
                WriteWarning("WorkSpace", "GeneratedActionLayerの生成に失敗しました");
            }
            if (!AssetDatabase.CopyAsset(Config.OriginalFXLayer, Config.GeneratedFXLayer))
            {
                WriteWarning("WorkSpace", "GeneratedFXLayerの生成に失敗しました");
            }
            if (!AssetDatabase.CopyAsset(Config.OriginalFXRelativeLayer, Config.GeneratedFXRelativeLayer))
            {
                WriteWarning("WorkSpace", "GeneratedFXRelativeLayerの生成に失敗しました");
            }
        }

        /// <summary>
        /// アバター上の作業Rootオブジェクト生成
        /// </summary>
        private void CreateRootObject()
        {
            EmotePrefabRootObject = new GameObject("EmotePrefabRootObject");
            EmotePrefabRootObject.transform.SetParent(Descriptor.transform);
        }

        /// <summary>
        /// Action置換オブジェクト生成
        /// </summary>
        private void CreateActionObject()
        {
            ActionObject = new GameObject("Action");
            ActionObject.transform.SetParent(EmotePrefabRootTransform);
            ActionController = AssetDatabase.LoadAssetAtPath<AnimatorController>(Config.GeneratedActionLayer);
            var c = ActionObject.AddComponent<ReplaceAction>();
            c.ActionController = ActionController;
        }

        /// <summary>
        /// FX追加オブジェクト生成
        /// </summary>
        private void CreateFXObject()
        {
            FXObject = new GameObject("FX");
            FXObject.transform.SetParent(EmotePrefabRootTransform);
            FXController = AssetDatabase.LoadAssetAtPath<AnimatorController>(Config.GeneratedFXLayer);
            var mergeAnimator = FXObject.AddComponent<ModularAvatarMergeAnimator>();
            mergeAnimator.animator = FXController;
            mergeAnimator.pathMode = MergeAnimatorPathMode.Absolute;
            mergeAnimator.matchAvatarWriteDefaults = true;
            mergeAnimator.layerPriority = 9999999;
        }

        /// <summary>
        /// FXRelative追加オブジェクト生成
        /// </summary>
        private void CreateFXRelativeObject()
        {
            FXRelativeObject = new GameObject("FXRelative");
            FXRelativeObject.transform.SetParent(EmotePrefabRootTransform);
            FXRelativeController = AssetDatabase.LoadAssetAtPath<AnimatorController>(Config.GeneratedFXRelativeLayer);
            var mergeAnimator = FXRelativeObject.AddComponent<ModularAvatarMergeAnimator>();
            mergeAnimator.animator = FXRelativeController;
            mergeAnimator.pathMode = MergeAnimatorPathMode.Relative;
            mergeAnimator.matchAvatarWriteDefaults = true;
            mergeAnimator.layerPriority = 9999999;
            mergeAnimator.relativePathRoot.Set(Descriptor.gameObject);
        }

        /// <summary>
        /// レイヤ間同期パラメータ生成
        /// </summary>
        private void CreateSyncObject()
        {
            SyncObject = new GameObject("Sync");
            SyncObject.transform.SetParent(EmotePrefabRootTransform);
            ModularAvatarParameters mparams = SyncObject.AddComponent<ModularAvatarParameters>();
            string[] addBoolLocalParameters = new string[] { "CN_IS_ACTION_ACTIVE", "CN_IS_ACTION_ACTIVE_FX1", "CN_IS_ACTION_ACTIVE_FX2" };
            foreach (string addBoolLocalParameter in addBoolLocalParameters)
            {
                mparams.parameters.Add(new ParameterConfig()
                {
                    nameOrPrefix = addBoolLocalParameter,
                    syncType = ParameterSyncType.Bool,
                    localOnly = true,
                });
            }
        }

        /// <summary>
        /// エモート用パラメータ
        /// </summary>
        private void CreateVRCEmote()
        {
            SyncObject = new GameObject("VRCEmote");
            SyncObject.transform.SetParent(EmotePrefabRootTransform);
            ModularAvatarParameters mparams = SyncObject.AddComponent<ModularAvatarParameters>();
            mparams.parameters.Add(new ParameterConfig()
            {
                nameOrPrefix = "VRCEmote",
                syncType = ParameterSyncType.Int,
                localOnly = false,
            });
        }

        /// <summary>
        /// AFK指定がない場合デフォルトAFK生成
        /// </summary>
        private void CreateDefaultAFK()
        {
            if (HasAFK) return;
            var obj = new GameObject("DefaultAFK");
            obj.transform.SetParent(EmotePrefabRootTransform);
            var emotePrefab = obj.AddComponent<EmotePrefab>();
            var AFKClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(Config.AFKClip);
            emotePrefab.SetEasy(AFKClip, false, true);
            GetEmotePrefabs();
        }

        /// <summary>
        /// PhysBoneのAnimated指定
        /// </summary>
        public void AnimatePhysBones()
        {
            VRCPhysBone[] AnimatePhysBone()
            {
                if (EmotePrefabs.Where(e => e.AnimatePhysBone.All).Any())
                {
                    return Descriptor.transform.GetComponentsInChildren<VRCPhysBone>(true);
                }
                else
                {
                    return EmotePrefabs
                        .Where(prefab => prefab.AnimatePhysBone.PhysBones != null)
                        .SelectMany(prefab => prefab.AnimatePhysBone.PhysBones.Where(bone => bone != null))
                        .Distinct()
                        .ToArray();
                }
            }
            foreach (var targetPhysBone in AnimatePhysBone())
            {
                targetPhysBone.isAnimated = true;
            }
        }
    }
}

