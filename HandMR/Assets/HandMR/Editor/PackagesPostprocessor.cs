using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
#if DOWNLOADED_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem;
#endif

public class PackagesPostprocessor : AssetPostprocessor
{
    class LoadPackages
    {
        List<string> packageLists_ = new List<string>();

        void listPackageManager()
        {
            if (packageLists_.Count > 0)
            {
                return;
            }

            var listRequest = UnityEditor.PackageManager.Client.List();
            while (listRequest.Status == UnityEditor.PackageManager.StatusCode.InProgress)
            {
                System.Threading.Thread.Sleep(16);
            }

            if (listRequest.Status == UnityEditor.PackageManager.StatusCode.Success)
            {
                foreach (var package in listRequest.Result)
                {
                    packageLists_.Add(package.packageId);
                }
            }
        }

        public bool addPackageManager(string packageName)
        {
            listPackageManager();
            foreach (string name in packageLists_)
            {
                if (name.Contains(packageName))
                {
                    return false;
                }
            }

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

            packageLists_.Add(packageName);

            return true;
        }
    }

    static bool isLoading_ = false;
    static bool isLoaded_ = false;

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

    [MenuItem("HandMR/Add Packages to PackageManager")]
    static void loadPackages()
    {
        LoadPackages loadPackage = new LoadPackages();

        bool isChange = false;

        isChange |= loadPackage.addPackageManager("com.unity.xr.arfoundation");
        isChange |= loadPackage.addPackageManager("com.unity.xr.arsubsystems");
        isChange |= loadPackage.addPackageManager("com.unity.inputsystem");
        isChange |= loadPackage.addPackageManager("com.unity.xr.management");

        isChange |= loadPackage.addPackageManager("com.unity.xr.arcore@3.0.1");
        isChange |= loadPackage.addPackageManager("com.unity.xr.arkit@3.0.1");

        isChange |= loadPackage.addPackageManager("com.unity.xr.googlevr.android@2.0.0");
        isChange |= loadPackage.addPackageManager("com.unity.xr.googlevr.ios@2.0.1");

        AssetDatabase.Refresh();
        while (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID("ProjectSettings/ProjectSettings.asset")))
        {
            System.Threading.Thread.Sleep(16);
            AssetDatabase.Refresh();
        }
        setScriptingDefineSymbol("DOWNLOADED_ARFOUNDATION");

        if (isChange)
        {
            Debug.Log("パッケージを更新しました");
        }
    }

    void OnPreprocessAsset()
    {
        if (assetPath == "ProjectSettings/ProjectSettings.asset")
        {
            return;
        }

        if (isLoaded_)
        {
            return;
        }

        if (isLoading_)
        {
            while (isLoading_)
            {
                System.Threading.Thread.Sleep(16);
            }
            return;
        }

        isLoading_ = true;

        loadPackages();

        isLoaded_ = true;
        isLoading_ = false;
    }

    [MenuItem("HandMR/Setting Prefabs")]
    static void settingPrefab()
    {
#if DOWNLOADED_ARFOUNDATION
        string prefabPath = "Assets/HandMR/SubAssets/ARVR/Prefabs/ManagerAndSession.prefab";
        GameObject contentsRoot = PrefabUtility.LoadPrefabContents(prefabPath);
        ARSessionOrigin aRSessionOrigin = contentsRoot.GetComponent<ARSessionOrigin>();
        aRSessionOrigin.camera = contentsRoot.GetComponentInChildren<Camera>();
        ARPlaneManager aRPlaneManager = contentsRoot.GetComponent<ARPlaneManager>();
        aRPlaneManager.planePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HandMR/SubAssets/ARVR/Prefabs/Sub/ARPlane.prefab");
        aRPlaneManager.detectionMode = UnityEngine.XR.ARSubsystems.PlaneDetectionMode.Horizontal;

        TrackedPoseDriver trackedPoseDriver = contentsRoot.GetComponentInChildren<TrackedPoseDriver>();
        trackedPoseDriver.positionAction = new InputAction(binding: "<HandheldARInputDevice>/devicePosition");
        trackedPoseDriver.rotationAction = new InputAction(binding: "<HandheldARInputDevice>/deviceRotation");

        PrefabUtility.SaveAsPrefabAsset(contentsRoot, prefabPath);
        PrefabUtility.UnloadPrefabContents(contentsRoot);

        Debug.Log("プレハブファイルの設定を行いました");
#endif
    }

    [InitializeOnLoadMethod]
    static void initOnLoad()
    {
#if DOWNLOADED_ARFOUNDATION && !SETTING_PREFAB
        settingPrefab();
        setScriptingDefineSymbol("SETTING_PREFAB");
#endif
    }
}
