// <copyright file="EmotePrefab.cs"></copyright>

using UnityEditor;
using UnityEngine;

namespace com.github.pandrabox.emoteprefab.runtime
{
    /// <summary>
    /// ユーザがアクセスするメインUI
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Pan/EmotePrefab")]
    public class EmotePrefab : MonoBehaviour, VRC.SDKBase.IEditorOnly
    {
        [SerializeField]
        private AnimationClip _motion=null;
        [SerializeField]
        private string _name="";
        [SerializeField]
        private bool _isOneShot=false;

#if UNITY_EDITOR
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
                        _isOneShot = !_motion.isLooping;
                    }
                    else
                    {
                        Name = string.Empty;
                        _isOneShot = false;
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

        /// <summary>
        /// ExpressionMenuから選択時1回実行後終了するEmoteならばtrue, その他(ループやホールド)ならばFalse
        /// </summary>
        public bool IsOneShot
        {
            get => _isOneShot;
            set
            {
                if (_isOneShot != value)
                {
                    _isOneShot = value;
                    EditorUtility.SetDirty(this);
                }
            }
        }

        [ExecuteInEditMode]
        [CustomEditor(typeof(EmotePrefab))]
        public class EmotePrefabEditor : UnityEditor.Editor, VRC.SDKBase.IEditorOnly
        {
            EmotePrefab NowInstance;
            public override void OnInspectorGUI()
            {
                NowInstance = (EmotePrefab)target;
                NowInstance.Motion = (AnimationClip)EditorGUILayout.ObjectField("Motion", NowInstance.Motion, typeof(AnimationClip), false);
                NowInstance.Name = EditorGUILayout.TextField("Name", NowInstance.Name);
                NowInstance.IsOneShot = EditorGUILayout.Toggle("IsOneShot", NowInstance.IsOneShot);
            }
        }
#endif
    }
}

/* For Reviwer
    VRC.SDKBase.IEditorOnly is an official VRC interface. It is used to mark a script as editor-only.
    Please review this code in Japanese.
*/