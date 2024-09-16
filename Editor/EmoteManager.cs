// <copyright file="EmoteManager.cs"></copyright>

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
using static com.github.pandrabox.emoteprefab.runtime.Generic;

namespace com.github.pandrabox.emoteprefab.editor
{
    /// <summary>
    /// 各EmotePrefabを統括する静的クラス
    /// </summary>
    public static class EmoteManager
    {
        private static bool _prefabInitialized;
        private static bool _paramInitialized;
        //private static int _currentIndex = 0;
        private static EmotePrefab[] _emotePrefabs;
        private static EmoteProperty[] _emoteProperties;

        /// <summary>
        /// 現行のEmotePrefab
        /// </summary>
        public static EmotePrefab EmotePrefab(int n)
        {
            return EmotePrefabs[n];
        }

        /// <summary>
        /// 現行のEmoteProperty
        /// </summary>
        public static EmoteProperty EmoteProperty(int n)
        {
            return EmoteProperties[n];
        }

        /// <summary>
        /// CurrentがOneShotかどうか
        /// </summary>
        public static bool IsOneShot(int n)
        {
            return EmotePrefab(n).IsOneShot;
        }

        /// <summary>
        /// 現行のID(=VRCEmoteの値)
        /// </summary>
        public static int ID(int n)
        {
            return n + 1;
        }

        /// <summary>
        /// 現行のエモート名
        /// </summary>
        public static string EmoteName(int n)
        {
            return EmotePrefab(n).Name;
        }

        /// <summary>
        /// 現行のState名
        /// </summary>
        public static string StateName(int n)
        {
            return $@"E{ID(n):D3}";
        }

        /// <summary>
        /// 現行のState名
        /// </summary>
        public static string AFKName(int n)
        {
            return $@"A{ID(n):D3}";
        }

        /// <summary>
        /// WDのState名
        /// </summary>
        public static string WDStateName(int n)
        {
            return $@"WD{ID(n):D3}";
        }

        /// <summary>
        /// エモートへ入る遷移条件
        /// </summary>
        public static TransitionInfo StartTransitionInfo(int n)
        {
            return EmotePrefab(n).UseCustomStartTransition ? EmotePrefab(n).StartTransitionInfo : EmoteProperty(n).StartTransitionInfo;
        }

        /// <summary>
        /// 正常終了の遷移条件
        /// </summary>
        public static TransitionInfo RegularExitTransitionInfo(int n)
        {
            return EmotePrefab(n).UseCustomExitTransition ? EmotePrefab(n).ExitTransitionInfo : EmoteProperty(n).RegularExitTransitionInfo;
        }

        /// <summary>
        /// 強制終了の遷移条件
        /// </summary>
        public static TransitionInfo ForceExitTransitionInfo(int n)
        {
            return EmoteProperty(n).ForceExitTransitionInfo;
        }

        /// <summary>
        /// 現行のHumanoidClip
        /// </summary>
        public static AnimationClip HumanoidClip(int n)
        {
            return EmoteProperty(n).Dividedclip.HumanoidClip;
        }

        /// <summary>
        /// 現行のBodyShapeBlockerClip
        /// </summary>
        public static AnimationClip BodyShapeBlockerClip(int n)
        {
            return EmoteProperty(n).Dividedclip.BodyShapeBlockerClip;
        }

        /// <summary>
        /// 現行のUnhumanoidClip
        /// </summary>
        public static AnimationClip UnhumanoidClip(int n)
        {
            return EmoteProperty(n).Dividedclip.UnhumanoidClip;
        }

        /// <summary>
        /// 現行のFakeWriteDefaultClip
        /// </summary>
        public static AnimationClip FakeWriteDefaultClip(int n)
        {
            return EmotePrefab(n).FakeWriteDefaultClip ?? EmoteProperty(n).Dividedclip.FakeWriteDefaultClip;
        }

        /// <summary>
        /// BodyShapeの有無
        /// </summary>
        public static bool HasBodyShape(int n)
        {
            return EmoteProperty(n).Dividedclip.HasBodyShape;
        }

        /// <summary>
        /// Humanoidの有無
        /// </summary>
        public static bool HasHumanoid(int n)
        {
            return EmoteProperty(n).Dividedclip.HasHumanoid;
        }

        /// <summary>
        /// Unhumanoidの有無
        /// </summary>
        public static bool HasUnhumanoid(int n)
        {
            return EmoteProperty(n).Dividedclip.HasUnhumanoid;
        }

        /// <summary>
        /// Expressionに表示するエモートかどうか
        /// </summary>
        public static bool IsEmote(int n)
        {
            return EmotePrefab(n).IsEmote;
        }

