using nadena.dev.ndmf;
using com.github.pandrabox.emoteprefab.editor;

[assembly: ExportsPlugin(typeof(EmotePrefabPlugin))]

namespace com.github.pandrabox.emoteprefab.editor
{
    public class EmotePrefabPlugin : Plugin<EmotePrefabPlugin>
    {
        protected override void Configure()
        {
            InPhase(BuildPhase.Transforming).BeforePlugin("nadena.dev.modular-avatar").Run("com.github.pandrabox.emoteprefab", ctx =>
            {
                new EmotePrefabProcessor(ctx.AvatarDescriptor);
            });
        }
    }
}