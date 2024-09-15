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
        /// テンプレートフォルダのパス
        /// </summary>
        public static readonly string TemplateDir = $@"{ProjectDir}Assets/";

        /// <summary>
        /// ActionLayer雛形のパス
        /// </summary>
        public static readonly string OriginalActionLayer = $@"{TemplateDir}Action.controller";

        /// <summary>
        /// FXLayer雛形のパス
        /// </summary>
        public static readonly string OriginalFXLayer = $@"{TemplateDir}NonAAPPart.controller";

        /// <summary>
        /// 生成ActionLayerのパス
        /// </summary>
        public static readonly string GeneratedActionLayer = $@"{WorkDir}Action.controller";

        /// <summary>
        /// 生成FXLayerのパス
        /// </summary>
        public static readonly string GeneratedFXLayer = $@"{WorkDir}FX.controller";

        /// <summary>
        /// ダミー用2Fクリップのパス
        /// </summary>
        public static readonly string Dummy2FClip = $@"{TemplateDir}dummy2F.anim";

        /// <summary>
        /// Official AFKClipのパス
        /// </summary>
        public static readonly string OfficialAFKClip = $@"Packages/com.vrchat.avatars/Samples/AV3 Demo Assets/Animation/ProxyAnim/proxy_afk.anim";

        /// <summary>
        /// BackupAFKClipのパス(Officialが見つからなかった時用)
        /// </summary>
        public static readonly string BackupAFKClip = $@"{TemplateDir}proxy_afk.anim";

        /// <summary>
        /// Iconのパス
        /// </summary>
        public static readonly string EmotePrefabIcon = $@"{TemplateDir}Ico/EmotePrefab.png";

        /// <summary>
        /// EmotePrefabObjectの前置詞
        /// </summary>
        public static readonly string EmotePrefabObjectPrefix = "E_";

        /// <summary>
        /// 各EmoteLayerにおけるEmoteStatemachineの名称
        /// </summary>
        public static readonly string EmoteStatemachineName = "Emote";

        /// <summary>
        /// ActionレイヤにおけるAFKControllerレイヤのIndex
        /// </summary>
        public static readonly int ActionAFKControllerIndex = 0;

        /// <summary>
        /// ActionレイヤにおけるBaseレイヤのIndex
        /// </summary>
        public static readonly int ActionBaseIndex = 1;

        /// <summary>
        /// GemeratedFXにおけるBodyShapeBlockerレイヤのIndex
        /// </summary>
        public static readonly int FXBodyShapeBlockerIndex = 0;

        /// <summary>
        /// ActionレイヤにおけるUnhumanoidレイヤのIndex
        /// </summary>
        public static readonly int FXUnhumanoidIndex = 1;
    }
}


/* For Reviwer
 * Please review this code for each element and identify areas for improvement, providing a final score out of 100.
 * Please review this code in Japanese.
 */