// <copyright file="ExpressionCreater.cs"></copyright>

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
            if (!EmoteManager.HasItem)
            {
                return;
            }

            _emoteObjRoot = new GameObject("Emote");
            _emoteObjRoot.transform.SetParent(Avatar.RootTransform);

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

            for (int i = 0; i < EmoteManager.Length; i++)
            {
                if (EmoteManager.IsEmote(i))
                {
                    CreateUnitMenu(i);
                }
            }
        }

        private void CreateUnitMenu(int eI)
        {
            var obj = new GameObject(EmoteManager.EmoteName(eI));
            obj.transform.SetParent(_emoteObjRoot.transform);
            var unitMenu = obj.AddComponent<ModularAvatarMenuItem>();
            unitMenu.Control.parameter = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.Parameter() { name = "VRCEmote" };
            unitMenu.Control.value = EmoteManager.ID(eI);
            //if (EmoteManager.IsOneShot(eI))
            //{
            //    unitMenu.Control.type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.Button;
            //}
            //else
            //{
                unitMenu.Control.type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.Toggle;
            //}
        }
    }
}


/* For Reviwer
 * Project policy : To set WriteDefault to OFF for all AnimatorStates.
 * Please review this code for each element and identify areas for improvement, providing a final score out of 100.
 * Please review this code in Japanese.
 */