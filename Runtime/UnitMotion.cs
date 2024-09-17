// <copyright file="TransitionInfo.cs"></copyright>

#if UNITY_EDITOR

#pragma warning disable SA1401 // Fields should be private

using System;
using UnityEngine;

namespace com.github.pandrabox.emoteprefab.runtime
{
    [Serializable]
    public class UnitMotion
    {
        public AnimationClip Motion;
        public bool UseCustomExitTransition;
        public TransitionInfo ExitTransitionInfo;
    }
}

#endif