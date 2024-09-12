// <copyright file="EmoteManager.cs"></copyright>
#pragma warning disable SA1201 // Elements should appear in the correct order

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
    /// 各EmotePrefabを統括する静的クラス
    /// </summary>
    public static class EmoteManager
    {
        private static bool _initialized;
        private static int _currentIndex = 0;
        private static EmotePrefab[] _emotePrefabs;
        private static EmoteProperty[] _emoteProperties;

        public static void clear()
        {
            _initialized = false;
        }

        /// <summary>
        /// EmoteManagerの初期化
        /// </summary>
        /// <returns>初期化成功フラグ</returns>
        public static bool Init()
        {
            if (_initialized)
            {
                WriteWarning("EmoteManager.Init", "EmoteManager has been initialized twice, which is not allowed.");
                return false;
            }

            _initialized = true;
            _emotePrefabs = Avatar.RootTransform.GetComponentsInChildren<EmotePrefab>(false)
                .Where(emote => emote.Motion != null)
                .ToArray();
            if (_emotePrefabs.Length == 0)
            {
                return false;
            }

            _emotePrefabs = _emotePrefabs.OrderBy(c => c.Name).ToArray();
            _currentIndex = 0;
            InitEmoteProperties();
            return true;
        }

        /// <summary>
        /// EmoteManagerの初期化チェック
        /// </summary>
        private static void InitCheck()
        {
            if (!_initialized)
            {
                Init();
            }
        }

        /// <summary>
        /// EmotePropertiesの初期化
        /// </summary>
        private static void InitEmoteProperties()
        {
            _emoteProperties = new EmoteProperty[_emotePrefabs.Length];
            MoveFirst();
            while (Enable)
            {
                _emoteProperties[_currentIndex] = new EmoteProperty();
                Next();
            }
        }

        /// <summary>
        /// 現行のEmotePrefab
        /// </summary>
        public static EmotePrefab EmotePrefab
        {
            get
            {
                InitCheck();
                return _emotePrefabs[_currentIndex];
            }
        }

        /// <summary>
        /// 現行のEmoteProperty
        /// </summary>
        public static EmoteProperty EmoteProperty
        {
            get
            {
                InitCheck();
                return _emoteProperties[_currentIndex];
            }
        }

        /// <summary>
        /// CurrentがOneShotかどうか
        /// </summary>
        public static bool IsOneShot
        {
            get
            {
                InitCheck();
                return EmotePrefab.IsOneShot;
            }
        }

        /// <summary>
        /// 現行のID(=VRCEmoteの値)
        /// </summary>
        public static int ID
        {
            get
            {
                InitCheck();
                return _currentIndex + 1;
            }
        }

        /// <summary>
        /// 現行のエモート名
        /// </summary>
        public static string EmoteName
        {
            get
            {
                InitCheck();
                return EmotePrefab.Name;
            }
        }

        /// <summary>
        /// 現行のState名
        /// </summary>
        public static string StateName
        {
            get
            {
                InitCheck();
                return $@"E{ID:D3}";
            }
        }

        /// <summary>
        /// WDのState名
        /// </summary>
        public static string WDStateName
        {
            get
            {
                InitCheck();
                return $@"WD{ID:D3}";
            }
        }

        /// <summary>
        /// エモートへ入る遷移条件
        /// </summary>
        public static TransitionInfo StartTransitionInfo
        {
            get
            {
                InitCheck();
                return EmoteProperty.StartTransitionInfo;
            }
        }

        /// <summary>
        /// 正常終了の遷移条件
        /// </summary>
        public static TransitionInfo RegularExitTransitionInfo
        {
            get
            {
                InitCheck();
                return EmoteProperty.RegularExitTransitionInfo;
            }
        }

        /// <summary>
        /// 強制終了の遷移条件
        /// </summary>
        public static TransitionInfo ForceExitTransitionInfo
        {
            get
            {
                InitCheck();
                return EmoteProperty.ForceExitTransitionInfo;
            }
        }

        /// <summary>
        /// 現行のHumanoidClip
        /// </summary>
        public static AnimationClip HumanoidClip
        {
            get
            {
                InitCheck();
                return EmoteProperty.Dividedclip.HumanoidClip;
            }
        }

        /// <summary>
        /// 現行のBodyShapeBlockerClip
        /// </summary>
        public static AnimationClip BodyShapeBlockerClip
        {
            get
            {
                InitCheck();
                return EmoteProperty.Dividedclip.BodyShapeBlockerClip;
            }
        }

        /// <summary>
        /// 現行のUnhumanoidClip
        /// </summary>
        public static AnimationClip UnhumanoidClip
        {
            get
            {
                InitCheck();
                return EmoteProperty.Dividedclip.UnhumanoidClip;
            }
        }

        /// <summary>
        /// 現行のFakeWriteDefaultClip
        /// </summary>
        public static AnimationClip FakeWriteDefaultClip
        {
            get
            {
                InitCheck();
                return EmoteProperty.Dividedclip.FakeWriteDefaultClip;
            }
        }

        /// <summary>
        /// BodyShapeの有無
        /// </summary>
        public static bool HasBodyShape
        {
            get
            {
                InitCheck();
                return EmoteProperty.Dividedclip.HasBodyShape;
            }
        }

        /// <summary>
        /// Humanoidの有無
        /// </summary>
        public static bool HasHumanoid
        {
            get
            {
                InitCheck();
                return EmoteProperty.Dividedclip.HasHumanoid;
            }
        }

        /// <summary>
        /// Unhumanoidの有無
        /// </summary>
        public static bool HasUnhumanoid
        {
            get
            {
                InitCheck();
                return EmoteProperty.Dividedclip.HasUnhumanoid;
            }
        }

        /// <summary>
        /// 有効性確認
        /// </summary>
        /// <returns>有効かどうか</returns>
        public static bool Enable
        {
            get
            {
                InitCheck();
                return _currentIndex < _emotePrefabs.Length;
            }
        }

        /// <summary>
        /// Currentを次に進める
        /// </summary>
        /// <returns>成否</returns>
        public static void Next()
        {
            InitCheck();
            if (Enable)
            {
                _currentIndex++;
            }
        }

        /// <summary>
        /// 最初に戻る
        /// </summary>
        public static void MoveFirst()
        {
            InitCheck();
            _currentIndex = 0;
        }
    }
}


/* For Reviwer
 * Please review this code for each element and identify areas for improvement, providing a final score out of 100.
 * Please review this code in Japanese.
 */