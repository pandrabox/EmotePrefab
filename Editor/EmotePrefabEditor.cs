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
        private EmotePrefab _chainToLog;
        private EmotePrefab _nowInstance;

        void OnEnable()
        {
            _nowInstance = (EmotePrefab)target;
            RenewChain(true);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            _nowInstance.Motion = (AnimationClip)EditorGUILayout.ObjectField("Motion", _nowInstance.Motion, typeof(AnimationClip), false);
            _nowInstance.Name = EditorGUILayout.TextField("Name", _nowInstance.Name);
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

                var spUseCustomExitTransition = serializedObject.FindProperty("UseCustomExitTransition");
                EditorGUILayout.PropertyField(spUseCustomExitTransition);
                if (spUseCustomExitTransition.boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ExitTransitionInfo"));
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("FakeWriteDefaultClip"));


                /* UIとしては動作するが機能未実装
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ChainTo"));
                RenewChain();

                GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ChainFrom"));
                GUI.enabled = true;
                */





            }
            serializedObject.ApplyModifiedProperties();


        }

        private void RenewChain(bool initial = false)
        {
            if (initial)
            {
                if(_nowInstance.ChainTo==null)
                {
                    _nowInstance.ChainTo = null;  // "null"をnullに変換
                }
                if (_nowInstance.ChainFrom == null)
                {
                    _nowInstance.ChainFrom = null;
                }
                _chainToLog = _nowInstance.ChainTo;
                return;
            }
            if (_nowInstance.ChainTo != _chainToLog)
            {
                bool doChange = true;
                WriteWarning("ChangeChain", $@"{_nowInstance?.name} : {_chainToLog?.name} >> {_nowInstance.ChainTo?.name}");
                if (_nowInstance.ChainTo?.ChainFrom != null)
                {
                    doChange = EditorUtility.DisplayDialog(
                        "Emote Chain", // ダイアログタイトル
                        $@"{_nowInstance.ChainTo.Name}は既に{_nowInstance.ChainTo.ChainFrom.Name}からチェインされています。実行するとそのチェインが解除されます。実行しますか？",
                        "Yes", // Yesボタンのラベル
                        "No" // Noボタンのラベル
                    );
                    if (doChange)
                    {
                        _nowInstance.ChainTo.ChainFrom.ChainTo = null;
                        _nowInstance.ChainTo.ChainFrom= null;
                    }
                }
                if (doChange)
                {
                    if (_nowInstance.ChainTo != null)
                    {
                        _nowInstance.ChainTo.ChainFrom = _nowInstance;
                    }
                    RenewChain(true);
                }
                else
                {
                    _nowInstance.ChainTo = _chainToLog;
                }
            }
        }

    }
}