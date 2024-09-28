#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using com.github.pandrabox.emoteprefab.runtime;
using nadena.dev.ndmf;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;
using static com.github.pandrabox.emoteprefab.runtime.Generic;

public class BulkGeneration : EditorWindow
{
    private DefaultAsset targetFolder;
    private Vector2 scrollPosition;
    private string[] animFiles;

    public static void ShowWindow()
    {
        // ウィンドウを表示
        GetWindow<BulkGeneration>("BulkGeneration");
    }
    private void LegacySupporArea()
    {

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        var selectObj = Selection.gameObjects.FirstOrDefault();
        if (selectObj == null)
        {
            GUILayout.Label("この機能を使うためには、対象のアバターをHierarchyでクリックして下さい", labelStyle);
            return;
        }
        var descriptor = FindComponentFromParent<VRCAvatarDescriptor>(selectObj);
        if (descriptor == null)
        {
            GUILayout.Label("この機能を使うためには、対象のアバターをHierarchyでクリックして下さい", labelStyle);
            return;
        }
        GUILayout.Label($@"対象アバター：{descriptor.name}");
        if (descriptor.transform.Find(Config.LegacyObjName) != null)
        {
            GUILayout.Label("実行済み", labelStyle);
            return;
        }
        if (GUILayout.Button("実行"))
        {
            new LegacySupport(descriptor);
            var cp = descriptor.transform.Find(Config.ControlPanelObjName).GetComponent<ControlPanel>();
            cp.LegacySupport = false;
            EditorUtility.DisplayDialog("一括生成", $@"アバター直下に{Config.LegacyObjName}オブジェクトを生成し、重複防止のためControlPanelのLegacySupportをOFFにしました。", "OK");
        }
    }
    // エディタウィンドウの内容を描画
    private void OnGUI()
    {
        GUILayout.Label("一括生成(既存Actionから)", EditorStyles.boldLabel);
        LegacySupporArea();

        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));


        GUILayout.Label("一括生成(フォルダから)", EditorStyles.boldLabel);
        targetFolder = (DefaultAsset)EditorGUILayout.ObjectField("対象フォルダ", targetFolder, typeof(DefaultAsset), false);

        if (targetFolder == null) return;

        string folderPath = AssetDatabase.GetAssetPath(targetFolder);
        if (!Directory.Exists(folderPath))
        {
            EditorGUILayout.HelpBox("有効なフォルダを選択して下さい", MessageType.Warning);
            return;
        }

        animFiles = Directory.GetFiles(folderPath, "*.anim", SearchOption.AllDirectories);

        if (animFiles.Length == 0)
        {
            EditorGUILayout.HelpBox("animファイルの入っているフォルダを選択して下さい", MessageType.Warning);
            return;
        }

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150));
        foreach (var animFile in animFiles)
        {
            GUILayout.Label(animFile);
        }
        GUILayout.EndScrollView();

        if (GUILayout.Button("EmotePrefab化"))
        {
            var rootName = $@"Emotes_{Path.GetFileName(folderPath)}";
            var root = new GameObject(rootName).transform;
            foreach(var animFile in animFiles)
            {
                var obj = new GameObject();
                obj.transform.SetParent(root);
                var ep = obj.AddComponent<EmotePrefab>();
                ep.SetEasy(AssetDatabase.LoadAssetAtPath<AnimationClip>(animFile));
            }
            EditorUtility.DisplayDialog("一括生成", $@"Scene直下に{rootName}を生成しました。アバター下へ移動して下さい。","OK");
        }
    }
}

#endif