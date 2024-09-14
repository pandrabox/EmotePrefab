# EmotePrefab
![sample](/.github/Img/sample.png)
- A tool for assembling Action layer out of reusable components
- [EmotePrefabをVCCに追加](https://pandrabox.github.io/vpm/)
- [EmotePrefabをunitypackage(VPAI)で追加](https://api.anatawa12.com/create-vpai/?name=EmotePrefab-installer.unitypackage&repo=https://pandrabox.github.io/vpm/index.json&package=com.github.pandrabox.emoteprefab&version=>=1.1.0)
---
- 現在完成している機能
    - EmotePrefabコンポーネント
        - Motion…Emoteのモーションデータ
        - Name…メニューに表示する名前
        - IsOneShot…1回で終了する場合True
    - EmotePrefabコンポーネントに定義されているモーションを選択できるメニュー作成
    - メニューから実行すると該当モーションを実行
    - 非Humanoidアニメ対応
- 今後実装する機能
    - Issue参照
- 注意事項
    - 本アセットはActionレイヤの既存の設定を完全に無視して上書きします。アバターに設定済みのモーション・AFK等は無視されます
    - 関連アセットとの同時使用は未検証で、不具合が予見されます。お気づきの点あればお気軽にIssue登録下さい

# thanks
- [VRChat向けCustom Animator Controllers](https://booth.pm/ja/items/4424448)
-- v2.0 Actionレイヤをベースに作成しました
- [Modular Avatar](https://github.com/bdunderscore/modular-avatar)
-- メニュー作成等で利用しています。その他、githubの使用方法、コーディング方法などの参考として大いにお世話になりました
- [Non-Destructive Modular Framework](https://github.com/bdunderscore/ndmf)
-- 完全に依存しています
- [HyakuashiUdonMotionRecorder](https://github.com/mukaderabbit/mukaderabbit-HyakuashiUdonMotionRecorder-HUMR-)
-- モーションデータの作成に使用させて頂いています
- [AvatarParametersDriver](https://github.com/Narazaka/AvatarParametersDriver)
-- VRC_AvatarParametersDriverのコーディングの参考にさせて頂きました