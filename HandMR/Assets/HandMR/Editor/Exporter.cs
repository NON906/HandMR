using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class Exporter
{
    static string[] getPathsExcept(string currentDir, string exceptDir)
    {
        while (exceptDir.EndsWith("/"))
        {
            exceptDir = exceptDir.Substring(0, exceptDir.Length - 1);
        }

        List<string> paths = new List<string>();
        string[] splitExceptDir = exceptDir.Split('/');
        string currentExceptDir = currentDir + "/" + splitExceptDir[0];
        for (int loop = 0; loop < splitExceptDir.Length; loop++)
        {
            string[] dirs = Directory.GetDirectories(currentDir, "*", SearchOption.TopDirectoryOnly);
            foreach (string dir in dirs)
            {
                string newDir = dir;
                while (newDir.EndsWith("/"))
                {
                    newDir = newDir.Substring(0, newDir.Length - 1);
                }
                if (newDir != currentExceptDir)
                {
                    paths.Add(newDir);
                }
            }

            if (loop < splitExceptDir.Length - 1)
            {
                currentDir = currentExceptDir;
                currentExceptDir += "/" + splitExceptDir[loop + 1];
            }
        }

        return paths.ToArray();
    }

    [MenuItem("HandMR/Develop/Export with settings")]
    static void Export()
    {
        string symbols;
        symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        symbols = symbols.Replace(";DOWNLOADED_ARFOUNDATION", "");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, symbols);
        symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
        symbols = symbols.Replace(";DOWNLOADED_ARFOUNDATION", "");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, symbols);

        AssetDatabase.ExportPackage(getPathsExcept("Assets", "HandMR/SubAssets/HandVR/Plugins/iOS"),
            "HandMR_0.x.unitypackage",
            ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeLibraryAssets);
    }

    [MenuItem("HandMR/Develop/Export iOS assets")]
    static void ExportiOS()
    {
        AssetDatabase.ExportPackage(new string[] { "Assets/HandMR/SubAssets/HandVR/Plugins/iOS" },
            "HandMR_iOS_plugin_for_projects_0.x.unitypackage",
            ExportPackageOptions.Interactive | ExportPackageOptions.Recurse);
    }
}
