using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.IO;
using System;
using System.Linq;
#if DOWNLOADED_HOLOGLA
using Hologla;
#endif

public class HandMRSceneInitializer
{
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

    [MenuItem("HandMR/Download iOS Plugins")]
    static void InitProjectForIOS()
    {
        if (!Directory.Exists(Application.dataPath + "/HandMR/SubAssets/HandVR/Plugins/iOS"))
        {
            if (!Directory.Exists(Application.dataPath + "/../../downloads"))
            {
                Directory.CreateDirectory(Application.dataPath + "/../../downloads");
            }

            download("https://github.com/NON906/HandMR/releases/download/0.9/HandMR_iOS_plugin_for_projects_0.9.unitypackage", Application.dataPath + "/../../downloads/HandMR_iOS_plugin_for_projects_0.9.unitypackage");
            AssetDatabase.ImportPackage(Application.dataPath + "/../../downloads/HandMR_iOS_plugin_for_projects_0.9.unitypackage", false);
        }
    }

    [MenuItem("HandMR/Initialize Scene")]
    static void InitScene()
    {
        if (Camera.main != null)
        {
            Undo.RecordObject(Camera.main.gameObject, "Deactivate Camera");
            Camera.main.gameObject.SetActive(false);
        }

        GameObject handmrObject = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HandMR/Prefabs/HandMRManager.prefab")) as GameObject;
        Undo.RegisterCreatedObjectUndo(handmrObject, "Create object");

        Undo.PerformUndo();
        Undo.PerformRedo();
    }
}
