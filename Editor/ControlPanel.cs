#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using com.github.pandrabox.emoteprefab.editor;
using com.github.pandrabox.emoteprefab.runtime;
using nadena.dev.modular_avatar.core;
using nadena.dev.modular_avatar.core.editor;
using nadena.dev.ndmf;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;
using static com.github.pandrabox.emoteprefab.runtime.Generic;

namespace com.github.pandrabox.emoteprefab.editor
{
    public class PutControlPanel
    {
        /// <summary>
        /// To call from Unity menu
        /// </summary>
        [MenuItem("Pan/PutControlPanel")]
        public static void PutControlPanelMain()
        {
            var selectObj = Selection.gameObjects.FirstOrDefault();
            var descriptor = FindComponentFromParent<VRCAvatarDescriptor>(selectObj);
            if (descriptor == null)
            {
                EditorUtility.DisplayDialog("PutControlPanel", "この機能はHierarchyでControlPanelをアクティブにしたいアバターを選択してから使って下さい","OK");
                return;
            }
            var existController = descriptor.transform.Find(Config.ControlPanelObjName);
            if (existController != null)
            {
                if (existController.GetComponent<ControlPanel>()== null)
                {
                    existController.gameObject.AddComponent<ControlPanel>();
                }
                Selection.activeGameObject = existController.gameObject;
                return;
            }
            var obj = new GameObject(Config.ControlPanelObjName);
            obj.transform.SetParent(descriptor.transform);
            obj.AddComponent<ControlPanel>();
            Selection.activeGameObject = obj;
        }
    }
}

#endif

