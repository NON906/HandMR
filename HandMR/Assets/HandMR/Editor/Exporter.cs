using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace HandMR
{
#if HANDMR_DEVELOP
    public class Exporter
    {
        public static string[] GetPathsExcept(string currentDir, string exceptDir)
        {
            if (!exceptDir.StartsWith(currentDir))
            {
                return new string[] { currentDir };
            }

            if (currentDir.StartsWith(exceptDir))
            {
                return new string[] { };
            }

            if (!AssetDatabase.IsValidFolder(currentDir))
            {
                if (currentDir != exceptDir)
                {
                    return new string[] { currentDir };
                }
                else
                {
                    return new string[] { };
                }
            }

            var re = new Regex(currentDir);
            exceptDir = re.Replace(exceptDir, "", 1);
            while (exceptDir.StartsWith("/"))
            {
                exceptDir = exceptDir.Substring(1, exceptDir.Length - 1);
            }

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
                    string newDir = dir.Replace("\\", "/");
                    while (newDir.EndsWith("/"))
                    {
                        newDir = newDir.Substring(0, newDir.Length - 1);
                    }
                    if (newDir != currentExceptDir)
                    {
                        paths.Add(newDir);
                    }
                }

                string[] files = Directory.GetFiles(currentDir, "*", SearchOption.TopDirectoryOnly);
                foreach (string file in files)
                {
                    string newFile = file.Replace("\\", "/");
                    if (newFile != currentExceptDir)
                    {
                        paths.Add(newFile);
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

        public static string[] GetPathsExcept(string[] currentDirs, string[] exceptDirs)
        {
            foreach (string exceptDir in exceptDirs)
            {
                List<string> dirsList = new List<string>();
                foreach (string currentDir in currentDirs)
                {
                    dirsList.AddRange(GetPathsExcept(currentDir, exceptDir));
                }
                currentDirs = dirsList.ToArray();
            }
            return currentDirs;
        }

        [MenuItem("Tools/HandMR/Develop/Export")]
        static void Export()
        {
            AssetDatabase.ExportPackage(
                GetPathsExcept(new string[] { "Assets/HandMR" },
                new string[] { "Assets/HandMR/iOS_assets", "Assets/HandMR/SubAssets/HandVR/Plugins/iOS", "Assets/HandMR/Sample/HandFireBall" }),
                "HandMR_0.x.unitypackage",
                ExportPackageOptions.Interactive | ExportPackageOptions.Recurse);
        }

        [MenuItem("Tools/HandMR/Develop/Export with Sample")]
        static void ExportWithSample()
        {
            AssetDatabase.ExportPackage(
                GetPathsExcept(new string[] { "Assets/HandMR" },
                new string[] { "Assets/HandMR/iOS_assets", "Assets/HandMR/SubAssets/HandVR/Plugins/iOS" }),
                "HandMR_Sample_0.x.unitypackage",
                ExportPackageOptions.Interactive | ExportPackageOptions.Recurse);
        }

        [MenuItem("Tools/HandMR/Develop/Export iOS assets")]
        static void ExportiOS()
        {
            AssetDatabase.ExportPackage(new string[] { "Assets/HandMR/SubAssets/HandVR/Plugins/iOS", "Assets/HandMR/iOS_assets" },
                "HandMR_iOS_plugin_for_projects_0.x.unitypackage",
                ExportPackageOptions.Interactive | ExportPackageOptions.Recurse);
        }
    }
#endif
}
