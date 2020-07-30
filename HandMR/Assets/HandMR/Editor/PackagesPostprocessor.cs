using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
#if DOWNLOADED_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem;
#endif

public class PackagesPostprocessor : AssetPostprocessor
{
    class LoadPackages
    {
        class Package
        {
            public string PackageId;
            public bool Installed = false;
            public string Version;
            public string[] Versions = null;
        }

        List<Package> packageLists_ = new List<Package>();

        async void listPackageManager()
        {
            if (packageLists_.Count > 0)
            {
                return;
            }

            var listRequest = UnityEditor.PackageManager.Client.List();
            while (listRequest.Status == UnityEditor.PackageManager.StatusCode.InProgress)
            {
                await Task.Delay(16);
            }

            if (listRequest.Status == UnityEditor.PackageManager.StatusCode.Success)
            {
                foreach (var package in listRequest.Result)
                {
                    Package listPackage = new Package();
                    listPackage.PackageId = package.packageId.Split('@')[0];
                    listPackage.Version = package.version;
                    listPackage.Versions = package.versions.all;
                    packageLists_.Add(listPackage);
                }
            }
        }

        public async Task<bool> addPackageManager(string packageName, string version = "")
        {
            listPackageManager();

            Package newPackage = null;
            foreach (var package in packageLists_)
            {
                if (package.PackageId == packageName)
                {
                    if (package.Version == version || package.Installed)
                    {
                        return false;
                    }

                    newPackage = package;
                    break;
                }
            }
            if (newPackage == null)
            {
                newPackage = new Package();
                newPackage.PackageId = packageName;

                var searchRequest = UnityEditor.PackageManager.Client.Search(packageName);
                do
                {
                    await Task.Delay(16);
                } while (searchRequest.Status == UnityEditor.PackageManager.StatusCode.InProgress);
                var searchResult = searchRequest.Result;

                newPackage.Version = "0";
                if (searchResult != null && searchResult.Length > 0 && searchResult[0].versions != null && searchResult[0].versions.all != null)
                {
                    newPackage.Versions = searchResult[0].versions.all;
                }
                else
                {
                    newPackage.Versions = new string[0];
                }
            }

            string[] targetVersionStrings = version.Replace("-preview.", ".-1.").Split('.');
            int[] targetVersion = new int[targetVersionStrings.Length];
            for (int loop = 0; loop < targetVersionStrings.Length; loop++)
            {
                if (!int.TryParse(targetVersionStrings[loop], out targetVersion[loop]))
                {
                    targetVersion[loop] = -2;
                }
            }

            int[] newVersion = new int[0];
            string newVersionString = "";
            foreach (string newVersionStringInLoop in newPackage.Versions)
            {
                string[] versionStrings = newVersionStringInLoop.Replace("-preview.", ".-1.").Split('.');
                int[] versions = new int[versionStrings.Length];
                for (int loop = 0; loop < versionStrings.Length; loop++)
                {
                    if (!int.TryParse(versionStrings[loop], out versions[loop]))
                    {
                        versions[loop] = -2;
                    }
                }
                bool breakFlag = false;
                bool checkTarget = true;
                bool checkNew = true;
                for (int loop = 0; loop < versions.Length; loop++)
                {
                    if ((targetVersion.Length > loop && versions[loop] > targetVersion[loop] && checkTarget)
                        || (newVersion.Length > loop && newVersion[loop] > versions[loop] && checkNew))
                    {
                        breakFlag = true;
                        break;
                    }
                    if (targetVersion.Length > loop && versions[loop] < targetVersion[loop])
                    {
                        checkTarget = false;
                    }
                    if (newVersion.Length > loop && newVersion[loop] < versions[loop])
                    {
                        checkNew = false;
                    }
                }
                if (!breakFlag)
                {
                    newVersion = versions;
                    newVersionString = newVersionStringInLoop;
                }
            }

            if (newVersion.Length == 0)
            {
                return false;
            }

            if (newPackage.Version == newVersionString)
            {
                return false;
            }

            if (packageLists_.Contains(newPackage))
            {
                packageLists_.Remove(newPackage);
                var removeRequest = UnityEditor.PackageManager.Client.Remove(newPackage.PackageId + "@" + newPackage.Version);
                do
                {
                    await Task.Delay(16);
                } while (removeRequest.Status == UnityEditor.PackageManager.StatusCode.InProgress);
            }

            newPackage.Version = newVersionString;

            UnityEditor.PackageManager.Requests.AddRequest request;
            request = UnityEditor.PackageManager.Client.Add(newPackage.PackageId + "@" + newPackage.Version);
            do
            {
                await Task.Delay(16);
            } while (request.Status == UnityEditor.PackageManager.StatusCode.InProgress);
            if (request.Status != UnityEditor.PackageManager.StatusCode.Success)
            {
                EditorUtility.DisplayDialog("エラー", packageName + "を追加できませんでした", "OK");
                return false;
            }

            newPackage.Installed = true;
            packageLists_.Add(newPackage);

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
    static async void loadPackages()
    {
        LoadPackages loadPackage = new LoadPackages();

        bool isChange = false;

        isChange |= await loadPackage.addPackageManager("com.unity.xr.arfoundation", "3.0");
        isChange |= await loadPackage.addPackageManager("com.unity.xr.arsubsystems", "3.0");
        isChange |= await loadPackage.addPackageManager("com.unity.inputsystem", "1");
        isChange |= await loadPackage.addPackageManager("com.unity.xr.management", "3.0.3");

        isChange |= await loadPackage.addPackageManager("com.unity.xr.arcore", "3.0");
        isChange |= await loadPackage.addPackageManager("com.unity.xr.arkit", "3.0");

        isChange |= await loadPackage.addPackageManager("com.unity.xr.googlevr.android", "2.0.0");
        isChange |= await loadPackage.addPackageManager("com.unity.xr.googlevr.ios", "2.0.1");

        AssetDatabase.Refresh();
        while (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID("ProjectSettings/ProjectSettings.asset")))
        {
            await Task.Delay(16);
            AssetDatabase.Refresh();
        }
        setScriptingDefineSymbol("DOWNLOADED_ARFOUNDATION");

        if (isChange)
        {
            Debug.Log("パッケージを更新しました");
        }
    }

    async void OnPreprocessAsset()
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
                await Task.Delay(16);
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
        //aRPlaneManager.requestedDetectionMode = UnityEngine.XR.ARSubsystems.PlaneDetectionMode.Horizontal;

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
