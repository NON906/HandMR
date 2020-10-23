using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
#if DOWNLOADED_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine.XR.Management;
#endif

public class PackagesPostprocessor
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
                    Package listPackage = new Package();
                    listPackage.PackageId = package.packageId.Split('@')[0];
                    listPackage.Version = package.version;
                    listPackage.Versions = package.versions.all;
                    packageLists_.Add(listPackage);
                }
            }
        }

        public bool addPackageManager(string packageName, string version = "")
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
                    System.Threading.Thread.Sleep(16);
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
                    System.Threading.Thread.Sleep(16);
                } while (removeRequest.Status == UnityEditor.PackageManager.StatusCode.InProgress);
            }

            newPackage.Version = newVersionString;

            UnityEditor.PackageManager.Requests.AddRequest request;
            request = UnityEditor.PackageManager.Client.Add(newPackage.PackageId + "@" + newPackage.Version);
            do
            {
                System.Threading.Thread.Sleep(16);
            } while (request.Status == UnityEditor.PackageManager.StatusCode.InProgress);
            if (request.Status != UnityEditor.PackageManager.StatusCode.Success)
            {
                EditorUtility.DisplayDialog("Error", "Can't add " + packageName, "OK");
                return false;
            }

            newPackage.Installed = true;
            packageLists_.Add(newPackage);

            return true;
        }
    }

    static DialogWindow window_ = null;

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

        symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        if (!symbols.Contains(symbol))
        {
            if (symbols == null || symbols == "")
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbol);
            }
            else
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbols + ";" + symbol);
            }
        }
    }

    //[MenuItem("HandMR/Add Packages to PackageManager")]
    static void loadPackages()
    {
        LoadPackages loadPackage = new LoadPackages();

        bool isChange = false;

        isChange |= loadPackage.addPackageManager("com.unity.xr.management", "3.2");
        isChange |= loadPackage.addPackageManager("com.unity.xr.arfoundation", "4.0");
        isChange |= loadPackage.addPackageManager("com.unity.xr.arsubsystems", "4.0");

        isChange |= loadPackage.addPackageManager("com.unity.xr.arcore", "4.0");
        isChange |= loadPackage.addPackageManager("com.unity.xr.arkit", "4.0");

        AssetDatabase.Refresh();
        while (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID("ProjectSettings/ProjectSettings.asset")))
        {
            System.Threading.Thread.Sleep(16);
            AssetDatabase.Refresh();
        }
        setScriptingDefineSymbol("DOWNLOADED_ARFOUNDATION");
        AssetDatabase.SaveAssets();

        if (isChange)
        {
            Debug.Log("Add packages is finished.");
        }
    }

    static void settingPrefab()
    {
#if DOWNLOADED_ARFOUNDATION
        string prefabPath = "Assets/HandMR/SubAssets/ARVR/Prefabs/ManagerAndSession.prefab";
        GameObject contentsRoot = PrefabUtility.LoadPrefabContents(prefabPath);
        ARSessionOrigin aRSessionOrigin = contentsRoot.GetComponent<ARSessionOrigin>();
        aRSessionOrigin.camera = contentsRoot.GetComponentInChildren<Camera>();
        ARPlaneManager aRPlaneManager = contentsRoot.GetComponent<ARPlaneManager>();
        aRPlaneManager.planePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HandMR/SubAssets/ARVR/Prefabs/Sub/ARPlane.prefab");
        //aRPlaneManager.detectionMode = UnityEngine.XR.ARSubsystems.PlaneDetectionMode.Horizontal;
        aRPlaneManager.requestedDetectionMode = UnityEngine.XR.ARSubsystems.PlaneDetectionMode.Horizontal;

        PrefabUtility.SaveAsPrefabAsset(contentsRoot, prefabPath);
        PrefabUtility.UnloadPrefabContents(contentsRoot);
#endif
    }

    static bool settingProject()
    {
        bool isChange = false;

        var version = PlayerSettings.Android.minSdkVersion;
        if (version == AndroidSdkVersions.AndroidApiLevelAuto || version < AndroidSdkVersions.AndroidApiLevel24)
        {
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            isChange = true;
        }
        version = PlayerSettings.Android.targetSdkVersion;
        if (version == AndroidSdkVersions.AndroidApiLevelAuto || version < AndroidSdkVersions.AndroidApiLevel29)
        {
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
            isChange = true;
        }
        if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android) != ScriptingImplementation.IL2CPP)
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            isChange = true;
        }
        if (PlayerSettings.Android.targetArchitectures != AndroidArchitecture.ARM64)
        {
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            isChange = true;
        }
        var graphicsApi = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
        if (graphicsApi.Length != 1 || graphicsApi[0] != UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3)
        {
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android,
                new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 });
            isChange = true;
        }
        if (PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Android) != ApiCompatibilityLevel.NET_4_6)
        {
            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_4_6);
            isChange = true;
        }

        string iosVersion = PlayerSettings.iOS.targetOSVersionString;
        string[] iosVersionSplit = iosVersion.Split('.');
        int iosVersionMajor;
        if (iosVersionSplit.Length <= 0 || !int.TryParse(iosVersionSplit[0], out iosVersionMajor))
        {
            iosVersionMajor = -1;
        }
        int iosVersionMinor;
        if (iosVersionSplit.Length <= 1 || !int.TryParse(iosVersionSplit[1], out iosVersionMinor))
        {
            iosVersionMinor = -1;
        }
        if (iosVersionMajor < 11 || (iosVersionMajor == 11 && iosVersionMinor < 0))
        {
            PlayerSettings.iOS.targetOSVersionString = "11.0";
            isChange = true;
        }
        if (PlayerSettings.GetArchitecture(BuildTargetGroup.iOS) != 1)
        {
            PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 1);
            isChange = true;
        }
        if (PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.iOS) != ApiCompatibilityLevel.NET_4_6)
        {
            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.iOS, ApiCompatibilityLevel.NET_4_6);
            isChange = true;
        }

        if (PlayerSettings.defaultInterfaceOrientation != UIOrientation.LandscapeLeft)
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            isChange = true;
        }

        if (isChange)
        {
            AssetDatabase.SaveAssets();
        }

        return isChange;
    }

    static bool settingLayers(string[] layerNames, int[] defaultLayersNo)
    {
        bool isChange = false;

        string[] managerText = File.ReadAllLines("ProjectSettings/TagManager.asset");
        int managerTextLayersStart = 0;
        foreach (string line in managerText)
        {
            if (line == "  layers:")
            {
                managerTextLayersStart++;
                break;
            }
            managerTextLayersStart++;
        }

        for (int loop = 0; loop < layerNames.Length; loop++)
        {
            if (LayerMask.NameToLayer(layerNames[loop]) >= 0)
            {
                continue;
            }

            if (string.IsNullOrEmpty(LayerMask.LayerToName(defaultLayersNo[loop])))
            {
                managerText[managerTextLayersStart + defaultLayersNo[loop]] = "  - " + layerNames[loop];
                isChange = true;
                continue;
            }

            for (int loop2 = managerTextLayersStart + 8; loop2 < managerTextLayersStart + 32; loop2++)
            {
                if (managerText[loop2] == "  - ")
                {
                    managerText[loop2] = "  - " + layerNames[loop];
                    isChange = true;
                    break;
                }
            }
        }

        if (isChange)
        {
            File.WriteAllLines("ProjectSettings/TagManager.asset", managerText);
            AssetDatabase.Refresh();
        }

        return isChange;
    }

    static bool settings()
    {
#if DOWNLOADED_ARFOUNDATION
        bool isChange = false;

        isChange |= settingProject();

        isChange |= settingLayers(new string[] { "BackGround", "ControlObject" }, new int[] { 20, 21 });

        return isChange;
#else
        return false;
#endif
    }

    //[MenuItem("HandMR/Setting Project, Layers and Prefabs")]
    static void initSettings()
    {
#if DOWNLOADED_ARFOUNDATION
        settings();
        settingPrefab();
        Debug.Log("Setting is finished.");
#endif
    }

    [MenuItem("HandMR/Show Start Dialog Window")]
    static void showDialogWindow()
    {
        if (window_ == null)
        {
            window_ = EditorWindow.GetWindow<DialogWindow>(true, "HandMR");
            window_.Show();
        }
        if (!window_.hasFocus)
        {
            window_.Focus();
        }
    }

    [InitializeOnLoadMethod]
    static void initOnLoad()
    {
#if !CLOSE_DIALOG_WINDOW
        System.Threading.Thread.Sleep(1000);
        showDialogWindow();
#endif
    }

    public class DialogWindow : EditorWindow
    {
        void OnGUI()
        {
            GUILayout.Label("Step 1. Add Packages to PackageManager");
            if (GUILayout.Button("Execute"))
            {
                loadPackages();
            }

            GUILayout.Label("Step 2. Setting Project, Layers and Prefabs");
            if (GUILayout.Button("Execute"))
            {
                initSettings();
            }

            GUILayout.Label("Step 3. Setting XR");
            GUILayout.Label("  1. Open Project Settings -> XR-Plugin Management.");
            GUILayout.Label("  2. Check ARCore (for Android) and/or ARKit (for iOS).");
            if (GUILayout.Button("Open Project Settings"))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings...");
            }

            GUILayout.Label("Step 4. Download And Install iOS Plugins");
            if (GUILayout.Button("Execute"))
            {
                HandMRSceneInitializer.InitProjectForIOS();
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Close"))
            {
                Close();
            }
        }

        void OnDestroy()
        {
            setScriptingDefineSymbol("CLOSE_DIALOG_WINDOW");
            window_ = null;
        }
    }
}
