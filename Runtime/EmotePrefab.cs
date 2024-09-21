// <copyright file="EmotePrefab.cs"></copyright>

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using VRC.SDK3.Dynamics.PhysBone.Components;
using System.Linq;

namespace com.github.pandrabox.emoteprefab.runtime
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Pan/EmotePrefab")]
    public class EmotePrefab : MonoBehaviour, VRC.SDKBase.IEditorOnly
    {
        public bool IsEmote=true;
        public bool IsAFK;
        public Texture2D Icon;
        [SerializeField]
        private string _name;
        public PhysBoneSelection AnimatePhysBone = new PhysBoneSelection();
        public PhysBoneSelection ShrinkPhysBone = new PhysBoneSelection();
        public List<UnitMotion> UnitMotions = new List<UnitMotion> { new UnitMotion() };
        public UnitMotion RootMotion => UnitMotions.FirstOrDefault();
        public AnimationClip RootClip { get => RootMotion.Clip.Original; set => RootMotion.Clip.Original = value; }
        public bool Enable => RootClip != null && (IsEmote || IsAFK);
        public bool ViewAdvancedOptions;
        public AnimationClip Motion
        {
            get => RootClip;
            set
            {
                if (value == RootClip) return;
                try
                {
                    if (value == null)
                    {
                        RootClip = null;
                        Name = string.Empty;
                    }
                    else
                    {
                        RootClip = value;
                        Name = RootClip.name.Replace("proxy_stand_", string.Empty).Replace("proxy_", string.Empty);
                    }
                }
                finally
                {
                    EditorUtility.SetDirty(this);
                }
            }
        }
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                gameObject.name = $@"{Config.EmotePrefabObjectPrefix}{_name}";
                EditorUtility.SetDirty(this);
            }
        }

        public void SetEasy(AnimationClip rootClip, bool isEmote, bool isAFK)
        {
            RootClip = rootClip;
            IsEmote = isEmote;
            IsAFK = isAFK;
            if (Name != null) Name = name;
        }
    }

    [CustomEditor(typeof(EmotePrefab))]
    public class EmotePrefabEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var nowInstance = (EmotePrefab)target;
            var exMode = nowInstance.UnitMotions.FirstOrDefault().Mode;
            serializedObject.Update();
            try
            {

                using (new EditorGUILayout.HorizontalScope())
                {
                    var IconProp = serializedObject.FindProperty("Icon");
                    using (new EditorGUILayout.VerticalScope())
                    {
                        nowInstance.Motion = (AnimationClip)EditorGUILayout.ObjectField("Motion", nowInstance.Motion, typeof(AnimationClip), false);
                        nowInstance.Name = EditorGUILayout.TextField("Name", nowInstance.Name);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("IsEmote"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("IsAFK"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("Icon"));
                    }

                    if (IconProp != null)
                    {
                        var tex = IconProp.objectReferenceValue as Texture2D;
                        if (tex != null && !IconProp.hasMultipleDifferentValues)
                        {
                            var size = EditorGUIUtility.singleLineHeight * 5;
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

                if (exMode > 0)
                {
                    var UnitMotionsProp = serializedObject.FindProperty("UnitMotions");
                    var rect2 = GUILayoutUtility.GetRect(Screen.width, EditorGUIUtility.singleLineHeight);
                    var rect3 = new Rect(rect2.x, rect2.y, rect2.width / 4, rect2.height);
                    EditorGUI.LabelField(rect3, "Chain Emote");
                    rect3 = new Rect(rect2.x + rect2.width / 4, rect2.y, rect2.width / 4, rect2.height);
                    if (GUI.Button(rect3, "Ex")) modeup();
                    rect3 = new Rect(rect2.x + rect2.width / 2, rect2.y, rect2.width / 4, rect2.height);
                    if (GUI.Button(rect3, "-")) UnitMotionsProp.arraySize -= UnitMotionsProp.arraySize <= 1 ? 0 : 1;
                    rect3 = new Rect(rect2.x + rect2.width / 4 * 3, rect2.y, rect2.width / 4, rect2.height);
                    if (GUI.Button(rect3, "+")) UnitMotionsProp.arraySize++;
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < UnitMotionsProp.arraySize; ++i)
                    {
                        EditorGUILayout.PropertyField(UnitMotionsProp.GetArrayElementAtIndex(i));
                    }
                    EditorGUI.indentLevel--;
                    if (exMode > 0)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("AnimatePhysBone"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("ShrinkPhysBone"));
                    }
                }
                else
                {
                    if (GUILayout.Button("Ex")) modeup();
                }
            }
            finally
            {
                serializedObject.ApplyModifiedProperties();
            }
            void modeup()
            {
                exMode++;
                if (exMode > 2) exMode = 0;
                foreach (var u in nowInstance.UnitMotions)
                {
                    u.Mode = exMode;
                }
            }
        }
    }
}
#endif