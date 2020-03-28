//#define DOWNLOADED_HOLOGLA
//#define DOWNLOADED_ARCORE
//#define DOWNLOADED_ARKIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine.Networking;
using System.IO;
#if DOWNLOADED_HOLOGLA
using Hologla;
#endif
#if DOWNLOADED_ARCORE
using GoogleARCore;
#endif
#if DOWNLOADED_ARKIT
using UnityEngine.XR.iOS;
#endif

public class HandMRSceneInitializer
{
    static bool addPackageManager(string packageName)
    {
        var request = UnityEditor.PackageManager.Client.Add(packageName);
        do
        {
            System.Threading.Thread.Sleep(16);
        } while (request.Status == UnityEditor.PackageManager.StatusCode.InProgress);
        if (request.Status != UnityEditor.PackageManager.StatusCode.Success)
        {
            EditorUtility.DisplayDialog("エラー", packageName + "を追加できませんでした", "OK");
            return false;
        }
        return true;
    }

    static bool download(string url, string path)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            UnityWebRequestAsyncOperation request = webRequest.SendWebRequest();
            while (!request.isDone)
            {
                EditorUtility.DisplayProgressBar("Download Assets", "Downloading: " + url, request.progress);
                System.Threading.Thread.Sleep(16);
            }
            if (webRequest.error != null)
            {
                EditorUtility.DisplayDialog("エラー", "ダウンロードに失敗しました\n" + url + "\n" + webRequest.error, "OK");
                return false;
            }
            File.WriteAllBytes(path, webRequest.downloadHandler.data);
        }

        return true;
    }

    static void setScriptingDefineSymbol(string symbol)
    {
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        if (!symbols.Contains(symbol))
        {
            if (symbols == null || symbols == "")
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, symbol);
            }
            else
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, symbols + ";" + symbol);
            }
        }

        symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
        if (!symbols.Contains(symbol))
        {
            if (symbols == null || symbols == "")
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, symbol);
            }
            else
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, symbols + ";" + symbol);
            }
        }
    }

    static void initARCore()
    {
        addPackageManager("com.unity.xr.legacyinputhelpers");
        addPackageManager("com.unity.multiplayer-hlapi");
        addPackageManager("com.unity.xr.googlevr.android@2.0.0");

        if (!Directory.Exists(Application.dataPath + "/../../downloads"))
        {
            Directory.CreateDirectory(Application.dataPath + "/../../downloads");
        }
        if (!Directory.Exists(Application.dataPath + "/GoogleARCore"))
        {
            download("https://github.com/google-ar/arcore-unity-sdk/releases/download/v1.16.0/arcore-unity-sdk-1.16.0.unitypackage", Application.dataPath + "/../../downloads/arcore-unity-sdk-1.16.0.unitypackage");
            AssetDatabase.ImportPackage(Application.dataPath + "/../../downloads/arcore-unity-sdk-1.16.0.unitypackage", false);
        }
        if (!Directory.Exists(Application.dataPath + "/Hologla"))
        {
            download("https://github.com/ho-lo/HologlaSDK-Unity/releases/download/v0.9.0-beta/Hologla.unitypackage", Application.dataPath + "/../../downloads/Hologla.unitypackage");
            AssetDatabase.ImportPackage(Application.dataPath + "/../../downloads/Hologla.unitypackage", false);
        }

        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

        setScriptingDefineSymbol("DOWNLOADED_HOLOGLA");
        setScriptingDefineSymbol("DOWNLOADED_ARCORE");
    }

    [MenuItem("HandMR/Android/Download Assets and Init Project with MR")]
    static void InitProjectForARCore()
    {
        initARCore();
    }

    [MenuItem("HandMR/Android/Initialize Prefabs with MR")]
    static void InitPrefabsForMRARCore()
    {
#if DOWNLOADED_ARCORE
        string prefabPath = "Assets/HandMR/SubAssets/MRUtil/Prefabs/HandMRARCore.prefab";
        GameObject contentsRoot = PrefabUtility.LoadPrefabContents(prefabPath);
        ARCoreBackgroundRenderer arcoreBackground = contentsRoot.GetComponentInChildren<ARCoreBackgroundRenderer>();
        arcoreBackground.BackgroundMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/GoogleARCore/SDK/Materials/ARBackground.mat");
        PrefabUtility.SaveAsPrefabAsset(contentsRoot, prefabPath);
        PrefabUtility.UnloadPrefabContents(contentsRoot);
#endif
    }

