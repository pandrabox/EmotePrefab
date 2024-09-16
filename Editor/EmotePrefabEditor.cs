// <copyright file="EmotePrefabEditor.cs"></copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using com.github.pandrabox.emoteprefab.editor;
using com.github.pandrabox.emoteprefab.runtime;
using nadena.dev.modular_avatar.core;
using nadena.dev.modular_avatar.core.editor;
using nadena.dev.ndmf;
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
            nowInstance.Motion = (AnimationClip)EditorGUILayout.ObjectField("Motion", nowInstance.Motion, typeof(AnimationClip), false);
            nowInstance.Name = EditorGUILayout.TextField("Name", nowInstance.Name);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IsOneShot"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IsEmote"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IsAFK"));

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

                var spUseCustomExitTransition = serializedObject.FindProperty("UseCustomExitTransition");
                EditorGUILayout.PropertyField(spUseCustomExitTransition);
                if (spUseCustomExitTransition.boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ExitTransitionInfo"));
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("FakeWriteDefaultClip"));

            }
            serializedObject.ApplyModifiedProperties();


        }


        // SceneビューのGizmo描画を抑制
        [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Pickable)]
        static void DrawGizmoForMyComponent(EmotePrefab component, GizmoType gizmoType)
        {
            // ここで何も描画しない、もしくはカスタムの描画ロジックを設定
        }
    }
}