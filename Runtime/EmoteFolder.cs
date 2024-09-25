#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using VRC.SDK3.Dynamics.PhysBone.Components;
using System.Linq;
using VRC.SDK3.Avatars.Components;
using static com.github.pandrabox.emoteprefab.runtime.Generic;

namespace com.github.pandrabox.emoteprefab.runtime
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Pan/EmoteFolder")]
    public class EmoteFolder : EmoteMenuInfo
    {
        public string FolderName;
        public Texture2D Icon;
    }
}
#endif