using System;
using System.Collections.Generic;
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
using static com.github.pandrabox.emoteprefab.runtime.Generic;
using static com.github.pandrabox.emoteprefab.editor.EmoteManager;

#pragma warning disable SA1600 // Elements should be documented
namespace com.github.pandrabox.emoteprefab.editor
{
    /// <summary>
    /// ExpressionMenuを生成するクラス
    /// </summary>
    public class ExpressionCreater
    {
        private GameObject _emoteObjRoot;

        public ExpressionCreater()
        {
            if (!HasTask)
            {
                return;
            }

            _emoteObjRoot = new GameObject("Emote");
            _emoteObjRoot.transform.SetParent(EmotePrefabRootTransform);

            _emoteObjRoot.AddComponent<ModularAvatarMenuInstaller>();

            var param = _emoteObjRoot.AddComponent<ModularAvatarParameters>();
            param.parameters.Add(new ParameterConfig()
            {
                nameOrPrefix = "VRCEmote",
                syncType = ParameterSyncType.Int,
            });

            var menu = _emoteObjRoot.AddComponent<ModularAvatarMenuItem>();
            menu.Control.type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.SubMenu;
            menu.MenuSource = SubmenuSource.Children;
            menu.Control.icon = AssetDatabase.LoadAssetAtPath<Texture2D>(Config.EmotePrefabIcon);

            CreateFoldedMenu(Descriptor.transform, menu.transform);
        }


        private void CreateFoldedMenu(Transform currentTrans, Transform currentFolder)
        {

            var emoteFolder = currentTrans.GetComponent<EmoteFolder>();
            if (emoteFolder!=null)
            {
                var subFolderObj = new GameObject(emoteFolder.FolderName);
                subFolderObj.transform.SetParent(currentFolder);
                currentFolder=subFolderObj.transform;

                var folderMenu = subFolderObj.AddComponent<ModularAvatarMenuItem>();
                folderMenu.Control.type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.SubMenu;
                folderMenu.MenuSource = SubmenuSource.Children;
                folderMenu.Control.icon = FolderIcon(emoteFolder);
            }
            var emotePrefab = currentTrans.GetComponent<EmotePrefab>();
            if (emotePrefab!=null && emotePrefab.IsEmote && emotePrefab.gameObject.activeInHierarchy)
            {
                CreateUnitMenu(emotePrefab, currentFolder);
            }

            // 次の探索
            var childCount = currentTrans.childCount;
            for (var i = 0; i < childCount; i++)
            {
                CreateFoldedMenu(currentTrans.GetChild(i), currentFolder);
            }
        }

        private void CreateUnitMenu(EmotePrefab emotePrefab, Transform folder)
        {
            var obj = new GameObject(emotePrefab.Name);
            obj.transform.SetParent(folder);
            var unitMenu = obj.AddComponent<ModularAvatarMenuItem>();
            unitMenu.Control.parameter = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.Parameter() { name = "VRCEmote" };
            unitMenu.Control.value = emotePrefab.ID;
            unitMenu.Control.type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.Toggle;
            unitMenu.Control.icon = Icon(emotePrefab);
        }

        public static Texture2D FolderIcon(EmoteFolder folder)
        {
            if(folder.Icon != null)
            {
                return folder.Icon;
            }
            else
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>(Config.FolderIcon);
            }
        }

        public static Texture2D Icon(EmotePrefab emotePrefab)
        {
            if (emotePrefab.Icon != null)
            {
                return emotePrefab.Icon;
            }
            else if (emotePrefab.RootMotion.MotionType == MotionType.Loop)
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>(Config.LoopIcon);
            }
            else if (emotePrefab.RootMotion.MotionType == MotionType.Hold)
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>(Config.HoldIcon);
            }
            else if (emotePrefab.RootMotion.MotionType == MotionType.UnstoppableOneshot)
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>(Config.UnstoppableIcon);
            }
            else
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>(Config.OneShotIcon);
            }
        }
    }
}
