// <copyright file="TransitionInfo.cs"></copyright>

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

#pragma warning disable SA1401 // Fields should be private

namespace com.github.pandrabox.emoteprefab.editor
{
    /// <summary>
    /// Transitionパラメータを管理するクラス
    /// </summary>
    public class TransitionInfo
    {
        public bool HasExitTime;
        public float ExitTime;
        public bool HasFixedDuration;
        public float Duration;
        public float Offset;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="hasExitTime">From完了まで待つならTrue, 条件ですぐ遷移ならfalse</param>
        /// <param name="exitTime">hasExitTimeがtrueのとき遷移開始するタイミング(%)</param>
        /// <param name="hasFixedDuration">durationの単位指定。trueなら秒,falseならState正規時間への%</param>
        /// <param name="duration">遷移時間</param>
        /// <param name="offset">遷移後のStateのOffset</param>
        public TransitionInfo(bool hasExitTime, float exitTime, bool hasFixedDuration, float duration, float offset = 0)
        {
            HasExitTime = hasExitTime;
            ExitTime = exitTime;
            HasFixedDuration = hasFixedDuration;
            Duration = duration;
            Offset = offset;
        }
    }
}
