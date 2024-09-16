// <copyright file="EmotePrefab.cs"></copyright>

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using VRC.SDK3.Dynamics.PhysBone.Components;
using System;

namespace com.github.pandrabox.emoteprefab.runtime
{
    /// <summary>
    /// ユーザがアクセスするメインUI
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Pan/EmotePrefab")]
    [Serializable]
    public class EmotePrefab : MonoBehaviour, VRC.SDKBase.IEditorOnly
    {
        public bool IsOneShot;
        public bool IsEmote;
        public bool IsAFK;
        public Texture2D Icon;
        public bool ViewAdvancedOptions;
        public bool AnimateAllPhysBones;
        public List<VRCPhysBone> AnimatePhysBones;
        public bool ShrinkAllPhysBones;
        public List<VRCPhysBone> ShrinkPhysBones;
        public bool UseCustomStartTransition;
        public TransitionInfo StartTransitionInfo;
        public bool UseCustomExitTransition;
        public TransitionInfo ExitTransitionInfo;
        public AnimationClip FakeWriteDefaultClip;
        public EmotePrefab ChainTo;
        public EmotePrefab ChainFrom;

        [SerializeField]
        private AnimationClip _motion;
        [SerializeField]
        private string _name;

        /// <summary>
        /// Emoteのファイル　設定時、名称とループ設定を自動初期化
        /// </summary>
        public AnimationClip Motion
        {
            get => _motion;
            set
            {
                if (_motion != value)
                {
                    _motion = value;

                    if (_motion != null)
                    {
                        Name = _motion.name.Replace("proxy_stand_", string.Empty).Replace("proxy_", string.Empty);
                        IsOneShot = !_motion.isLooping;
                        IsEmote = true;
                    }
                    else
                    {
                        Name = string.Empty;
                        IsOneShot = false;
                    }

                    EditorUtility.SetDirty(this);
                }
            }
        }

        /// <summary>
        /// ExpressionMenuに表示する名称。GameObjectの名称も同期
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    gameObject.name = $@"{Config.EmotePrefabObjectPrefix}{_name}";
                    EditorUtility.SetDirty(this);
                }
            }
        }
    }
}

#endif
/* For Reviwer
    VRC.SDKBase.IEditorOnly is an official VRC interface. It is used to mark a script as editor-only.
    Please review this code in Japanese.
*/