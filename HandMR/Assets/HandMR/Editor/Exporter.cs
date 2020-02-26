using UnityEngine;
using System.Collections;
using UnityEditor;

public class Exporter
{
    [MenuItem("HandMR/Develop/Export with settings")]
    static void Export()
    {
        AssetDatabase.ExportPackage(new string[] { "Assets/HandMR", "Assets/Plugins", "Assets/StreamingAssets" },
            "HandMR.unitypackage",
            ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeLibraryAssets);
    }

    [MenuItem("HandMR/Develop/Export iOS assets")]
    static void ExportiOS()
    {
        AssetDatabase.ExportPackage(new string[] { "Assets/HandMR/SubAssets/HandVR/Plugins/iOS", "Assets/UnityARKitPlugin" },
            "HandMR_iOS_plugin_for_projects_0.x.unitypackage",
            ExportPackageOptions.Interactive | ExportPackageOptions.Recurse);
    }
}