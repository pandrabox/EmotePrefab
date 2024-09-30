// <copyright file="EmotePrefab.cs"></copyright>

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using VRC.SDK3.Dynamics.PhysBone.Components;
using System.Linq;
using UnityEditor.Animations;

namespace com.github.pandrabox.emoteprefab.runtime
{
    public class NBitInt : MonoBehaviour, VRC.SDKBase.IEditorOnly
    {
        public string ParameterName;
        public int MaxValue;
    }
}
#endif