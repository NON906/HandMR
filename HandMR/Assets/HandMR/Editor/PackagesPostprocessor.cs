using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

public class PackagesPostprocessor : AssetPostprocessor
{
    static bool isLoading_ = false;
    static bool isLoaded_ = false;

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

    bool addPackageManager(string packageName)
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

    void OnPreprocessAsset()
    {
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

        bool isChange = false;

        isChange |= addPackageManager("com.unity.xr.arfoundation");
        isChange |= addPackageManager("com.unity.xr.arsubsystems");
        isChange |= addPackageManager("com.unity.inputsystem");
        isChange |= addPackageManager("com.unity.xr.management");

        isChange |= addPackageManager("com.unity.xr.arcore@3.0.1");
        isChange |= addPackageManager("com.unity.xr.arkit@3.0.1");

        isChange |= addPackageManager("com.unity.xr.googlevr.android@2.0.0");
        isChange |= addPackageManager("com.unity.xr.googlevr.ios@2.0.1");

        if (isChange)
        {
            AssetDatabase.Refresh();
            Debug.Log("パッケージを更新しました");
        }

        isLoaded_ = true;
        isLoading_ = false;
    }
}
