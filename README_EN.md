# HandMR

This is assets for hands tracking with [Dangla](https://dangla.jp/) MR or smartphone VR (cardboard etc).

This works Android (ARM64 and ARCore) or iOS (ARKit).

You can use [AR Foundation Remote 2](https://assetstore.unity.com/packages/tools/utilities/ar-foundation-remote-2-0-201106) for remote debugging.

Confirmed with Unity (version 2020.3.12f1).

Unitypackage file is [here](https://github.com/NON906/HandMR/releases).

Asset Store page is [here](https://assetstore.unity.com/packages/slug/181940).

## How to use

1. ``git clone`` or import this asset.

2. The setting window will open (or open from Tools->HandMR->Show Start Dialog Window), so execute contents.

3. Placement Assets/HandMR/Prefabs/HandMRManager.prefab on scene.

<div style="page-break-before:always"></div>

New version is support new Input System (and URP).  
You can now treat it like a regular XR device with XR-Interaction-Toolkit etc.  
The following is how to use it in the XR-Interaction-Toolkit sample.

1. ``git clone`` or download sample project.  
[https://github.com/Unity-Technologies/XR-Interaction-Toolkit-Examples](https://github.com/Unity-Technologies/XR-Interaction-Toolkit-Examples)

2. Open sample project, and import this asset.

3. The setting window will open (or open from Tools->HandMR->Show Start Dialog Window), so execute contents.

4. Open XR_UniversalRenderPipelineAsset_Renderer and add 'Background Renderer Feature' to 'Renderer Features'.

5. Open WorldInteractionDemo scene.

6. Disable XRRigDemo/CameraOffset/Main Camera.

7. Add HandMRManagerURP prefab and HandArea prefab (In Assets/HandMR/Prefabs) to scene.

8. Change the following on XRRigDemo from 'Main Camera' to 'HandMRManagerURP/MRObject/HologlaCameraParent/HologlaCamera' (be added in 7.).
- XRRig->Camera GameObject
- ContinuousMoveProvider->Forward Source
- LocomotionSchemeManager->Head Forward Source

9. Setting 'HandMRManagerURP->HandMRManager->Center Transform' to 'XRRigDemo/CameraOffset'.

10. Setting 'Assets/Samples/XR Interaction Toolkit/1.0.0-pre.6/Default Input Actions/XRI Default Input Actions.inputactions' (sample: 'Assets/InputActions/HandMR XRI Default Input Actions.inputactions').

11. Attach SettingFromPlayerPrefs to HandMRManagerURP (Optional).

<div style="page-break-before:always"></div>

## License

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