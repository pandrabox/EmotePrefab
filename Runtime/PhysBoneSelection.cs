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
    }


    [CustomPropertyDrawer(typeof(PhysBoneSelection))]
    public class PhysBoneSelectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var allProperty = property.FindPropertyRelative("All");
            var physBonesProperty = property.FindPropertyRelative("PhysBones");

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
                    }
                }

                fieldRect.x += fieldRect.width;

                if (GUI.Button(fieldRect, "+"))
                {
                    physBonesProperty.arraySize++;
                }

                // VRCPhysBone リストの表示
                EditorGUI.indentLevel++;
                fieldRect.x = position.x;
                fieldRect.width = position.width;
                fieldRect.y += EditorGUIUtility.singleLineHeight;
                for (int i = 0; i < physBonesProperty.arraySize; i++)
                {
                    EditorGUI.PropertyField(fieldRect, physBonesProperty.GetArrayElementAtIndex(i), GUIContent.none);
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
    }
}
#endif