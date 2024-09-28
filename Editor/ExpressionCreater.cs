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

            var emoteMenuInfo = _emoteObjRoot.AddComponent<EmoteMenuInfo>();
            emoteMenuInfo.Sort = true;
            emoteMenuInfo.AutoFolderMode = EmoteMenuInfo.AutoFolderModeType.sub;

            var menu = _emoteObjRoot.AddComponent<ModularAvatarMenuItem>();
            menu.Control.type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.SubMenu;
            menu.MenuSource = SubmenuSource.Children;
            menu.Control.icon = AssetDatabase.LoadAssetAtPath<Texture2D>(Config.EmotePrefabIcon);

            _emoteObjRoot.AddComponent<ModularAvatarMenuInstaller>();

            CreateFoldedMenu(Descriptor.transform, menu.transform);
            ResolveEmoteMenuInfo();
            AddHeightControl();
            AddFootLockControl();
        }

        private void AddHeightControl()
        {
            var obj = new GameObject("Height");
            obj.transform.SetParent(_emoteObjRoot.transform);
            obj.transform.SetSiblingIndex(0);
            var menu=obj.AddComponent<ModularAvatarMenuItem> ();
            menu.Control.type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.RadialPuppet;
            menu.Control.subParameters = new[] {
                new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.Parameter()
                {
                    name = "EmotePrefab/Height"
                }
            };
            menu.Control.icon = AssetDatabase.LoadAssetAtPath<Texture2D>(Config.HeightIcon);
        }

        private void AddFootLockControl()
        {
            var obj = new GameObject("FootLock");
            obj.transform.SetParent(_emoteObjRoot.transform);
            obj.transform.SetSiblingIndex(0);
            var menu = obj.AddComponent<ModularAvatarMenuItem>();
            menu.Control.type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.Toggle;
            menu.Control.parameter =
                new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.Parameter()
                {
                    name = "EmotePrefab/FootLock"
                };
            menu.Control.icon = AssetDatabase.LoadAssetAtPath<Texture2D>(Config.FootLockIcon);
        }

        private Transform CreateSubFolder(Transform currentFolder, EmoteFolder emoteFolder)
        {
            var subFolderObj = new GameObject(emoteFolder.FolderName);
            subFolderObj.transform.SetParent(currentFolder);

            var emoteMenuInfo = subFolderObj.AddComponent<EmoteMenuInfo>();
            emoteMenuInfo.Sort = emoteFolder.Sort;
            emoteMenuInfo.AutoFolderMode = emoteFolder.AutoFolderMode;

            var folderMenu = subFolderObj.AddComponent<ModularAvatarMenuItem>();
            folderMenu.Control.type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.SubMenu;
            folderMenu.MenuSource = SubmenuSource.Children;
            folderMenu.Control.icon = FolderIcon(emoteFolder);

            return subFolderObj.transform;
        }


        private void CreateFoldedMenu(Transform currentTrans, Transform currentFolder)
        {

            var emoteFolder = currentTrans.GetComponent<EmoteFolder>();
            if (emoteFolder!=null && emoteFolder.gameObject.activeInHierarchy)
            {
                currentFolder = CreateSubFolder(currentFolder, emoteFolder);
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
            if(folder != null && folder.Icon != null)
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


        private void ResolveEmoteMenuInfo()
        {
            EmoteMenuInfo[] folders = _emoteObjRoot.transform.GetComponentsInChildren<EmoteMenuInfo>();
            foreach (var folder in folders)
            {
                if (folder.Sort)
                {
                    SortChildren(folder.transform);
                    PackFolder(folder);
                }
            }
        }

        private void PackFolder(EmoteMenuInfo parent)
        {
            if (parent.AutoFolderMode == EmoteMenuInfo.AutoFolderModeType.none) return;
            if (parent.transform.childCount < 16) return;
            
            int itemIndex = 0;
            GameObject currentPack=null;
            int itemNum = parent.transform.childCount;
            string startItem=null, exitItem, suffix=null;
            for (int m = 0; m < itemNum; m++)
            {
                if (itemIndex == 0)
                {
                    currentPack = new GameObject();
                    if (parent.AutoFolderMode == EmoteMenuInfo.AutoFolderModeType.sub)
                    {
                        currentPack.transform.SetParent(parent.transform, false);
                    }
                    else
                    {
                        currentPack.transform.SetParent(parent.transform.parent, false);
                        suffix = $@"{parent.name} ";
                    }
                    startItem = getItemName(parent);

                    var folderMenu = currentPack.AddComponent<ModularAvatarMenuItem>();
                    folderMenu.Control.type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.SubMenu;
                    folderMenu.MenuSource = SubmenuSource.Children;
                    folderMenu.Control.icon = parent.gameObject.GetComponent<ModularAvatarMenuItem>().Control.icon;
                }
                exitItem = getItemName(parent);
                parent.transform.GetChild(0).SetParent(currentPack.transform);
                itemIndex = itemIndex++ <7 ? itemIndex : 0;
                if (itemIndex == 0 || m+1==itemNum)
                {
                    if (startItem != exitItem)
                    {
                        currentPack.name = $@"{suffix}{startItem}～{exitItem}";
                    }
                    else
                    {
                        currentPack.name = $@"{suffix}{startItem}";
                    }
                }
            }
            if (parent.AutoFolderMode == EmoteMenuInfo.AutoFolderModeType.parallel)
            {
                GameObject.DestroyImmediate(parent.gameObject);
            }
        }

        private string getItemName(EmoteMenuInfo em)
        {
            var nm = em?.transform?.GetChild(0)?.GetComponent<EmotePrefab>()?.Name;
            if (nm != null) { return nm; }
            return em?.transform?.GetChild(0)?.name;
        }

        private void SortChildren(Transform parent)
        {
            // Get the children and sort them by name
            var children = parent.Cast<Transform>().OrderBy(t => t.name).ToList();

            // Bring EmoteFolder objects to the front
            var emoteFolders = children.Where(t => t.GetComponent<EmoteMenuInfo>() != null).ToList();
            var otherObjects = children.Where(t => t.GetComponent<EmoteMenuInfo>() == null).ToList();

            // Clear the existing hierarchy
            foreach (Transform child in children)
            {
                child.SetParent(null);
            }

            // Reattach EmoteFolder objects first, followed by other objects
            foreach (var emoteFolder in emoteFolders)
            {
                emoteFolder.SetParent(parent);
            }

            foreach (var other in otherObjects)
            {
                other.SetParent(parent);
            }

            // Set the siblings order based on the list
            for (int i = 0; i < parent.childCount; i++)
            {
                parent.GetChild(i).SetSiblingIndex(i);
            }
        }
    }
}
