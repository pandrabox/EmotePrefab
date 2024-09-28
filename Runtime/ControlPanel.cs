#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using VRC.SDK3.Dynamics.PhysBone.Components;
using System.Linq;
using VRC.SDK3.Avatars.Components;
using static com.github.pandrabox.emoteprefab.runtime.Generic;
using UnityEngine.UIElements;
using System.IO;

namespace com.github.pandrabox.emoteprefab.runtime
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Pan/ControlPanel")]
    public class ControlPanel : MonoBehaviour, VRC.SDKBase.IEditorOnly
    {
        public bool Enable = false;
        public List<int> FaxialExpressionLayer = new List<int>() { 1, 2 };
        public int UseSyncBitNum;
        public bool UseHeightControl = true;
        public float HeightUpper=3, HeightLower=-3;
    }



    [CustomEditor(typeof(ControlPanel))]
    public class ControlPanelEditor : Editor
    {
        Texture2D Icon;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            try
            {
                if (Icon == null) Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(Config.LogoAndTitleIcon);
                if (Icon != null)
                {
                    // スペースを挿入して上部に配置
                    GUILayout.Space(5);

                    // 中央に配置するための計算
                    float windowWidth = EditorGUIUtility.currentViewWidth;
                    float iconWidth = 311;
                    float iconHeight = 67;

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace(); // 左側の余白
                    GUILayout.Label(Icon, GUILayout.Width(iconWidth), GUILayout.Height(iconHeight)); // アイコン表示
                    GUILayout.FlexibleSpace(); // 右側の余白
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                }
                var nowinstance = (ControlPanel)target;
                var descriptor = nowinstance.transform.parent.GetComponent<VRCAvatarDescriptor>();
                var enableProp = serializedObject.FindProperty("Enable");
                enableProp.boolValue = true;

                void Disable(string msg)
                {
                    enableProp.boolValue = false;
                    EditorGUILayout.HelpBox(msg, MessageType.Warning);
                }

                GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.wordWrap = true;  // 自動改行を有効にする
                GUILayout.Label("　ControlPanelは、EmotePrefabをアバター単位で全体設定するものです。アバターセットアップをする方が各自導入するもので、エモートデータに同梱されていた場合、何かの誤りかもしれません。", labelStyle);

                if (descriptor == null)
                {
                    Disable("親オブジェクトにVRCAvatarDescriptorが見つかりませんでした。親のオブジェクトの状態を確認するか、このオブジェクトを削除して「Pan→PutControlPanel」より再導入して下さい。");
                }
                var controlPanelsCount = descriptor.transform.Cast<Transform>().Count(c => c.name == "EmotePrefab_ControlPanel" && c.GetComponent<ControlPanel>() != null);
                if (controlPanelsCount > 1)
                {
                    Disable("ControlPanelが複数見つかりました。1つのアバターに導入できるControlPanelは1つのみです。不要なオブジェクトを削除して下さい。");
                }
                if (nowinstance.gameObject.name != "EmotePrefab_ControlPanel")
                {
                    Disable("このコンポーネントは「EmotePrefab_ControlPanel」という名前のオブジェクトにアタッチされているときのみ有効です。オブジェクトの名前を修正するか、削除して「Pan→PutControlPanel」より再導入して下さい。");
                }
                //if (descriptor.transform.chil)

                if (!enableProp.boolValue) return;
                var emotePrefabs = descriptor.transform.GetComponentsInChildren<EmotePrefab>(false).Where(e => e.Enable).ToArray();
                var hasAFK = emotePrefabs.Where(e => e.IsAFK).Any();
                GUILayout.Label("EmotePrefabが使用する同期Bit数", TitleStyle());
                var bitProp = serializedObject.FindProperty("UseSyncBitNum");
                var heightProp = serializedObject.FindProperty("UseHeightControl");
                bitProp.intValue = Mathf.CeilToInt(Mathf.Log(emotePrefabs.Length, 2)) + (heightProp.boolValue ? 8 : 0 );
                GUI.enabled = false;
                EditorGUILayout.PropertyField(bitProp);
                GUI.enabled = true;


                GUILayout.Label("Emote高さ調整機能", TitleStyle());
                EditorGUILayout.PropertyField(heightProp);
                if(heightProp.boolValue)
                {
                    var upperProp = serializedObject.FindProperty("HeightUpper");
                    var lowerProp = serializedObject.FindProperty("HeightLower");
                    EditorGUILayout.PropertyField(upperProp);
                    EditorGUILayout.PropertyField(lowerProp);

                    if (Mathf.Abs(upperProp.floatValue + lowerProp.floatValue) > 0.05f)
                    {
                        EditorGUILayout.HelpBox("UpperとLowerは合計0にすることを強く推奨します", MessageType.Warning);
                    }
                }

                GUILayout.Label("表情レイヤ", TitleStyle());
                EditorGUILayout.PropertyField(serializedObject.FindProperty("FaxialExpressionLayer"));

                GUILayout.Label("EmotePrefab一括生成", TitleStyle());
                if (GUILayout.Button("起動"))
                {
                    EmotePrefabBulkGeneration.ShowWindow();
                }
                
            }
            finally
            {
                serializedObject.ApplyModifiedProperties();
            }
        }


        private static GUIStyle TitleStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.normal.background = MakeTex(1, 1, new Color(57f / 255f, 167f / 255f, 244f / 255f, 1f));
            style.normal.textColor = Color.black;
            style.fontStyle = FontStyle.Bold;
            return style;
        }
        private static Texture2D MakeTex(int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = color;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
#endif