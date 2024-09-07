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
        public void Run(VRCAvatarDescriptor AvatarDescriptor)//,GameObject AvatarRootObject
        {
            this.AvatarDescriptor = AvatarDescriptor;
            ActionLayerReplace();
        }
        private void ActionLayerReplace()
        {
            string ActionAnimatorPath = $@"Packages\com.github.pandrabox.emoteprefab\Assets\BearsDen\CustomAnimatorControllers\Action.controller";
            var AssignController= AssetDatabase.LoadAssetAtPath<AnimatorController>(ActionAnimatorPath);
            if (AssignController == null) {
                throw new Exception("EmotePrefab ActionLayerReplace AssignController Not Found");
            }
            AvatarDescriptor.baseAnimationLayers[3].isDefault = false;
            AvatarDescriptor.baseAnimationLayers[3].animatorController = AssignController;
        }
    }
}
