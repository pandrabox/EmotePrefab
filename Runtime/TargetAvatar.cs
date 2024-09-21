// <copyright file="Generic.cs"></copyright>
#pragma warning disable SA1600 // Elements should be documented

using System;
using System.Collections.Generic;
using System.Linq;
using nadena.dev.ndmf;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace com.github.pandrabox.emoteprefab.runtime
{
    public class TargetAvatar
    {
        public VRCAvatarDescriptor Descriptor;
        public GameObject RootObject => Descriptor.gameObject;
        public Transform RootTransform => Descriptor.transform;

        public TargetAvatar(VRCAvatarDescriptor descriptor)
        {
            Descriptor = descriptor;
        }
    }
}

