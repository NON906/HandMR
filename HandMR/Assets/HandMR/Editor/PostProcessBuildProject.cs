using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace HandMR
{
	public static class PostProcessBuildProject
	{
		[PostProcessBuild(100)]
		public static void OnPostProcessBuild(BuildTarget target, string path)
		{
			if (target == BuildTarget.iOS)
			{
				postProcessBuildiOS(path);
			}
		}

		static void postProcessBuildiOS(string path)
		{
			string projectPath = PBXProject.GetPBXProjectPath(path);
			PBXProject pbxProject = new PBXProject();

			pbxProject.ReadFromString(File.ReadAllText(projectPath));

			string target = pbxProject.GetUnityFrameworkTargetGuid();
			string mainTarget = pbxProject.GetUnityMainTargetGuid();

			string libGuid = pbxProject.FindFileGuidByRealPath("Libraries/HandMR/SubAssets/HandVR/Plugins/iOS/MultiHandAppLib-fl.a");
			pbxProject.RemoveFile(libGuid);
			pbxProject.RemoveFileFromBuild(target, libGuid);
			pbxProject.RemoveFrameworkFromProject(target, libGuid);
			pbxProject.AddBuildProperty(target, "OTHER_LDFLAGS_FRAMEWORK", "-force_load Libraries/HandMR/SubAssets/HandVR/Plugins/iOS/MultiHandAppLib-fl.a");

			string[] files = Directory.GetFiles(Path.Combine(Application.dataPath, "HandMR/iOS_assets"));
			foreach (string file in files)
			{
				if (file.EndsWith(".meta") || file == Path.Combine(Application.dataPath, "HandMR/iOS_assets/LICENSE"))
				{
					continue;
				}

				string fileGuid = pbxProject.AddFile(file, Path.Combine("UnityFramework", Path.GetFileName(file)));
				pbxProject.AddFileToBuild(target, fileGuid);
				pbxProject.AddFileToBuild(mainTarget, fileGuid);
			}

			File.WriteAllText(projectPath, pbxProject.WriteToString());
		}
	}
}
