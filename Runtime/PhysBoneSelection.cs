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
    public class PhysBoneSelection 
    {
        public bool All;
        public List<VRCPhysBone> PhysBones = new List<VRCPhysBone>();
        public List<string> paths = new List<string>();

        public void RestorePhysBones(Transform Root)
        {
            for(int i = 0; i < Mathf.Min(paths.Count, PhysBones.Count); i++) {
                if (PhysBones[i] == null)
                {
                    PhysBones[i] = Root.Find(paths[i])?.GetComponent<VRCPhysBone>();
                }
            }
        }
    }


    [CustomPropertyDrawer(typeof(PhysBoneSelection))]
    public class PhysBoneSelectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);


            var allProperty = property.FindPropertyRelative("All");
            var physBonesProperty = property.FindPropertyRelative("PhysBones");
            var pathsProperty = property.FindPropertyRelative("paths");
            if (pathsProperty.arraySize != physBonesProperty.arraySize)
            {
                pathsProperty.arraySize = physBonesProperty.arraySize;
            }

            Rect fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            if (allProperty.boolValue)
            {
                var allStr = label.text.Contains("Phys") ? label.text.Replace("Phys", "All Phys") : $"{label.text}All";
                EditorGUI.PropertyField(fieldRect, allProperty, new GUIContent(allStr));
            }
            else
            {
                // ALL ボタンとリスト操作のUI
                EditorGUI.LabelField(fieldRect, label.text);

                fieldRect.width = position.width / 4;
                fieldRect.x += fieldRect.width;

                if (GUI.Button(fieldRect, "ALL"))
                {
                    allProperty.boolValue = true;
                }

                fieldRect.x += fieldRect.width;

                if (GUI.Button(fieldRect, "-"))
                {
                    if (physBonesProperty.arraySize > 0)
                    {
                        physBonesProperty.arraySize--;
                        pathsProperty.arraySize = physBonesProperty.arraySize;
                    }
                }

                fieldRect.x += fieldRect.width;

                if (GUI.Button(fieldRect, "+"))
                {
                    physBonesProperty.arraySize++;
                    pathsProperty.arraySize = physBonesProperty.arraySize;
                }

                // VRCPhysBone リストの表示
                EditorGUI.indentLevel++;
                fieldRect.x = position.x;
                fieldRect.width = position.width;
                fieldRect.y += EditorGUIUtility.singleLineHeight;
                float xButtonSize = 20f;
                for (int i = 0; i < physBonesProperty.arraySize; i++)
                {
                    var leftrect = new Rect(fieldRect.x, fieldRect.y, fieldRect.width - xButtonSize, fieldRect.height);
                    var rightrect = new Rect(fieldRect.x + leftrect.width, fieldRect.y, xButtonSize, fieldRect.height);
                    var physBoneProperty = physBonesProperty.GetArrayElementAtIndex(i);
                    var cashProperty = pathsProperty.GetArrayElementAtIndex(i);
                    EditorGUI.PropertyField(leftrect, physBoneProperty, GUIContent.none);

                    if (GUI.Button(rightrect, "x"))
                    {
                        cashProperty.stringValue = string.Empty;
                        physBoneProperty.objectReferenceValue = null;
                    }

                    cashProperty.stringValue = PBPath((VRCPhysBone)physBoneProperty.objectReferenceValue) ?? cashProperty.stringValue;
                    fieldRect.y += EditorGUIUtility.singleLineHeight;
                }
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 高さの計算
            if (property.FindPropertyRelative("All").boolValue)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            else
            {
                var spPB = property.FindPropertyRelative("PhysBones");
                int extraLines = spPB.arraySize + 1;
                return EditorGUIUtility.singleLineHeight * extraLines;
            }
        }

        private string PBPath(VRCPhysBone physBone)
        {
            if (physBone == null) return null;
            var descriptor = FindComponentFromParent<VRCAvatarDescriptor>(physBone.gameObject);
            if (descriptor == null) return null;
            return FindPathRecursive(descriptor.transform, physBone.transform);
        }
    }
}
#endif