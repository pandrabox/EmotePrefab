#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using com.github.pandrabox.emoteprefab.runtime;
using nadena.dev.ndmf;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;
using static com.github.pandrabox.emoteprefab.runtime.Generic;

namespace com.github.pandrabox.emoteprefab.runtime
{
    [Serializable]
    [DisallowMultipleComponent]
    [AddComponentMenu("Pan/EmoteMenuInfo")]
    public class EmoteMenuInfo : MonoBehaviour, VRC.SDKBase.IEditorOnly
    {
        public enum AutoFolderModeType
        {
            sub,
            parallel,
            none
        }
        public bool Sort;
        public AutoFolderModeType AutoFolderMode;
    }
}
#endif