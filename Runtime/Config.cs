// <copyright file="Config.cs" ></copyright>

namespace com.github.pandrabox.emoteprefab.runtime
{
    /// <summary>
    /// プロジェクトで使用している定数
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Projectフォルダの絶対パス
        /// </summary>
        public static readonly string ProjectDir = "Packages/com.github.pandrabox.emoteprefab/";

        /// <summary>
        /// Workフォルダの絶対パス
        /// </summary>
        public static readonly string WorkDir = $@"{ProjectDir}Work/";

        /// <summary>
        /// ActionLayer雛形のパス
        /// </summary>
        public static readonly string OriginalActionLayer = $@"{ProjectDir}Assets/BearsDen/CustomAnimatorControllers/Action.controller";

        /// <summary>
        /// FXLayer雛形のパス
        /// </summary>
        public static readonly string OriginalFXLayer = $@"{ProjectDir}Assets/Pan/NonAAPPart/NonAAPPart.controller";

        /// <summary>
        /// 生成ActionLayerのパス
        /// </summary>
        public static readonly string GeneratedActionLayer = $@"{WorkDir}Action.controller";

        /// <summary>
        /// 生成FXLayerのパス
        /// </summary>
        public static readonly string GeneratedFXLayer = $@"{WorkDir}FX.controller";

        /// <summary>
        /// EmotePrefabObjectの前置詞
        /// </summary>
        public static readonly string EmotePrefabObjectPrefix = "E_";

        /// <summary>
        /// 各EmoteLayerにおけるEmoteStatemachineの名称
        /// </summary>
        public static readonly string EmoteStatemachineName = "Emote";
    }
}


/* For Reviwer
 * Please review this code for each element and identify areas for improvement, providing a final score out of 100.
 * Please review this code in Japanese.
 */