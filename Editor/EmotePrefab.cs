using System;
using System.Linq;
using System.Collections.Generic;
using nadena.dev.ndmf;
using nadena.dev.modular_avatar.core;
using nadena.dev.modular_avatar.core.editor;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.Components;
using com.github.pandrabox.emoteprefab.runtime;
using static com.github.pandrabox.emoteprefab.runtime.Generic;
using com.github.pandrabox.emoteprefab.editor;
using static UnityEditor.ShaderData;
using System.IO;

[assembly: ExportsPlugin(typeof(EmotePrefabPass))]

namespace com.github.pandrabox.emoteprefab.editor
{
    /// <summary>
    /// To call from Unity menu (Debug Only, Comment out upon release.)
    /// </summary>
    public class EmotePrefabUnityMenu : MonoBehaviour
    {
        [MenuItem("PanDev/EmotePrefab")]
        static void GenEmotePrefab()
        {
            var Target = Selection.activeGameObject;
            var AvatarDescriptor = FindComponentFromParent<VRCAvatarDescriptor>(Target);
            new EmotePrefabMain().Run(AvatarDescriptor);
        }
    }
    /// <summary>
    /// To call from NDMF
    /// </summary>
    public class EmotePrefabPass : Plugin<EmotePrefabPass>
    {
        protected override void Configure()
        {
            //try
            //{
            InPhase(BuildPhase.Transforming).BeforePlugin("nadena.dev.modular-avatar").Run("PanEmotePrefab", ctx =>
            {
                var TargetComponents = ctx.AvatarRootTransform.GetComponentsInChildren<EmotePrefab>(false);
                foreach (var T in TargetComponents)
                {
                    new EmotePrefabMain().Run(ctx.AvatarDescriptor);
                    return;
                }
            });
            //}
            //catch (Exception e)
            //{
            //    Debug.LogError($@"[Pan:EmotePrefab]{e}");
            //}
        }
    }
    /// <summary>
    /// Actual operation
    /// </summary>
    public class EmotePrefabMain : MonoBehaviour
    {
        VRCAvatarDescriptor AvatarDescriptor;
        AnimatorController ActionController;
        ChildAnimatorStateMachine[] TopLevelStateMachines;
        public void Run(VRCAvatarDescriptor AvatarDescriptor)//,GameObject AvatarRootObject
        {
            if(AvatarDescriptor == null)
            {
                Debug.LogError("[EmotePrefab] AvatarDescriptor==null");
                return;
            }
            this.AvatarDescriptor = AvatarDescriptor;
            CreateWorkFolder();
            ActionLayerReplace();
            AddEmotes();
        }
        private void CreateWorkFolder()
        {
            DirectoryInfo di = new DirectoryInfo(CONST.WORKDIR);
            if (!di.Exists)
            {
                di.Create();
            }
        }
        private void ActionLayerReplace()
        {
            string WorkActionAnimatorPath = $@"{CONST.WORKDIR}Action.controller";
            AssetDatabase.CopyAsset(CONST.OrgActionAnimatorPath, WorkActionAnimatorPath);

            var AssignController = AssetDatabase.LoadAssetAtPath<AnimatorController>(WorkActionAnimatorPath);
            if (AssignController == null)
            {
                throw new Exception("EmotePrefab ActionLayerReplace AssignController Not Found");
            }
            AvatarDescriptor.baseAnimationLayers[3].isDefault = false;
            AvatarDescriptor.baseAnimationLayers[3].animatorController = AssignController;
        }
        private void AddEmotes()
        {

            var PAL = new PanActionLayer(AvatarDescriptor);
            var TargetComponents = AvatarDescriptor.transform.GetComponentsInChildren<EmotePrefab>(false);　//false=非アクティブは無視ということ
            var SortedComponents = TargetComponents.OrderBy(component => component.Name).ToArray();
            for (int i = 0; i < SortedComponents.Length; i++)
            {
                PAL.AddEmote(i + 1, SortedComponents[i]);
            }
            CreateMAMenu(SortedComponents);
        }
        private void CreateMAMenu(EmotePrefab[] SortedEP)
        {
            //Objectの作成
            var EPMenuObj = new GameObject("Emote");

            //Parameter定義とInstaller
            var EPParams = EPMenuObj.AddComponent<ModularAvatarParameters>();
            EPParams.parameters.Add(new ParameterConfig()
            {
                nameOrPrefix = "VRCEmote",
                syncType = ParameterSyncType.Int,
            });
            EPMenuObj.AddComponent<ModularAvatarMenuInstaller>();

            //Menuの親生成
            EPMenuObj.transform.SetParent(AvatarDescriptor.transform);
            var EPMenu = EPMenuObj.AddComponent<ModularAvatarMenuItem>();
            EPMenu.Control.type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.SubMenu;
            EPMenu.MenuSource = SubmenuSource.Children;

            //Menuの実体生成
            for (int i = 0;i < SortedEP.Length;i++)
            {
                int ID = i+1;
                var CurrentEP = SortedEP[i];
                var UnitMenuObj = new GameObject(CurrentEP.Name);
                UnitMenuObj.transform.SetParent(EPMenuObj.transform);
                var UnitMenu = UnitMenuObj.AddComponent<ModularAvatarMenuItem>();
                if (CurrentEP.IsOneShot)
                {
                    UnitMenu.Control.type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.Button;
                }
                else
                {
                    UnitMenu.Control.type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.Toggle;
                }
                UnitMenu.Control.parameter = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.Parameter() { name = "VRCEmote" };
                UnitMenu.Control.value = ID;
            }
        }
    }
}

