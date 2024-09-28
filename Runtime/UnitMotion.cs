#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.UIElements;
using UnityEngine;
using static com.github.pandrabox.emoteprefab.runtime.TransitionInfo;
using static UnityEngine.GraphicsBuffer;

namespace com.github.pandrabox.emoteprefab.runtime
{

    public enum MotionType{
        OneShot,
        Loop,
        Hold,
        UnstoppableOneshot
    }

    [Serializable]
    public class UnitMotion
    {
        public MotionType MotionType;
        public UnitMotionClips Clip = new UnitMotionClips();
        public UnitMotionTransitionInfo TransitionInfo = new UnitMotionTransitionInfo();
        public int Mode=0;
    }

    [Serializable]
    public class UnitMotionClips
    {
        public AnimationClip Original;
        public AnimationClip Humanoid;
        public AnimationClip UnHumanoid;
        public AnimationClip FakeWD;
        public AnimationClip UnHumanoidR;
        public AnimationClip FakeWDR;
        public AnimationClip ShrinkPB;
        public AnimationClip ShrinkWD;
        public AnimationClip HumanoidH, HumanoidL;
        public BlendTree HumanoidBlendTree;
    }
    [Serializable]
    public class UnitMotionTransitionInfo
    {
        public TransitionInfo Start = new TransitionInfo(TransitionType.Start);
        public TransitionInfo AutoExit = new TransitionInfo(TransitionType.AutoExit);
        public TransitionInfo ManualExit = new TransitionInfo(TransitionType.ManualExit);
        public TransitionInfo Sit = new TransitionInfo(TransitionType.Sit);
        public TransitionInfo Quick = new TransitionInfo(TransitionType.Quick);
    }


