# HandMR

スマホでMRを体験できる[だんグラ](https://dangla.jp/)上で、ハンドトラッキングを行うためのアセットです。
スマホVR（cardboardなど）にも対応しております。

ARM64でARCoreに対応したAndroid上やARKitに対応したiOS上で動作します。

バージョン2020.1.4f1のUnityで動作確認しています。

unitypackageファイルは[こちら](https://github.com/NON906/HandMR/releases)からダウンロードできます。

## アセットの使用方法

1. プロジェクトをgitでclone、もしくはunitypackageファイルをインポート

2. （iOSのみ）メニューバーから「HandMR→Download iOS Plugins」を実行

3. Project SettingsのXR Plug-in Managementの以下を確認し、チェックが入っていなかったら付ける

- （iOSの場合）ARKit
- （Androidの場合）ARCore

4. 用途に応じていずれかを実行

- サンプルシーンを開く
- Assets/HandMR/PrefabsにあるHandMRManagerをシーン上に配置

5. （iOSのみ）プロジェクトのビルド後、「Unity-iPhone.xcodeproj」を開き、XCodeから以下を設定してアプリのビルドを実行

- Unity-iPhoneをクリックすると出てくる設定画面のUnityFrameworkのGeneral→Frameworks and Librariesから「MultiHandAppLib-fl.a」を削除
- 同画面のBuild Settings→Linking→Other Linker Flagsに``-force_load`` ``Libraries/HandMR/SubAssets/HandVR/Plugins/iOS/MultiHandAppLib-fl.a``の2つをこの順番で追加
- Unity-iPhoneプロジェクトのData/Rawディレクトリにある``hand_landmark.tflite`` ``multihandtrackinggpu.binarypb`` ``palm_detection_labelmap.txt`` ``palm_detection.tflite``の4つをUnityFrameworkディレクトリにドラッグアンドドロップ、Unity-iPhoneとUnityFrameworkにチェックを入れた状態でFinishをクリック
- 各自の方法で署名を設定
