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
    public class GameObjectSelection
    {
        public List<GameObject> GameObjects = new List<GameObject>();
        public List<string> paths = new List<string>();

        public void RestoreGameObjects(Transform Root)
        {
            for (int i = 0; i < Mathf.Min(paths.Count, GameObjects.Count); i++)
            {
                if (GameObjects[i] == null)
                {
                    GameObjects[i] = Root.Find(paths[i])?.GetComponent<GameObject>();
                }
            }
        }
    }


    [CustomPropertyDrawer(typeof(GameObjectSelection))]
    public class GameObjectSelectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);


            var gameObjectsProperty = property.FindPropertyRelative("GameObjects");
            var pathsProperty = property.FindPropertyRelative("paths");
            if (pathsProperty.arraySize != gameObjectsProperty.arraySize)
            {
                pathsProperty.arraySize = gameObjectsProperty.arraySize;
            }

            var fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            // リスト操作のUI
            EditorGUI.LabelField(fieldRect, label.text);

            fieldRect.width = position.width / 4;
            fieldRect.x += fieldRect.width;

            fieldRect.x += fieldRect.width;

            if (GUI.Button(fieldRect, "-"))
            {
                if (gameObjectsProperty.arraySize > 0)
                {
                    gameObjectsProperty.arraySize--;
                    pathsProperty.arraySize = gameObjectsProperty.arraySize;
                }
            }

            fieldRect.x += fieldRect.width;

            if (GUI.Button(fieldRect, "+"))
            {
                gameObjectsProperty.arraySize++;
                pathsProperty.arraySize = gameObjectsProperty.arraySize;
            }

            // GameObject リストの表示
            EditorGUI.indentLevel++;
            fieldRect.x = position.x;
            fieldRect.width = position.width;
            fieldRect.y += EditorGUIUtility.singleLineHeight;
            float xButtonSize = 20f;
            for (int i = 0; i < gameObjectsProperty.arraySize; i++)
            {
                var leftrect = new Rect(fieldRect.x, fieldRect.y, fieldRect.width - xButtonSize, fieldRect.height);
                var rightrect = new Rect(fieldRect.x + leftrect.width, fieldRect.y, xButtonSize, fieldRect.height);
                var gameObjectProperty = gameObjectsProperty.GetArrayElementAtIndex(i);
                var cashProperty = pathsProperty.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(leftrect, gameObjectProperty, GUIContent.none);

                if (GUI.Button(rightrect, "x"))
                {
                    cashProperty.stringValue = string.Empty;
                    gameObjectProperty.objectReferenceValue = null;
                }

                cashProperty.stringValue = GameObjectPath((GameObject)gameObjectProperty.objectReferenceValue) ?? cashProperty.stringValue;
                fieldRect.y += EditorGUIUtility.singleLineHeight;
            }
            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var gameObjectsProperty = property.FindPropertyRelative("GameObjects");
            int extraLines = gameObjectsProperty.arraySize + 1;
            return EditorGUIUtility.singleLineHeight * extraLines;
        }

        private string GameObjectPath(GameObject gameObject)
        {
            if (gameObject == null) return null;
            var descriptor = FindComponentFromParent<VRCAvatarDescriptor>(gameObject);
            if (descriptor == null) return null;
            return FindPathRecursive(descriptor.transform, gameObject.transform);
        }
    }
}
#endif
