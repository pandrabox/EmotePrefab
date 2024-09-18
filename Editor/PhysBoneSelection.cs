#if UNITY_EDITOR
using com.github.pandrabox.emoteprefab.runtime;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

namespace com.github.pandrabox.emoteprefab.editor
{
    [CustomPropertyDrawer(typeof(PhysBoneSelection))]
    public class PhysBoneSelectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var labelProperty = property.FindPropertyRelative("label");
            var allProperty = property.FindPropertyRelative("All");
            var physBonesProperty = property.FindPropertyRelative("PhysBones");

            string propertyName = property.propertyPath;
            int lastDotIndex = propertyName.LastIndexOf('.');
            if (lastDotIndex >= 0)
            {
                propertyName = propertyName.Substring(lastDotIndex + 1);
            }
            labelProperty.stringValue = propertyName;

            var labelStr = labelProperty.stringValue;
            var spAll = allProperty;
            var spPB = physBonesProperty;

            Rect fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            if (spAll.boolValue)
            {
                var allStr = labelStr.Contains("Phys") ? labelStr.Replace("Phys", "AllPhys") : $"{labelStr}All";
                EditorGUI.PropertyField(fieldRect, spAll, new GUIContent(allStr));
            }
            else
            {
                // ALL ボタンとリスト操作のUI
                EditorGUI.LabelField(fieldRect, labelStr);
                //fieldRect.y += EditorGUIUtility.singleLineHeight;

                fieldRect.width = position.width / 4;
                fieldRect.x += fieldRect.width;

                if (GUI.Button(fieldRect, "ALL"))
                {
                    spAll.boolValue = true;
                }

                fieldRect.x += fieldRect.width;

                if (GUI.Button(fieldRect, "-"))
                {
                    if (spPB.arraySize > 0)
                    {
                        spPB.arraySize--;
                    }
                }

                fieldRect.x += fieldRect.width;

                if (GUI.Button(fieldRect, "+"))
                {
                    spPB.arraySize++;
                }

                // VRCPhysBone リストの表示
                EditorGUI.indentLevel++;
                fieldRect.x = position.x;
                fieldRect.width = position.width;
                fieldRect.y += EditorGUIUtility.singleLineHeight;
                for (int i = 0; i < spPB.arraySize; i++)
                {
                    EditorGUI.PropertyField(fieldRect, spPB.GetArrayElementAtIndex(i), GUIContent.none);
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