#if DOWNLOADED_HOLOGLA && DOWNLOADED_ARCORE
    [MenuItem("HandMR/Android/Initialize Scene with MR")]
    static void InitSceneForARCore()
    {
        GameObject handmrObject = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HandMR/SubAssets/MRUtil/Prefabs/HandMRARCore.prefab")) as GameObject;
        Undo.RegisterCreatedObjectUndo(handmrObject, "Create object");
        GameObject[] mrHandObjects = new GameObject[2];
        for (int loop = 0; loop < 2; loop++)
        {
            mrHandObjects[loop] = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HandMR/SubAssets/MRHand/Prefabs/MRHand.prefab")) as GameObject;
            Undo.RegisterCreatedObjectUndo(mrHandObjects[loop], "Create object");
        }

        HologlaCameraManager hologlaCameraManager = Object.FindObjectOfType<HologlaCameraManager>();
        HologlaInput hologlaInput = Object.FindObjectOfType<HologlaInput>();
        SettingFromPlayerPrefs settingFromPlayerPrefs = handmrObject.GetComponent<SettingFromPlayerPrefs>();
        MRCameraTargetManager mrCameraTargetManager = handmrObject.GetComponentInChildren<MRCameraTargetManager>();
        Canvas background = mrCameraTargetManager.GetComponentInChildren<Canvas>();
        GazeInput gazeInput = hologlaCameraManager.GetComponent<GazeInput>();
        GameObject sphere = hologlaCameraManager.transform.Find("Sphere").gameObject;
        ARCoreBackgroundRenderer arcoreBackground = handmrObject.GetComponentInChildren<ARCoreBackgroundRenderer>();

        Undo.RegisterCompleteObjectUndo(new Object[] {
            handmrObject,
            mrHandObjects[0],
            mrHandObjects[1],
            hologlaCameraManager,
            hologlaInput,
            hologlaInput.LeftButtonComp,
            hologlaInput.RightButtonComp,
            settingFromPlayerPrefs,
            mrCameraTargetManager,
            background,
            mrHandObjects[0].GetComponent<SetParentMainCamera>(),
            mrHandObjects[1].GetComponent<SetParentMainCamera>(),
            mrHandObjects[0].GetComponent<HandVRSphereHand>(),
            mrHandObjects[1].GetComponent<HandVRSphereHand>(),
            gazeInput,
            sphere,
            arcoreBackground
        }, "Initialize Scene with MR");

        hologlaCameraManager.nearClipPlane = 0.01f;
        gazeInput.enabled = false;
        sphere.SetActive(false);

        UnityEventTools.RemovePersistentListener(hologlaInput.onPressLeftAndRight, gazeInput.InputLeftAndRightEvent);
        UnityEventTools.RemovePersistentListener(hologlaInput.LeftButtonComp.onClick, gazeInput.InputLeftEvent);
        UnityEventTools.RemovePersistentListener(hologlaInput.RightButtonComp.onClick, gazeInput.InputRightEvent);

        settingFromPlayerPrefs.HologlaCameraManagerObj = hologlaCameraManager.gameObject;
        settingFromPlayerPrefs.LeftButton = hologlaInput.LeftButtonComp.transform;
        settingFromPlayerPrefs.RightButton = hologlaInput.RightButtonComp.transform;

        mrCameraTargetManager.Cameras = new Camera[] {
            hologlaCameraManager.transform.Find("SingleEyeCamera").GetComponent<Camera>(),
            hologlaCameraManager.transform.Find("MultiCamera/LeftEyeCamera").GetComponent<Camera>(),
            hologlaCameraManager.transform.Find("MultiCamera/RightEyeCamera").GetComponent<Camera>()
        };
        mrCameraTargetManager.CameraTransform = hologlaCameraManager.transform;

        UnityEventTools.RemovePersistentListener(hologlaInput.LeftButtonComp.onClick, mrCameraTargetManager.ResetPosition);
        UnityEventTools.AddPersistentListener(hologlaInput.LeftButtonComp.onClick, mrCameraTargetManager.ResetPosition);

        background.worldCamera = hologlaCameraManager.GetComponent<Camera>();

        for (int loop = 0; loop < 2; loop++)
        {
            mrHandObjects[loop].GetComponent<SetParentMainCamera>().MainCameraTransform = hologlaCameraManager.transform;
            mrHandObjects[loop].GetComponent<HandVRSphereHand>().Id = loop;
        }

        arcoreBackground.BackgroundMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/GoogleARCore/SDK/Materials/ARBackground.mat");

        Undo.PerformUndo();
        Undo.PerformRedo();
    }
