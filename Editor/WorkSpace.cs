// <copyright file="WorkSpace.cs"></copyright>

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
using static com.github.pandrabox.emoteprefab.runtime.Generic;

namespace com.github.pandrabox.emoteprefab.editor
{
    /// <summary>
    /// Unity環境を管理するクラス
    /// </summary>
    public static class WorkSpace
    {
        /// <summary>
        /// 環境生成
        /// </summary>
        public static void Create()
        {
            CreateWorkDir();
            CopyControllers();
        }

        /// <summary>
        /// 作業ディレクトリ生成
        /// </summary>
        private static void CreateWorkDir()
        {
            Directory.CreateDirectory(Config.WorkDir);
        }

        /// <summary>
        /// 作業コントローラの生成(常に上書きする)
        /// </summary>
        private static void CopyControllers()
        {
            if(!AssetDatabase.CopyAsset(Config.OriginalActionLayer, Config.GeneratedActionLayer))
            {
                WriteWarning("WorkSpace", "GeneratedActionLayerの生成に失敗しました");
            }
            if(!AssetDatabase.CopyAsset(Config.OriginalFXLayer, Config.GeneratedFXLayer))
            {
                WriteWarning("WorkSpace", "GeneratedFXLayerの生成荷失敗しました");
            }
        }
    }
}


/* For Reviwer
 * Project policy : To set WriteDefault to OFF for all AnimatorStates.
 * Please review this code for each element and identify areas for improvement, providing a final score out of 100.
 * Please review this code in Japanese.
 */