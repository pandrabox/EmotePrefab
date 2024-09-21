#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using com.github.pandrabox.emoteprefab.runtime;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

namespace com.github.pandrabox.emoteprefab.runtime
{
    public enum TransitionType
    {
        Start,
        AutoExit,
        ManualExit,
        Sit,
        Quick,
        HasQuick,
    }

    [Serializable]
    public class TransitionInfo
    {
        public TransitionType Type;
        public bool HasExitTime;
        public float ExitTime;
        public bool HasFixedDuration;
        public float Duration;
        public float Offset;
        public static TransitionInfo Quick = new TransitionInfo(TransitionType.Quick);
        public static TransitionInfo HasQuick = new TransitionInfo(TransitionType.HasQuick);

        public Dictionary<TransitionType, (bool, float, bool, float, float)> DefaultValues = new Dictionary<TransitionType, (bool, float, bool, float, float)>()
        {
            { TransitionType.Start,         (false, 0.75f, true, 0.25f, 0) },
            { TransitionType.AutoExit,      (true, 0.75f, true, 0.25f, 0) },
            { TransitionType.ManualExit,    (false, 0.75f, true, 0.25f, 0) },
            { TransitionType.Sit,           (false, 0, false, 0, 0) },
            { TransitionType.Quick,         (false, 0, false, 0, 0) },
            { TransitionType.HasQuick,      (true, 0, false, 0, 0) },
        };

        public TransitionInfo(TransitionType type)
        {
            Type = type;
            ValReset();
        }

        public void ValReset()
        {
            (HasExitTime, ExitTime, HasFixedDuration, Duration, Offset) = DefaultValues[Type];
        }
    }
}
#endif