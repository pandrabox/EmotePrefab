// <copyright file="TransitionInfo.cs"></copyright>

#if UNITY_EDITOR

#pragma warning disable SA1401 // Fields should be private

using System;

namespace com.github.pandrabox.emoteprefab.runtime
{
    /// <summary>
    /// Transitionパラメータを管理するクラス
    /// </summary>
    [Serializable]
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

#endif