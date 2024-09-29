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



    [CustomEditor(typeof(EmoteFolder))]
    public class EmoteFolderEditor : EmotePrefabEditorLayout
    {
        public override void OnInspectorGUI()
        {
            DrawIcoAndLogo();
            serializedObject.Update();
            try
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    var IconProp = serializedObject.FindProperty("Icon");
                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("FolderName"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("Sort"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoFolderMode"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("Icon"));

                    }

                    if (IconProp != null)
                    {
                        var tex = IconProp.objectReferenceValue as Texture2D;
                        if (tex != null && !IconProp.hasMultipleDifferentValues)
                        {
                            var size = EditorGUIUtility.singleLineHeight * 4;
                            var margin = 4;
                            var withMargin = new Vector2(margin + size, margin + size);

                            var rect = GUILayoutUtility.GetRect(withMargin.x, withMargin.y, GUILayout.ExpandWidth(false),
                                GUILayout.ExpandHeight(true));
                            rect.x += margin;
                            rect.y = rect.y + rect.height / 2 - size / 2;
                            rect.width = size;
                            rect.height = size;

                            GUI.Box(rect, new GUIContent(), "flow node 1");
                            GUI.DrawTexture(rect, tex);
                        }
                    }
                }
            }
            finally
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif