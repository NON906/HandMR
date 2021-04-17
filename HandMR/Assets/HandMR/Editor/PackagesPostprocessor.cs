using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System;
#if DOWNLOADED_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine.XR.Management;
#endif

namespace HandMR
{
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

        public static void SetScriptingDefineSymbol(string symbol)
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

        static string deleteSymbol(string symbols, string symbol)
        {
            if (symbols.Contains(";" + symbol))
            {
                symbols = symbols.Replace(";" + symbol, "");
            }
            else if (symbols.Contains(symbol + ";"))
            {
                symbols = symbols.Replace(symbol + ";", "");
            }
            else
            {
                symbols = symbols.Replace(symbol, "");
            }

            return symbols;
        }

        static void deleteScriptingDefineSymbol(string symbol)
        {
            string symbols;

            symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            symbols = deleteSymbol(symbols, symbol);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, symbols);

            symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
            symbols = deleteSymbol(symbols, symbol);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, symbols);

            symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            symbols = deleteSymbol(symbols, symbol);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbols);
        }

        //[MenuItem("HandMR/Add Packages to PackageManager")]
        static void loadPackages()
        {
            LoadPackages loadPackage = new LoadPackages();

            bool isChange = false;

            isChange |= loadPackage.addPackageManager("com.unity.xr.management", "4.0.1");
            isChange |= loadPackage.addPackageManager("com.unity.xr.arfoundation", "4.0.12");
            isChange |= loadPackage.addPackageManager("com.unity.xr.arsubsystems", "4.0.12");

            isChange |= loadPackage.addPackageManager("com.unity.xr.arcore", "4.0.12");
            isChange |= loadPackage.addPackageManager("com.unity.xr.arkit", "4.0.12");

            AssetDatabase.Refresh();
            while (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID("ProjectSettings/ProjectSettings.asset")))
            {
                System.Threading.Thread.Sleep(16);
                AssetDatabase.Refresh();
            }
            SetScriptingDefineSymbol("DOWNLOADED_ARFOUNDATION");
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

            if (!PlayerSettings.allowUnsafeCode)
            {
                PlayerSettings.allowUnsafeCode = true;
                isChange = true;
            }
            SetScriptingDefineSymbol("ENABLE_UNSAFE_CODE");

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

            isChange |= settingLayers(new string[] { "BackGround", "ControlObject", "PlaySpaceWall" }, new int[] { 20, 21, 22 });

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

        [MenuItem("Tools/HandMR/Show Start Dialog Window")]
        static void showDialogWindow()
        {
            bool isOpenWindow = false;

            foreach (var window in Resources.FindObjectsOfTypeAll<EditorWindow>())
            {
                if (window.GetType() == typeof(DialogWindow))
                {
                    window.Focus();
                    isOpenWindow = true;
                }
            }

            if (!isOpenWindow)
            {
                EditorWindow.GetWindow<DialogWindow>(true, "HandMR", true);
            }
        }

        [InitializeOnLoadMethod]
        static void initOnLoad()
        {
#if !CLOSE_DIALOG_WINDOW
#if UNITY_2021_1_OR_NEWER
            Debug.Log("This Unity version is not supported auto open window. Please open window from 'Tools/HandMR/Show Start Dialog Window'.");
#else
            System.Threading.Thread.Sleep(1000);
            showDialogWindow();
#endif
#endif
        }

        public class DialogWindow : EditorWindow
        {
            ChangeLanguage.Languages lang_;
            bool isNotShowAgain_;

            void Awake()
            {
#if LANG_JP
                lang_ = ChangeLanguage.Languages.Japanese;
#else
                lang_ = ChangeLanguage.Languages.English;
#endif

#if CLOSE_DIALOG_WINDOW
                isNotShowAgain_ = true;
#else
                isNotShowAgain_ = false;
#endif
            }

            void OnGUI()
            {
                var currentPosition = position;
                currentPosition.width = 350f;
                currentPosition.height = 500f;
                position = currentPosition;

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
                GUILayout.Label("  2. Put check(s) ARCore (for Android) and/or ARKit (for iOS).");
                if (GUILayout.Button("Open Project Settings"))
                {
                    EditorApplication.ExecuteMenuItem("Edit/Project Settings...");
                }

                GUILayout.Label("Step 4. Copy Setting Files for Android Plugins");
                if (GUILayout.Button("Execute"))
                {
                    if (!Directory.Exists("Assets/Plugins"))
                    {
                        AssetDatabase.CreateFolder("Assets", "Plugins");
                    }
                    if (!Directory.Exists("Assets/Plugins/Android"))
                    {
                        AssetDatabase.CreateFolder("Assets/Plugins", "Android");
                    }
                    AssetDatabase.CopyAsset("Assets/HandMR/Plugins/Android/mainTemplate.gradle", "Assets/Plugins/Android/mainTemplate.gradle");
                    AssetDatabase.CopyAsset("Assets/HandMR/Plugins/Android/gradleTemplate.properties", "Assets/Plugins/Android/gradleTemplate.properties");
                    Debug.Log("Finish copy files.");
                }

                GUILayout.Label("Step 5. Download and Install iOS Plugins");
                if (GUILayout.Button("Execute"))
                {
                    HandMRSceneInitializer.InitProjectForIOS();
                }

                GUILayout.Label("Step 6. Select Languages");
                var newLang = (ChangeLanguage.Languages)EditorGUILayout.EnumPopup(lang_);
                if (newLang != lang_)
                {
                    ChangeLanguage.Change(lang_, newLang);
                    lang_ = newLang;
                }

                GUILayout.Label("Step 7. Debug Settings (Optional)");
                GUILayout.Label("  1. Install 'AR Foundation Editor Remote'.");
                if (GUILayout.Button("Go to Asset Store Page"))
                {
                    Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/ar-foundation-editor-remote-168773");
                }
                GUILayout.Label("  2. Copy model files for debugging.");
                if (GUILayout.Button("Execute"))
                {
                    Directory.CreateDirectory("mediapipe/modules/hand_landmark");
                    File.Copy(Application.dataPath + "/HandMR/SubAssets/HandVR/EditorModels/mediapipe/modules/hand_landmark/hand_landmark.tflite",
                        "mediapipe/modules/hand_landmark/hand_landmark.tflite", true);
                    File.Copy(Application.dataPath + "/HandMR/SubAssets/HandVR/EditorModels/mediapipe/modules/hand_landmark/handedness.txt",
                        "mediapipe/modules/hand_landmark/handedness.txt", true);
                    Directory.CreateDirectory("mediapipe/modules/palm_detection");
                    File.Copy(Application.dataPath + "/HandMR/SubAssets/HandVR/EditorModels/mediapipe/modules/palm_detection/palm_detection.tflite",
                        "mediapipe/modules/palm_detection/palm_detection.tflite", true);
                    Debug.Log("Finish copy files.");
                }

                GUILayout.Space(20);

                isNotShowAgain_ = GUILayout.Toggle(isNotShowAgain_, "Do not show again");

                if (GUILayout.Button("Close"))
                {
                    Close();
                }
            }

            void OnDestroy()
            {
                if (isNotShowAgain_)
                {
                    SetScriptingDefineSymbol("CLOSE_DIALOG_WINDOW");
                }
                else
                {
                    deleteScriptingDefineSymbol("CLOSE_DIALOG_WINDOW");
                }
            }
        }
    }
}