#endif

    [MenuItem("HandMR/Android/Download Assets and Init Project with VR")]
    static void InitProjectForARCoreVR()
    {
        initARCore();

        PlayerSettings.SetVirtualRealitySupported(BuildTargetGroup.Android, true);
        PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Android, new string[] { "None", "cardboard" });
    }

    static void initARKit()
    {
        addPackageManager("com.unity.xr.legacyinputhelpers");
        addPackageManager("com.unity.multiplayer-hlapi");
        addPackageManager("com.unity.xr.googlevr.ios");

        if (!Directory.Exists(Application.dataPath + "/../../downloads"))
        {
            Directory.CreateDirectory(Application.dataPath + "/../../downloads");
        }
        if (!Directory.Exists(Application.dataPath + "/UnityARKitPlugin"))
        {
            download("https://github.com/NON906/HandMR/releases/download/0.1/HandMR_iOS_plugin_for_projects_0.1.unitypackage", Application.dataPath + "/../../downloads/HandMR_iOS_plugin_for_projects_0.1.unitypackage");
            AssetDatabase.ImportPackage(Application.dataPath + "/../../downloads/HandMR_iOS_plugin_for_projects_0.1.unitypackage", false);
        }
        if (!Directory.Exists(Application.dataPath + "/Hologla"))
        {
            download("https://github.com/ho-lo/HologlaSDK-Unity/releases/download/v0.9.0-beta/Hologla.unitypackage", Application.dataPath + "/../../downloads/Hologla.unitypackage");
            AssetDatabase.ImportPackage(Application.dataPath + "/../../downloads/Hologla.unitypackage", false);
        }

        PlayerSettings.iOS.targetOSVersionString = "11.0";
        PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 1);

        setScriptingDefineSymbol("DOWNLOADED_HOLOGLA");
        setScriptingDefineSymbol("DOWNLOADED_ARKIT");
    }

    [MenuItem("HandMR/iOS/Download Assets and Init Project with MR")]
    static void InitProjectForARKit()
    {
        initARKit();
    }

    [MenuItem("HandMR/iOS/Initialize Prefabs with MR")]
    static void InitPrefabsForMRARKit()
    {
#if DOWNLOADED_ARKIT
        string prefabPath = "Assets/HandMR/SubAssets/MRUtil/Prefabs/HandMRARKit.prefab";
        GameObject contentsRoot = PrefabUtility.LoadPrefabContents(prefabPath);
        UnityARVideo video = contentsRoot.GetComponentInChildren<UnityARVideo>();
        video.m_ClearMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/UnityARKitPlugin/Plugins/iOS/UnityARKit/Materials/YUVMaterial.mat");
        PrefabUtility.SaveAsPrefabAsset(contentsRoot, prefabPath);
        PrefabUtility.UnloadPrefabContents(contentsRoot);
#endif
    }

