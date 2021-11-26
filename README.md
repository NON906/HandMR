# HandMR

English version is [here](https://github.com/NON906/HandMR/blob/master/README_EN.md).

スマホでMRを体験できる[だんグラ](https://dangla.jp/)上で、ハンドトラッキングを行うためのアセットです。
スマホVR（cardboardなど）にも対応しております。

ARM64でARCoreに対応したAndroid上やARKitに対応したiOS上で動作します。

[AR Foundation Remote 2](https://assetstore.unity.com/packages/tools/utilities/ar-foundation-remote-2-0-201106)によるリモートデバッグにも対応しております。

バージョン2020.3.12f1のUnityで動作確認しています。

unitypackageファイルは[こちら](https://github.com/NON906/HandMR/releases)からダウンロードできます。

[Asset Store](https://assetstore.unity.com/packages/slug/181940)でも配布しております。

## アセットの使用方法

1. プロジェクトをgitでclone、もしくはunitypackageファイルをインポート

2. 設定画面が開く（開かない場合はTools→HandMR→Show Start Dialog Windowから開く）ので、内容を確認して実行

3. Assets/HandMR/PrefabsにあるHandMRManagerをシーン上に配置

<div style="page-break-before:always"></div>

最新バージョンで、新しいInput System（及びURP）に対応いたしました。  
これに伴い、XR-Interaction-Toolkitといったものにも通常のXRデバイスと同様に扱うことが出来るようになりました。  
以下はXR-Interaction-Toolkitのサンプルを動作させる手順になります。

1. 以下のプロジェクトをクローンするか、ダウンロードする  
[https://github.com/Unity-Technologies/XR-Interaction-Toolkit-Examples](https://github.com/Unity-Technologies/XR-Interaction-Toolkit-Examples)

2. プロジェクトを開き、このアセットをインポートする

3. 設定画面が開く（開かない場合はTools→HandMR→Show Start Dialog Windowから開く）ので、内容を確認して実行

4. XR_UniversalRenderPipelineAsset_Rendererを開き、Renderer FeaturesにAR Background Renderer Featureを追加

5. WorldInteractionDemoシーンを開く

6. シーンにあるXRRigDemo/CameraOffset/Main Cameraを無効にする

7. Assets/HandMR/PrefabsにあるHandMRManagerURPとHandAreaをシーンに追加

8. XRRigDemoにある以下の設定を、全てMain Cameraから、7.で配置したHandMRManagerURPにあるMRObject/HologlaCameraParent/HologlaCameraに変更
- XRRig→Camera GameObject
- ContinuousMoveProvider→Forward Source
- LocomotionSchemeManager→Head Forward Source

9. HandMRManagerURPのHandMRManager→Center TransformをXRRigDemo/CameraOffsetに設定

10. XRRigDemo/CameraOffsetの位置を(0, 0, 0)に設定

11. Assets/Samples/XR Interaction Toolkit/1.0.0-pre.6/Default Input Actions/XRI Default Input Actions.inputactionsをAssets/InputActions/HandMR XRI Default Input Actions.inputactionsを参考に変更

12. （必要であれば）HandMRManagerURPにSettingFromPlayerPrefsをアタッチ

<div style="page-break-before:always"></div>

## ライセンス

A. HandMR MIT License

Copyright (c) 2020 NON906

B. MediaPipe Apache2.0

Copyright 2019 The MediaPipe Authors.

C. OpenCV 3-clause BSD License

Copyright (C) 2000-2019, Intel Corporation, all rights reserved.

Copyright (C) 2009-2011, Willow Garage Inc., all rights reserved.

Copyright (C) 2009-2016, NVIDIA Corporation, all rights reserved.

Copyright (C) 2010-2013, Advanced Micro Devices, Inc., all rights reserved.

Copyright (C) 2015-2016, OpenCV Foundation, all rights reserved.

Copyright (C) 2015-2016, Itseez Inc., all rights reserved.

D. HologlaSDK-Unity MIT License

Copyright (c) 2019 Hologram Co., Ltd.

E. hand-gesture-recognition-using-mediapipe Apache2.0

Author: Kazuhito Takahashi