        /// <summary>
        /// AFK選定対象かどうか
        /// </summary>
        public static bool IsAFK(int n)
        {
            return EmotePrefab(n).IsAFK;
        }

        /// <summary>
        /// 長さ
        /// </summary>
        public static int Length
        {
            get
            {
                return EmotePrefabs.Length;
            }
        }

        /// <summary>
        /// Expression Emoteが1つ以上あるかどうか
        /// </summary>
        public static bool HasEmote
        {
            get
            {
                return EmotePrefabs.Where(emote => emote.IsEmote).Any();
            }
        }

        /// <summary>
        /// AFKが1つ以上あるかどうか
        /// </summary>
        public static bool HasAFK
        {
            get
            {
                return EmotePrefabs.Where(emote => emote.IsAFK).Any();
            }
        }

        /// <summary>
        /// Emote又はAFKが1つ以上あるかどうか
        /// </summary>
        public static bool HasItem
        {
            get
            {
                return HasEmote || HasAFK;
            }
        }

        /// <summary>
        /// AFKの数
        /// </summary>
        public static int AFKCount
        {
            get
            {
                return EmotePrefabs.Where(emote => emote.IsAFK).Count();
            }
        }

        public static bool HasAnimateAllPhysBones
        {
            get
            {
                return EmotePrefabs.Where(emote => emote.AnimateAllPhysBones).Any();
            }
        }

        public static VRCPhysBone[] AnimatePhysBones
        {
            get
            {
                return EmotePrefabs
                    .Where(prefab => prefab.AnimatePhysBones != null) // nullチェック
                    .SelectMany(prefab => prefab.AnimatePhysBones.Where(bone => bone != null))
                    .Distinct()
                    .ToArray();
            }
        }

        /// <summary>
        /// 有効性確認
        /// </summary>
        /// <returns>有効かどうか</returns>
        public static bool Enable(int n)
        {
            return n < Length;
        }

        /// <summary>
        /// EmotePrefabsアクセス用
        /// </summary>
        private static EmotePrefab[] EmotePrefabs
        {
            get
            {
                if (!_prefabInitialized)
                {
                    _prefabInitialized = true;
                    LoadEmotePrefabs();
                    _emotePrefabs = _emotePrefabs.OrderBy(c => c.Name).ToArray();
                }

                return _emotePrefabs;

                void LoadEmotePrefabs()
                {
                    void RenewEmotePrefabs()
                    {
                        _paramInitialized = false;
                        _emotePrefabs = Avatar.RootTransform.GetComponentsInChildren<EmotePrefab>(false)
                            .Where(emote => emote.Motion != null)
                            .Where(emote => emote.IsAFK || emote.IsEmote)
                            .ToArray();
                    }
                    RenewEmotePrefabs();
                    if (CreateDefaultAFK())
                    {
                        RenewEmotePrefabs();
                    }
                }
            }
        }

        /// <summary>
        /// EmotePropertiesアクセス用
        /// </summary>
        private static EmoteProperty[] EmoteProperties
        {
            get
            {
                if (!_paramInitialized)
                {
                    _paramInitialized = true;
                    _emoteProperties = new EmoteProperty[EmotePrefabs.Length];
                    for (int i = 0; i < Length; i++)
                    {
                        _emoteProperties[i] = new EmoteProperty(i);
                    }
                }

                return _emoteProperties;
            }
        }

        /// <summary>
        /// 初期化済状態の解除
        /// </summary>
        public static void Clear()
        {
            _prefabInitialized = false;
            _paramInitialized = false;
        }

        /// <summary>
        /// Itemがあり、AFKがない場合、DefaultAFKを作成
        /// </summary>
        /// <returns>作成処理をした場合true, しなかった場合false</returns>
        private static bool CreateDefaultAFK()
        {
            if (!HasItem || HasAFK)
            {
                return false;
            }

            var obj = new GameObject();
            obj.transform.SetParent(Avatar.RootTransform);
            var emotePrefab = obj.AddComponent<EmotePrefab>();
            emotePrefab.Motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(Config.OfficialAFKClip) ?? AssetDatabase.LoadAssetAtPath<AnimationClip>(Config.BackupAFKClip);
            if (emotePrefab.Motion == null) 
            {
                WriteWarning("EmoteManager.CreateDefaultAFK", "AFK Clip Not Found");
            }
            obj.name = "EmotePrefab_DefaultAFK";
            emotePrefab.IsAFK = true;
            emotePrefab.IsEmote = false;
            return true;
        }
    }
}


/* For Reviwer
 * Please review this code for each element and identify areas for improvement, providing a final score out of 100.
 * Please review this code in Japanese.
 */