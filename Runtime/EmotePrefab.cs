using UnityEditor;
using UnityEngine;

namespace com.github.pandrabox.emoteprefab.runtime
{
    [AddComponentMenu("Pan/EmotePrefab")]
    public class EmotePrefab : MonoBehaviour, VRC.SDKBase.IEditorOnly
    {
        public Motion Motion;
        public string Name;
        public bool IsOneShot;

#if UNITY_EDITOR
        [ExecuteInEditMode]
        [CustomEditor(typeof(EmotePrefab))]
        public class EmotePrefabEditor : UnityEditor.Editor, VRC.SDKBase.IEditorOnly
        {
            EmotePrefab NowInstance;
            public override void OnInspectorGUI()
            {
                var IsChanged = false;
                EditorGUI.BeginChangeCheck();
                NowInstance = (EmotePrefab)target;
                if (NowInstance == null) return;

                EditorGUI.BeginChangeCheck();
                NowInstance.Motion = (Motion)EditorGUILayout.ObjectField("Motion", NowInstance.Motion, typeof(Motion), false);
                if (EditorGUI.EndChangeCheck())
                {
                    IsChanged = true;
                    var name = NowInstance.Motion.name;
                    name = name.Replace("proxy_stand_", "");
                    name = name.Replace("proxy_", "");
                    NowInstance.Name = name;
                    RenewPrefabName();
                    NowInstance.IsOneShot = !NowInstance.Motion.isLooping;
                }

                EditorGUI.BeginChangeCheck();
                NowInstance.Name = EditorGUILayout.TextField("Name", NowInstance.Name);
                if (EditorGUI.EndChangeCheck())
                {
                    RenewPrefabName();
                    IsChanged = true;
                }

                EditorGUI.BeginChangeCheck();
                NowInstance.IsOneShot = EditorGUILayout.Toggle("IsOneShot", NowInstance.IsOneShot);
                if (EditorGUI.EndChangeCheck())
                {
                    IsChanged = true;
                }



                if (IsChanged)
                {
                    EditorUtility.SetDirty(NowInstance);
                }
            }
            private void RenewPrefabName()
            {
                var name = $@"E_{NowInstance.Name}";
                NowInstance.gameObject.name = name;
            }
        }
#endif
    }
}