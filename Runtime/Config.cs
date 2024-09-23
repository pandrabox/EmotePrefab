#if UNITY_EDITOR

using System.Collections.Generic;
using static com.github.pandrabox.emoteprefab.runtime.TransitionInfo;

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
        public static readonly string OriginalFXLayer = $@"{TemplateDir}FX.controller";

        /// <summary>
        /// FXLayer雛形のパス
        /// </summary>
        public static readonly string OriginalFXRelativeLayer = $@"{TemplateDir}FXRelative.controller";

        /// <summary>
        /// 生成ActionLayerのパス
        /// </summary>
        public static readonly string GeneratedActionLayer = $@"{WorkDir}Action.controller";

        /// <summary>
        /// 生成FXLayerのパス
        /// </summary>
        public static readonly string GeneratedFXLayer = $@"{WorkDir}FX.controller";
        /// <summary>
        /// 生成FXLayerのパス
        /// </summary>
        public static readonly string GeneratedFXRelativeLayer = $@"{WorkDir}FXRelative.controller";

        /// <summary>
        /// ダミー用2Fクリップのパス
        /// </summary>
        public static readonly string Dummy2FClip = $@"{TemplateDir}dummy2F.anim";

        /// <summary>
        /// AFKのパス
        /// </summary>
        public static readonly string AFKClip = $@"{TemplateDir}AFK.anim";

        /// <summary>
        /// Iconのパス
        /// </summary>
        public static readonly string EmotePrefabIcon = $@"{TemplateDir}Ico/EmotePrefab.png";

        /// <summary>
        /// OneShotIconのパス
        /// </summary>
        public static readonly string OneShotIcon = $@"{TemplateDir}Ico/OneShot.png";

        /// <summary>
        /// LoopIconのパス
        /// </summary>
        public static readonly string LoopIcon = $@"{TemplateDir}Ico/Loop.png";

        /// <summary>
        /// HoldIconのパス
        /// </summary>
        public static readonly string HoldIcon = $@"{TemplateDir}Ico/Hold.png";

        /// <summary>
        /// HoldIconのパス
        /// </summary>
        public static readonly string UnstoppableIcon = $@"{TemplateDir}Ico/Unstoppable.png";

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
        /// GemeratedFXレイヤにおけるUnhumanoidレイヤのIndex
        /// </summary>
        public static readonly int FXUnhumanoidIndex = 1;

        /// <summary>
        /// GemeratedFXレイヤにおけるShrinkPhysBonesレイヤのIndex
        /// </summary>
        public static readonly int ShrinkPhysBonesIndex = 2;
    }
}


/* For Reviwer
 * Please review this code for each element and identify areas for improvement, providing a final score out of 100.
 * Please review this code in Japanese.
 */
#endif