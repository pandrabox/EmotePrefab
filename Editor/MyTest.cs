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

[assembly: ExportsPlugin(typeof(MyTestPass))]

namespace com.github.pandrabox.emoteprefab.runtime
{
    /// <summary>
    /// To call from Unity menu (Debug Only, Comment out upon release.)
    /// </summary>
    public class MyTestUnityMenu : MonoBehaviour
    {
        [MenuItem("PanDev/MyTest")]
        static void GenMyTest()
        {
            new MyTestMain().Run(Selection.activeGameObject);
        }
    }
    /// <summary>
    /// To call from NDMF
    /// </summary>
    public class MyTestPass : Plugin<MyTestPass>
    {
        protected override void Configure()
        {
            try
            {
                InPhase(BuildPhase.Transforming).BeforePlugin("nadena.dev.modular-avatar").Run("PanMyTest", ctx =>
                {
                    var TargetComponents = ctx.AvatarRootTransform.GetComponentsInChildren<MyTest>(false);
                    foreach (var T in TargetComponents)
                    {
                        new MyTestMain().Run(T.gameObject);
                    }
                });
            }
            catch (Exception e)
            {
                Debug.LogError($@"[Pan:MyTest]{e}");
            }
        }
    }
    /// <summary>
    /// Actual operation
    /// Retrieving ctx is not recommended to maintain debug availability.
    /// </summary>
    public class MyTestMain : MonoBehaviour
    {
        public void Run(GameObject Target)
        {
            Target.SetActive(false);
            Debug.LogWarning($@"MyTest:{Target.name}");
        }
    }
}
