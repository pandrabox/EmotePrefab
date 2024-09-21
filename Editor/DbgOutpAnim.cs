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
using VRC.SDK3.Dynamics.PhysBone.Components;
using static com.github.pandrabox.emoteprefab.runtime.Generic;
using static com.github.pandrabox.emoteprefab.editor.EmoteManager;

namespace com.github.pandrabox.emoteprefab.editor
{
    /// <summary>
    /// EmotePrefabの処理で生成されるAnimationをWorkフォルダに出力します。これはデバッグ時便利なことがありますが、使用しているEmotePrefabに書き込みを行うので決して公開版では使わないこと
    /// </summary>
    public class DbgOutpAnim
    {
        public DbgOutpAnim()
        {
            for(int m = 0; m < EmotePrefabs.Length; m++)
            {
                for (int n = 0; n < EmotePrefabs[m].UnitMotions.Count; n++)
                {
                    var clip = EmotePrefabs[m].UnitMotions[n].Clip;
                    OutpUnit(clip.Humanoid, "0hum", m, n);
                    OutpUnit(clip.PokerFace, "1pkf", m, n);
                    OutpUnit(clip.UnHumanoid, "2unh", m, n);
                    OutpUnit(clip.FakeWD, "3fwd", m, n);
                    OutpUnit(clip.UnHumanoidR, "4unhR", m, n);
                    OutpUnit(clip.FakeWDR, "5fwdR", m, n);
                    OutpUnit(clip.ShrinkPB, "6spb", m, n);
                    OutpUnit(clip.ShrinkWD, "7swd", m, n);
                }
            }

        }
        public void OutpUnit(AnimationClip c, string p, int m, int n)
        {
            if (c == null) return;
            var filename = $@"{Config.WorkDir}anim/e{m + 1}c{n}_{p}.anim";
            // WriteWarning("AnimOutp", filename);
            Directory.CreateDirectory($@"{Config.WorkDir}anim/");
            AssetDatabase.CreateAsset(c, filename);
        }
    }
}
