#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using VRC.SDK3.Dynamics.PhysBone.Components;
using System.Linq;
using VRC.SDK3.Avatars.Components;
using static com.github.pandrabox.emoteprefab.runtime.Generic;
using UnityEngine.UIElements;
using System.IO;

namespace com.github.pandrabox.emoteprefab.runtime
{
    public class EmotePrefabEditorLayout : Editor
    {
        public static Texture2D _emotePrefabIcoAndLogo;

        protected void DrawIcoAndLogo()
        {
            if (_emotePrefabIcoAndLogo == null) _emotePrefabIcoAndLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(Config.LogoAndTitleIcon);
            if (_emotePrefabIcoAndLogo != null)
            {
                float iconWidth = 186;
                float iconHeight = 40;

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(_emotePrefabIcoAndLogo, GUILayout.Width(iconWidth), GUILayout.Height(iconHeight));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(4);
            }
        }

        protected static void Title(string t)
        {
            GUILayout.BeginHorizontal();

            var lineRect = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            int leftBorderSize = 5;
            var leftRect = new Rect(lineRect.x, lineRect.y, leftBorderSize, lineRect.height);
            var rightRect = new Rect(lineRect.x + leftBorderSize, lineRect.y, lineRect.width - leftBorderSize, lineRect.height);
            Color leftColor = new Color32(0xF4, 0xAD, 0x39, 0xFF);
            Color rightColor = new Color32(0x39, 0xA7, 0xF4, 0xFF);

            EditorGUI.DrawRect(leftRect, leftColor);
            EditorGUI.DrawRect(rightRect, rightColor);

            var textStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(5, 0, 0, 0),
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.black },
            };

            GUI.Label(rightRect, t, textStyle);

            GUILayout.EndHorizontal();
        }
    }
}
#endif