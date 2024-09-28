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
    public class IntSelection
    {
        public List<int> Ints = new List<int>();
        public IntSelection(params int[] ints)
        {
            Ints.AddRange(ints);
        }
    }


    [CustomPropertyDrawer(typeof(IntSelection))]
    public class IntSelectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var intsProperty = property.FindPropertyRelative("Ints");

            Rect fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            fieldRect.width = position.width / 2;
            EditorGUI.LabelField(fieldRect, label.text);

            fieldRect.width = position.width / 4;
            fieldRect.x += fieldRect.width*2;

            if (GUI.Button(fieldRect, "-"))
            {
                if (intsProperty.arraySize > 0)
                {
                    intsProperty.arraySize--;
                }
            }

            fieldRect.x += fieldRect.width;

            if (GUI.Button(fieldRect, "+"))
            {
                intsProperty.arraySize++;
            }

            // int リストの表示
            EditorGUI.indentLevel++;
            fieldRect.x = position.x;
            fieldRect.width = position.width;
            fieldRect.y += EditorGUIUtility.singleLineHeight;
            for (int i = 0; i < intsProperty.arraySize; i++)
            {
                var intProperty = intsProperty.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(fieldRect, intProperty, GUIContent.none);
                fieldRect.y += EditorGUIUtility.singleLineHeight;
            }
            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var intsProperty = property.FindPropertyRelative("Ints");
            int extraLines = intsProperty.arraySize + 1;
            return EditorGUIUtility.singleLineHeight * extraLines;
        }
    }
}
#endif