    [CustomPropertyDrawer(typeof(UnitMotion))]
    public class UnitMotionDrawer : PropertyDrawer
    {
        public void MiniMotion(Rect area, float labelSize, SerializedProperty property, string name, string appearName = null)
        {
            if (appearName == null) appearName = name;
            var labelField = new Rect(area.x, area.y, labelSize, area.height);
            var objField = new Rect(area.x + labelSize, area.y, area.width - labelSize, area.height);
            EditorGUI.LabelField(labelField, appearName);
            EditorGUI.PropertyField(objField, property.FindPropertyRelative("Clip").FindPropertyRelative(name), new GUIContent());
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var originalClipID = property.FindPropertyRelative("Clip").FindPropertyRelative("Original").objectReferenceInstanceIDValue; //UI開始時のOriginalClipIDを取得(最後に使う)
            var Mode = property.FindPropertyRelative("Mode").intValue;
            EditorGUI.BeginProperty(position, label, property);
            float positiony = position.y;
            float positionx;
            Rect fieldRect = new Rect(position.x, positiony, position.width, EditorGUIUtility.singleLineHeight);
            float labelsize = 100;
            float checkSize = EditorGUIUtility.singleLineHeight * 1.5f;
            float unitsize = 30;
            var transProp = property.FindPropertyRelative("TransitionInfo").FindPropertyRelative("AutoExit");


            if (Mode==2)
            {
                fieldRect = new Rect(position.x, positiony, position.width / 2, EditorGUIUtility.singleLineHeight);
                MiniMotion(fieldRect, labelsize, property, "Original");
                fieldRect = new Rect(position.x + position.width / 2, positiony, position.width / 2, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("MotionType"));
                /*
                positiony += EditorGUIUtility.singleLineHeight;
                fieldRect = new Rect(position.x, positiony, position.width / 2, EditorGUIUtility.singleLineHeight);
                MiniMotion(fieldRect, labelsize, property, "Humanoid");
                positiony += EditorGUIUtility.singleLineHeight;
                fieldRect = new Rect(position.x, positiony, position.width / 2, EditorGUIUtility.singleLineHeight);
                MiniMotion(fieldRect, labelsize, property, "UnHumanoid");
                fieldRect = new Rect(position.x + position.width / 2, positiony, position.width / 2, EditorGUIUtility.singleLineHeight);
                MiniMotion(fieldRect, labelsize, property, "FakeWD");
                positiony += EditorGUIUtility.singleLineHeight;
                fieldRect = new Rect(position.x, positiony, position.width / 2, EditorGUIUtility.singleLineHeight);
                MiniMotion(fieldRect, labelsize, property, "UnHumanoidR");
                fieldRect = new Rect(position.x + position.width / 2, positiony, position.width / 2, EditorGUIUtility.singleLineHeight);
                MiniMotion(fieldRect, labelsize, property, "FakeWDR");
                positiony += EditorGUIUtility.singleLineHeight;
                fieldRect = new Rect(position.x, positiony, position.width / 2, EditorGUIUtility.singleLineHeight);
                MiniMotion(fieldRect, labelsize, property, "ShrinkPB");
                fieldRect = new Rect(position.x + position.width / 2, positiony, position.width / 2, EditorGUIUtility.singleLineHeight);
                MiniMotion(fieldRect, labelsize, property, "ShrinkWD");
                */

                positiony += EditorGUIUtility.singleLineHeight;
                float labelsize2 = ((position.x + position.width / 2) - checkSize - unitsize) / 2;
                float[] xSize = new float[] { position.x, labelsize, checkSize, (position.x + position.width / 2) - (position.x + labelsize + checkSize), checkSize, labelsize2, unitsize, labelsize2 };
                positionx = 0 + xSize[0] + xSize[1];
                fieldRect = new Rect(positionx, positiony, xSize[2] + xSize[3], EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(fieldRect, "ExitTime");
                fieldRect = new Rect(positionx += (xSize[2] + xSize[3]), positiony, xSize[4] + xSize[5] + xSize[6], EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(fieldRect, "Duration");
                fieldRect = new Rect(positionx += (xSize[4] + xSize[5]) + unitsize, positiony, xSize[7], EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(fieldRect, "Offset");


                foreach (string tag in new string[] { "Start", "AutoExit", "ManualExit", "Sit", "Quick" })
                {
                    transProp = property.FindPropertyRelative("TransitionInfo").FindPropertyRelative(tag);
                    positiony += EditorGUIUtility.singleLineHeight;
                    positionx = 0;
                    fieldRect = new Rect(positionx += xSize[0], positiony, xSize[1], EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(fieldRect, tag);
                    fieldRect = new Rect(positionx += xSize[1], positiony, xSize[2], EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(fieldRect, transProp.FindPropertyRelative("HasExitTime"), new GUIContent());
                    fieldRect = new Rect(positionx += xSize[2], positiony, xSize[3], EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(fieldRect, transProp.FindPropertyRelative("ExitTime"), new GUIContent());
                    fieldRect = new Rect(positionx += xSize[3], positiony, xSize[4], EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(fieldRect, transProp.FindPropertyRelative("HasFixedDuration"), new GUIContent());
                    fieldRect = new Rect(positionx += xSize[4], positiony, xSize[5], EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(fieldRect, transProp.FindPropertyRelative("Duration"), new GUIContent());
                    fieldRect = new Rect(positionx += xSize[5], positiony, xSize[6], EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(fieldRect, (transProp.FindPropertyRelative("HasFixedDuration").boolValue ? "s" : "%"));
                    fieldRect = new Rect(positionx += xSize[6], positiony, xSize[7], EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(fieldRect, transProp.FindPropertyRelative("Offset"), new GUIContent());
                }
            }
            else
            {
                var row = new Rect(position.x, positiony, position.width, EditorGUIUtility.singleLineHeight);
                var left = new Rect(position.x, row.y, row.width / 2, row.height);
                var leftlabel = new Rect(left.x, row.y, labelsize, row.height);
                var leftval = new Rect(leftlabel.x + labelsize, row.y, left.width - labelsize, row.height);
                EditorGUI.LabelField(leftlabel, "Motion");
                EditorGUI.PropertyField(leftval, property.FindPropertyRelative("Clip").FindPropertyRelative("Original"), new GUIContent());

                leftlabel.y += row.height;
                leftval.y += row.height;
                EditorGUI.LabelField(leftlabel, "MotionType");
                EditorGUI.PropertyField(leftval, property.FindPropertyRelative("MotionType"), new GUIContent());

                var right = new Rect(row.x + row.width / 2, row.y, row.width / 2, row.height);
                var rightlabel = new Rect(right.x, row.y, labelsize, row.height);
                var rightcheck = new Rect(right.x + labelsize, row.y, checkSize, row.height);
                var rightval = new Rect(rightcheck.x + checkSize, row.y, right.width - labelsize - checkSize - unitsize, row.height);
                var rightunit = new Rect(rightval.x + rightval.width, row.y, unitsize, row.height);
                EditorGUI.LabelField(right, "ExitTime");
                EditorGUI.PropertyField(rightcheck, transProp.FindPropertyRelative("HasExitTime"), new GUIContent());
                EditorGUI.PropertyField(rightval, transProp.FindPropertyRelative("ExitTime"), new GUIContent());

                right.y += row.height;
                rightcheck.y += row.height;
                rightval.y += row.height;
                rightunit.y += row.height;
                EditorGUI.LabelField(right, "Duration");
                EditorGUI.PropertyField(rightcheck, transProp.FindPropertyRelative("HasFixedDuration"), new GUIContent());
                EditorGUI.PropertyField(rightval, transProp.FindPropertyRelative("Duration"), new GUIContent());
                EditorGUI.LabelField(rightunit, (transProp.FindPropertyRelative("HasFixedDuration").boolValue ? "s" : "%"));
            }

            if (originalClipID != property.FindPropertyRelative("Clip").FindPropertyRelative("Original").objectReferenceInstanceIDValue) //UI開始時とOriginalClipIDが違ったらType更新
            {
                var originalClip = (AnimationClip)property.FindPropertyRelative("Clip").FindPropertyRelative("Original").objectReferenceValue;
                if (originalClip.isLooping && originalClip.length > 2f / 60)
                {
                    property.FindPropertyRelative("MotionType").enumValueIndex = (int)MotionType.Loop;
                }
                else
                {
                    property.FindPropertyRelative("MotionType").enumValueIndex = (int)MotionType.Hold;
                }
            };
            EditorGUI.EndProperty();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * (property.FindPropertyRelative("Mode").intValue==2 ? 7.3f : 2);
        }
    }
}
#endif