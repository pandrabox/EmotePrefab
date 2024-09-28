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

[assembly: ExportsPlugin(typeof(NBitIntPass))]

namespace com.github.pandrabox.emoteprefab.editor
{

    public class NBitIntPass : Plugin<NBitIntPass>
    {
        protected override void Configure()
        {
            InPhase(BuildPhase.Transforming).AfterPlugin("nadena.dev.modular-avatar").Run("com.github.pandrabox.emoteprefab", ctx =>
            {
                new NBitIntRun(ctx.AvatarRootTransform);
            });
        }
    }

    public class NBitIntRun
    {
        public NBitIntRun(Transform rootTransform)
        {
            var nBitInts = rootTransform.GetComponentsInChildren<NBitInt>(false).Where(c => c.ParameterName.Length > 0).ToArray();
            foreach (var nBitInt in nBitInts)
            {
                new NBitIntMain(nBitInt);
            }
        }
    }

    public class NBitIntMain
    {
        public string GeneratedNBitIntLayerPath;
        public AnimatorController NBitIntController;
        public string UniqueName;
        public int MaxValue;
        public int BitNum;
        public string ParameterName;
        public List<string> BitName = new List<string>();
        public AnimationClip Dummy2F;
        public TransitionInfo QuickTrans;
        public NBitIntMain(NBitInt nBitInt)
        {
            MaxValue = nBitInt.MaxValue+1;
            ParameterName = nBitInt.ParameterName;
            BitNum = (int)Math.Ceiling(Math.Log(MaxValue)/Math.Log(2));
            Dummy2F = DummyClip(2);
            QuickTrans = TransitionInfo.Quick;
            UniqueName = $@"NBitInt_{ParameterName}";
            GeneratedNBitIntLayerPath = $@"{Config.WorkDir}{UniqueName}.controller";
            if (!AssetDatabase.CopyAsset(Config.OriginalNBitIntLayer, GeneratedNBitIntLayerPath))
            {
                WriteWarning("WorkSpace", "NBitIntLayerの生成に失敗しました");
            }
            NBitIntController = AssetDatabase.LoadAssetAtPath<AnimatorController>(GeneratedNBitIntLayerPath);
            var layers = NBitIntController.layers;
            layers[0].name = UniqueName;
            NBitIntController.layers = layers;

            var obj = new GameObject(UniqueName);
            obj.transform.SetParent(nBitInt.transform);
            var mergeAnimator = obj.AddComponent<ModularAvatarMergeAnimator>();
            mergeAnimator.animator = NBitIntController;
            mergeAnimator.pathMode = MergeAnimatorPathMode.Absolute;
            mergeAnimator.matchAvatarWriteDefaults = true;

            ModularAvatarParameters maParams = obj.AddComponent<ModularAvatarParameters>();
            maParams.parameters.Add(new ParameterConfig()
            {
                nameOrPrefix = ParameterName,
                syncType = ParameterSyncType.Int,
                localOnly = true,
            });
            for (int n = 0; n < BitNum; n++)
            {
                BitName.Add($@"{ParameterName}_bit{n}");
                maParams.parameters.Add(new ParameterConfig()
                {
                    nameOrPrefix = BitName.Last(),
                    syncType = ParameterSyncType.Bool,
                });
                if (!NBitIntController.parameters.Any(p => p.name == BitName.Last()))
                {
                    NBitIntController.AddParameter(BitName.Last(), AnimatorControllerParameterType.Bool);
                }
            }
            new NBitADCWriterMain(this);
            new NBitDACWriterMain(this);
        }
    }

    public class NBitADCWriterMain : StateMachineWriter0
    {
        public NBitADCWriterMain(NBitIntMain config) : base(config.NBitIntController, config.UniqueName, "ADC", "Initial")
        {
            for (var n = 0; n < config.MaxValue; n++)
            {
                var state = CreateState($@"{n}", config.Dummy2F);
                SetTransition(_initialState, state, config.QuickTrans).AddCondition(AnimatorConditionMode.Equals, n, config.ParameterName);
                SetTransition(state, _exitState, config.QuickTrans).AddCondition(AnimatorConditionMode.NotEqual, n, config.ParameterName);

                VRCAvatarParameterDriver APD = state.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                for (int bit = 0; bit < config.BitNum; bit++)
                {
                    APD.parameters.Add(new VRC_AvatarParameterDriver.Parameter
                    {
                        type = VRC_AvatarParameterDriver.ChangeType.Set,
                        name = config.BitName[bit],
                        value = (n >> bit) & 1,
                    });
                }
            }
        }
        protected override void OnInstantiate() { }

    }

    public class NBitDACWriterMain : StateMachineWriter0
    {
        public NBitDACWriterMain(NBitIntMain config) : base(config.NBitIntController, config.UniqueName, "DAC", "Initial")
        {
            for (var n = 0; n < config.MaxValue; n++)
            {
                var state = CreateState($@"{n}", config.Dummy2F);
                var transIn = SetTransition(_initialState, state, config.QuickTrans);
                for (int bit = 0; bit < config.BitNum; bit++)
                {
                    var v = ((n >> bit) & 1) == 1;
                    transIn.AddCondition(v ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot, 0, config.BitName[bit]);
                    SetTransition(state, _exitState, config.QuickTrans).AddCondition(v ? AnimatorConditionMode.IfNot : AnimatorConditionMode.If, 0, config.BitName[bit]);
                }
                VRCAvatarParameterDriver APD = state.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                APD.parameters.Add(new VRC_AvatarParameterDriver.Parameter
                {
                    type = VRC_AvatarParameterDriver.ChangeType.Set,
                    name = config.ParameterName,
                    value = n,
                });
            }
        }
        protected override void OnInstantiate() { }

    }
}

#endif

