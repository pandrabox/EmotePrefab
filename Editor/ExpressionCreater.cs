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

            for (int i = 0; i < EmotePrefabs.Length; i++)
            {
                if (EmotePrefabs[i].IsEmote)
                {
                    CreateUnitMenu(i);
                }
            }
        }

        private void CreateUnitMenu(int eI)
        {
            var obj = new GameObject(EmotePrefabs[eI].Name);
            obj.transform.SetParent(_emoteObjRoot.transform);
            var unitMenu = obj.AddComponent<ModularAvatarMenuItem>();
            unitMenu.Control.parameter = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.Parameter() { name = "VRCEmote" };
            unitMenu.Control.value = eI+1;
            unitMenu.Control.type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.Toggle;
            unitMenu.Control.icon = Icon(eI);
        }


        public static Texture2D Icon(int n)
        {
            if (EmotePrefabs[n].Icon != null)
            {
                return EmotePrefabs[n].Icon;
            }
            else if (EmotePrefabs[n].RootMotion.MotionType == MotionType.Loop)
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>(Config.LoopIcon);
            }
            else if (EmotePrefabs[n].RootMotion.MotionType == MotionType.Hold)
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>(Config.HoldIcon);
            }
            else if (EmotePrefabs[n].RootMotion.MotionType == MotionType.UnstoppableOneshot)
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
