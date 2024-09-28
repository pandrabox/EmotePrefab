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

public class EmotePrefabBulkGeneration : EditorWindow
{
    private DefaultAsset targetFolder;
    private Vector2 scrollPosition;
    private string[] animFiles;

    public static void ShowWindow()
    {
        // ウィンドウを表示
        GetWindow<EmotePrefabBulkGeneration>("EmotePrefabBulkGeneration");
    }

    // エディタウィンドウの内容を描画
    private void OnGUI()
    {
        GUILayout.Label("一括生成", EditorStyles.boldLabel);
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