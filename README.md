# HandMR

スマホでMRを体験できる[だんグラ](https://dangla.jp/)上で、ハンドトラッキングを行うためのアセットです。
スマホVR（cardboard）にも対応しております。

ARM64でARCoreに対応したAndroid上やARKitに対応したiOS上で動作します。

バージョン2019.2.21f1のUnityで動作確認しています。
2019.3以降はiOSでは未対応なので、2019.2系で使用するようにしてください。

unitypackageファイルは[こちら](https://github.com/NON906/HandMR/releases)からダウンロードできます。

このプロジェクトには各アセットを自動でダウンロードする機能があります。
ライセンスなどについては、個々のアセットごとに確認するよう、お願いいたします。

## アセットの使用方法

1. プロジェクトをgitでclone、もしくはunitypackageファイルをインポート

2. メニューバーから以下を順番に実行（サンプルシーンの場合、Initialize Sceneは不要です）

Android

- HandMR→Android→Download Assets and Init Project with MR
- Hologla→ARCore→Initialize Project with ARCore
- （VRも使用する場合）HandMR→Android→Download Assets and Init Project with VR
- （VRも使用する場合）HandMR→Android→Initialize Prefabs with VR
- （MR用のシーン上で）Hologla→ARCore→Initialize Scene with ARCore
- （MR用のシーン上で）HandMR→Android→Initialize Scene with MR
- （VR用のシーン上で）HandMR→Android→Initialize Scene with VR

iOS

- HandMR→iOS→Download Assets and Init Project with MR
- Hologla→ARKit→Initialize Project with ARKit
- （VRも使用する場合）HandMR→iOS→Download Assets and Init Project with VR
- （VRも使用する場合）HandMR→iOS→Initialize Prefabs with VR
- （MR用のシーン上で）Hologla→ARKit→Initialize Scene with ARKit
- （MR用のシーン上で）HandMR→iOS→Initialize Scene with MR
- （VR用のシーン上で）HandMR→iOS→Initialize Scene with VR

3. （iOSのみ）プロジェクトのビルド後、「Unity-iPhone.xcworkspace」を開き、XCodeから以下を設定してアプリのビルドを実行

- Unity-iPhoneをクリックすると出てくる設定画面のGeneral→Frameworks, Libraries, and Embedded Contentから「MultiHandAppLib-fl.a」を削除
- 同画面のBuild Settings→Linking→Other Linker Flagsに``-force_load`` ``Libraries/HandMR/SubAssets/HandVR/Plugins/iOS/MultiHandAppLib-fl.a``の2つを追加
- Unity-iPhoneプロジェクトのData/Rawディレクトリにある``hand_landmark.tflite`` ``multihandtrackinggpu.binarypb`` ``palm_detection_labelmap.txt`` ``palm_detection.tflite``の4つをUnity-iPhone直下にドラッグアンドドロップし、初期設定のままFinishをクリック
