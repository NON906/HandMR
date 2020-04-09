using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

public class PackagesPostprocessor : AssetPostprocessor
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

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (importedAssets.ToList().Contains("Assets/HandMR/Editor/PackagesPostprocessor.cs"))
        {
            addPackageManager("com.unity.xr.arfoundation");
            addPackageManager("com.unity.xr.arsubsystems");
            addPackageManager("com.unity.inputsystem");
            addPackageManager("com.unity.xr.management");

            addPackageManager("com.unity.xr.arcore@3.0.1");
            addPackageManager("com.unity.xr.arkit@3.0.1");

            addPackageManager("com.unity.xr.googlevr.android@2.0.0");
            addPackageManager("com.unity.xr.googlevr.ios@2.0.1");

            Debug.Log("パッケージを更新しました");

            setScriptingDefineSymbol("DOWNLOADED_ARFOUNDATION");
        }
    }
}
