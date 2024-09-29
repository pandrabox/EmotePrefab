using nadena.dev.ndmf;
using com.github.pandrabox.emoteprefab.editor;
using com.github.pandrabox.emoteprefab.runtime;
using System.Linq;
using System.Diagnostics;
using static com.github.pandrabox.emoteprefab.runtime.Generic;

[assembly: ExportsPlugin(typeof(EmotePrefabPlugin))]

namespace com.github.pandrabox.emoteprefab.editor
{
    public class EmotePrefabPlugin : Plugin<EmotePrefabPlugin>
    {
        protected override void Configure()
        {
            InPhase(BuildPhase.Transforming).BeforePlugin("nadena.dev.modular-avatar").Run("com.github.pandrabox.emoteprefab", ctx =>
            {
                if (ctx.AvatarRootTransform.GetComponentsInChildren<EmotePrefab>(false).Length==0)
                {
                    return;
                }
                new EmotePrefabProcessor(ctx.AvatarDescriptor);
            });
        }
    }
}