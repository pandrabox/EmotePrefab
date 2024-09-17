// <copyright file="EmotePrefabEditor.cs"></copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using com.github.pandrabox.emoteprefab.editor;
using com.github.pandrabox.emoteprefab.runtime;
using nadena.dev.modular_avatar.core;
using nadena.dev.modular_avatar.core.editor;
using nadena.dev.ndmf;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;
using static com.github.pandrabox.emoteprefab.runtime.Generic;

namespace com.github.pandrabox.emoteprefab.editor
{
    [CustomEditor(typeof(EmotePrefab))]
    public class EmotePrefabEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var nowInstance = (EmotePrefab)target;
            var spMotions = serializedObject.FindProperty("Motions");
            var unitMotion = spMotions.GetArrayElementAtIndex(0);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            nowInstance.Motion = (AnimationClip)EditorGUILayout.ObjectField("Motion", nowInstance.Motion, typeof(AnimationClip), false);
            nowInstance.Name = EditorGUILayout.TextField("Name", nowInstance.Name);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IsOneShot"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IsEmote"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IsAFK"));
            var spIcon = serializedObject.FindProperty("Icon");
            EditorGUILayout.PropertyField(spIcon);

            EditorGUILayout.EndVertical();

            if (spIcon != null)
            {
                var tex = spIcon.objectReferenceValue as Texture2D;
                if (tex != null && !spIcon.hasMultipleDifferentValues)
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

            EditorGUILayout.EndHorizontal();



            var spViewAdvancedOptions = serializedObject.FindProperty("ViewAdvancedOptions");
            EditorGUILayout.PropertyField(spViewAdvancedOptions);
            if (spViewAdvancedOptions.boolValue)
            {
                var spAnimateAllPhysBones = serializedObject.FindProperty("AnimateAllPhysBones");
                EditorGUILayout.PropertyField(spAnimateAllPhysBones);
                if (!spAnimateAllPhysBones.boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("AnimatePhysBones"));
                }

                var spShrinkAllPhysBones = serializedObject.FindProperty("ShrinkAllPhysBones");
                EditorGUILayout.PropertyField(spShrinkAllPhysBones);
                if (!spShrinkAllPhysBones.boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ShrinkPhysBones"));
                }

                var spUseCustomStartTransition = serializedObject.FindProperty("UseCustomStartTransition");
                EditorGUILayout.PropertyField(spUseCustomStartTransition);
                if (spUseCustomStartTransition.boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("StartTransitionInfo"));
                }

                EditorGUILayout.PropertyField(unitMotion.FindPropertyRelative("UseCustomExitTransition"));
                if (nowInstance.Motions[0].UseCustomExitTransition)
                {
                    EditorGUILayout.PropertyField(unitMotion.FindPropertyRelative("ExitTransitionInfo"));
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("FakeWriteDefaultClip"));


                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("ChainMotion");
                    if (GUILayout.Button("+"))
                    {
                        nowInstance.Motions.Add(new UnitMotion());
                    }
                    if (GUILayout.Button("-"))
                    {
                        if (nowInstance.Motions.Count > 1)
                        {
                            nowInstance.Motions.RemoveAt(nowInstance.Motions.Count - 1);
                        }
                    }
                }
                for(int i = 1;i<nowInstance.Motions.Count;i++) 
                {
                    if (i >= spMotions.arraySize)
                    {
                        continue;
                    }
                    unitMotion = spMotions.GetArrayElementAtIndex(i);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("", GUILayout.Width(20));
                        using (new EditorGUILayout.VerticalScope())
                        {
                            EditorGUILayout.PropertyField(unitMotion.FindPropertyRelative("Motion"));
                            EditorGUILayout.PropertyField(unitMotion.FindPropertyRelative("UseCustomExitTransition"));
                        }
                        EditorGUILayout.LabelField("", GUILayout.Width(20));

                        if (nowInstance.Motions[i].UseCustomExitTransition)
                        {
                            EditorGUILayout.PropertyField(unitMotion.FindPropertyRelative("ExitTransitionInfo"));
                        }

                    }
                }


            }








            serializedObject.ApplyModifiedProperties();
        }
    }
}