#if DOWNLOADED_HOLOGLA && DOWNLOADED_ARKIT
    [MenuItem("HandMR/iOS/Initialize Scene with MR")]
    static void InitSceneForARKit()
    {
        GameObject handmrObject = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HandMR/SubAssets/MRUtil/Prefabs/HandMRARKit.prefab")) as GameObject;
        Undo.RegisterCreatedObjectUndo(handmrObject, "Create object");
        GameObject[] mrHandObjects = new GameObject[2];
        for (int loop = 0; loop < 2; loop++)
        {
            mrHandObjects[loop] = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HandMR/SubAssets/MRHand/Prefabs/MRHand.prefab")) as GameObject;
            Undo.RegisterCreatedObjectUndo(mrHandObjects[loop], "Create object");
        }

        HologlaCameraManager hologlaCameraManager = Object.FindObjectOfType<HologlaCameraManager>();
        HologlaInput hologlaInput = Object.FindObjectOfType<HologlaInput>();
        SettingFromPlayerPrefs settingFromPlayerPrefs = handmrObject.GetComponent<SettingFromPlayerPrefs>();
        MRCameraTargetManager mrCameraTargetManager = handmrObject.GetComponentInChildren<MRCameraTargetManager>();
        ARKitCameraTarget cameraTarget = mrCameraTargetManager.ARKitCameraTargetObject.GetComponent<ARKitCameraTarget>();
        Canvas background = mrCameraTargetManager.GetComponentInChildren<Canvas>();
        GazeInput gazeInput = hologlaCameraManager.GetComponent<GazeInput>();
        GameObject sphere = hologlaCameraManager.transform.Find("Sphere").gameObject;
        UnityARVideo video = handmrObject.GetComponentInChildren<UnityARVideo>();

        Undo.RegisterCompleteObjectUndo(new Object[] {
            handmrObject,
            mrHandObjects[0],
            mrHandObjects[1],
            hologlaCameraManager,
            hologlaInput,
            hologlaInput.LeftButtonComp,
            hologlaInput.RightButtonComp,
            settingFromPlayerPrefs,
            mrCameraTargetManager,
            cameraTarget,
            background,
            mrHandObjects[0].GetComponent<SetParentMainCamera>(),
            mrHandObjects[1].GetComponent<SetParentMainCamera>(),
            mrHandObjects[0].GetComponent<HandVRSphereHand>(),
            mrHandObjects[1].GetComponent<HandVRSphereHand>(),
            gazeInput,
            sphere,
            video
        }, "Initialize Scene with MR");

        hologlaCameraManager.nearClipPlane = 0.01f;
        gazeInput.enabled = false;
        sphere.SetActive(false);

        UnityEventTools.RemovePersistentListener(hologlaInput.onPressLeftAndRight, gazeInput.InputLeftAndRightEvent);
        UnityEventTools.RemovePersistentListener(hologlaInput.LeftButtonComp.onClick, gazeInput.InputLeftEvent);
        UnityEventTools.RemovePersistentListener(hologlaInput.RightButtonComp.onClick, gazeInput.InputRightEvent);

        settingFromPlayerPrefs.HologlaCameraManagerObj = hologlaCameraManager.gameObject;
        settingFromPlayerPrefs.LeftButton = hologlaInput.LeftButtonComp.transform;
        settingFromPlayerPrefs.RightButton = hologlaInput.RightButtonComp.transform;

        mrCameraTargetManager.Cameras = new Camera[] {
            hologlaCameraManager.transform.Find("SingleEyeCamera").GetComponent<Camera>(),
            hologlaCameraManager.transform.Find("MultiCamera/LeftEyeCamera").GetComponent<Camera>(),
            hologlaCameraManager.transform.Find("MultiCamera/RightEyeCamera").GetComponent<Camera>()
        };
        mrCameraTargetManager.CameraTransform = hologlaCameraManager.transform;

        cameraTarget.CameraObject = hologlaCameraManager.gameObject;

        UnityEventTools.RemovePersistentListener(hologlaInput.LeftButtonComp.onClick, mrCameraTargetManager.ResetPosition);
        UnityEventTools.AddPersistentListener(hologlaInput.LeftButtonComp.onClick, mrCameraTargetManager.ResetPosition);

        background.worldCamera = hologlaCameraManager.GetComponent<Camera>();

        for (int loop = 0; loop < 2; loop++)
        {
            mrHandObjects[loop].GetComponent<SetParentMainCamera>().MainCameraTransform = hologlaCameraManager.transform;
            mrHandObjects[loop].GetComponent<HandVRSphereHand>().Id = loop;
        }

        video.m_ClearMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/UnityARKitPlugin/Plugins/iOS/UnityARKit/Materials/YUVMaterial.mat");

        Undo.PerformUndo();
        Undo.PerformRedo();
    }
#endif

    [MenuItem("HandMR/iOS/Download Assets and Init Project with VR")]
    static void InitProjectForARKitVR()
    {
        initARKit();

        PlayerSettings.SetVirtualRealitySupported(BuildTargetGroup.iOS, true);
        PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.iOS, new string[] { "None", "cardboard" });
    }

    [MenuItem("HandMR/Android/Initialize Prefabs with VR")]
    static void InitPrefabsForVRARCore()
    {
#if DOWNLOADED_ARCORE
        string prefabPath = "Assets/HandMR/SubAssets/ARVR/ARCore/Prefabs/ARCoreDeviceVR.prefab";
        GameObject contentsRoot = PrefabUtility.LoadPrefabContents(prefabPath);
        ARCoreSession session = contentsRoot.GetComponent<ARCoreSession>();
        session.SessionConfig = AssetDatabase.LoadAssetAtPath<ARCoreSessionConfig>("Assets/GoogleARCore/Configurations/DefaultSessionConfig.asset");
        session.CameraConfigFilter = AssetDatabase.LoadAssetAtPath<ARCoreCameraConfigFilter>("Assets/GoogleARCore/Configurations/DefaultCameraConfigFilter.asset");
        PrefabUtility.SaveAsPrefabAsset(contentsRoot, prefabPath);
        PrefabUtility.UnloadPrefabContents(contentsRoot);
#endif
    }

    [MenuItem("HandMR/iOS/Initialize Prefabs with VR")]
    static void InitPrefabsForVRARKit()
    {
#if DOWNLOADED_ARKIT
        string prefabPath = "Assets/HandMR/SubAssets/ARVR/ARKit/Prefabs/CameraParent.prefab";
        GameObject contentsRoot = PrefabUtility.LoadPrefabContents(prefabPath);
        UnityARVideo video = contentsRoot.GetComponentInChildren<UnityARVideo>();
        video.m_ClearMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/UnityARKitPlugin/Plugins/iOS/UnityARKit/Materials/YUVMaterial.mat");
        PrefabUtility.SaveAsPrefabAsset(contentsRoot, prefabPath);
        PrefabUtility.UnloadPrefabContents(contentsRoot);
#endif
    }

    [MenuItem("HandMR/Android/Initialize Scene with VR")]
    [MenuItem("HandMR/iOS/Initialize Scene with VR")]
    static void InitSceneForVR()
    {
        if (Camera.main != null)
        {
            GameObject mainCamera = Camera.main.gameObject;
            Undo.RegisterCompleteObjectUndo(mainCamera, "Deactivate Camera");
            mainCamera.tag = "Untagged";
            mainCamera.SetActive(false);
        }

        GameObject arvrCamera = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HandMR/SubAssets/ARVR/All/Prefabs/ARVRCamera.prefab")) as GameObject;
        Undo.RegisterCreatedObjectUndo(arvrCamera, "Create object");

        GameObject handVRMain = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HandMR/SubAssets/HandVR/Prefabs/HandVR.prefab")) as GameObject;
        Undo.RegisterCreatedObjectUndo(handVRMain, "Create object");

        GameObject[] mrHandObjects = new GameObject[2];
        for (int loop = 0; loop < 2; loop++)
        {
            mrHandObjects[loop] = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HandMR/SubAssets/MRHand/Prefabs/MRHand.prefab")) as GameObject;
            Undo.RegisterCreatedObjectUndo(mrHandObjects[loop], "Create object");
        }

        SettingFromPlayerPrefsVR settingVR = Undo.AddComponent<SettingFromPlayerPrefsVR>(handVRMain);

        Undo.RegisterCompleteObjectUndo(new Object[] {
            settingVR,
            mrHandObjects[0].GetComponent<HandVRSphereHand>(),
            mrHandObjects[1].GetComponent<HandVRSphereHand>()
        }, "Initialize Scene with VR");

        settingVR.HandVRMainObj = handVRMain.GetComponent<HandVRMain>();

        for (int loop = 0; loop < 2; loop++)
        {
            mrHandObjects[loop].GetComponent<HandVRSphereHand>().Id = loop;
        }

        Undo.PerformUndo();
        Undo.PerformRedo();
    }
}
