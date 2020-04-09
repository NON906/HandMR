#undef UNITY_ANDROID
#undef UNITY_IOS

using Hologla;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Events;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.XR.iOS;
#elif UNITY_ANDROID
using UnityEngine.SpatialTracking;
using UnityEngine.UI;
#endif
using static UnityEditor.AssetDatabase;
using static UnityEditor.PrefabUtility;

public class SceneInitializeMenu
{
	const string HOLOGLA_CAMERA_PARENT_PATH = "Assets/Hologla/Prefabs/HologlaCameraParent.prefab";
	const string HOLOGLA_INPUT_PATH = "Assets/Hologla/Prefabs/HologlaInput.prefab";
//	const string PLAYMENU_PATH = "Assets/Hologla/Prefabs/Samples/PlayMenu.prefab";
	const string HOLOGLA_YUV_MATERIAL_PATH = "Assets/Hologla/Materials/HologlaYUVMaterial.mat";

	const string AR_CORE_SESSION_PREFAB_PATH = "Assets/GoogleARCore/Prefabs/ARCore Device.prefab";

	const string HOLOGLA_AR_CORE_AR_MATERIAL_PATH = "Assets/GoogleARCore/SDK/Materials/ARBackground.mat";

	const string AR_KIT_CAMERA_USAGE_DESCRIPTION = "ARKit";

	[MenuItem("Hologla/ARKit/Initialize Project with ARKit")]
	static void InitProjectForARKit()
	{
		//ARKitプラグインがないままぷiOSプラットフォームにするとスクリプトにエラーが出るので、最初にプラグインの有無をチェックする.
		if (false == Directory.Exists(Application.dataPath + "/UnityARKitPlugin"))
		{
			EditorUtility.DisplayDialog("エラー", "ARKitのプラグインがインポートされていないため、処理を中断しました。", "OK");

			return;
		}

		//プラットフォームがiOSになっていない場合は切り替える.
#if !UNITY_IOS
		EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildTargetGroup.iOS, BuildTarget.iOS);
#endif
#if UNITY_2017_1_OR_NEWER
		//カメラを使用するための表記が設定されていない場合は適当に設定.
		if( 0 == PlayerSettings.iOS.cameraUsageDescription.Length ){
			PlayerSettings.iOS.cameraUsageDescription = AR_KIT_CAMERA_USAGE_DESCRIPTION;
		}
#endif

		return;
	}

	[MenuItem("Hologla/ARKit/Initialize Scene with ARKit")]
	static void InitSceneForARKit()
	{
		//ARKitプラグインがないままぷiOSプラットフォームにするとスクリプトにエラーが出るので、最初にプラグインの有無をチェックする.
		if (false == Directory.Exists(Application.dataPath + "/UnityARKitPlugin"))
		{
			EditorUtility.DisplayDialog("エラー", "ARKitのプラグインがインポートされていないため、処理を中断しました。", "OK");

			return;
		}

		//デフォルトのMainCameraをDeactivate
		if (Camera.main != null)
		{
			Undo.RecordObject(Camera.main.gameObject, "Deactivate Camera");
			Camera.main.gameObject.SetActive(false);
		}

		//Hologlaのオブジェクト群を配置
		GameObject hologlaCameraParentRoot = InstantiatePrefab(LoadAssetAtPath<GameObject>(HOLOGLA_CAMERA_PARENT_PATH)) as GameObject;
		Undo.RegisterCreatedObjectUndo(hologlaCameraParentRoot, "Create " + hologlaCameraParentRoot.name);
		GameObject hologlaInput = InstantiatePrefab(LoadAssetAtPath<GameObject>(HOLOGLA_INPUT_PATH)) as GameObject;
		Undo.RegisterCreatedObjectUndo(hologlaInput, "Create " + hologlaInput.name);
//		var playMenu = InstantiatePrefab(LoadAssetAtPath<GameObject>(PLAYMENU_PATH)) as GameObject;
//		Undo.RegisterCreatedObjectUndo(playMenu, "Create " + playMenu.name);

		//ルートのHologlaCameraParentの子のHologlaCameraを取得(HologlaCameraManagerが付いてる方).
		GameObject hologlaCameraParent = hologlaCameraParentRoot.transform.Find("HologlaCamera").gameObject;

		//ARCameraManagerを配置
#if UNITY_IOS
        GameObject arCameraManager = new GameObject("ARCameraManager");
        UnityARCameraManager unityARCameraManager = arCameraManager.AddComponent<UnityARCameraManager>( );
		unityARCameraManager.m_camera = hologlaCameraParent.GetComponent<Camera>( );
        unityARCameraManager.planeDetection = UnityARPlaneDetection.Horizontal;
		Undo.RegisterCreatedObjectUndo(arCameraManager, "Create " + arCameraManager.name);
#endif

		//HologlaCameraManagerのArBackgroundMaterialにHologlaYUVMaterialを設定
		Material hologlaYuvMaterial = LoadAssetAtPath<Material>(HOLOGLA_YUV_MATERIAL_PATH);
		HologlaCameraManager hologlaCameraManager = hologlaCameraParent.GetComponent<HologlaCameraManager>();
		hologlaCameraManager.arBackgroundMaterial = hologlaYuvMaterial;

		ApplyHologlaInputSetting(hologlaCameraManager, hologlaInput.GetComponent<HologlaInput>( ));

		return;
	}



	[MenuItem("Hologla/ARCore/Initialize Project with ARCore")]
	static void InitProjectForARCore( )
	{
		//ARCoreプラグインがないままぷAndroidプラットフォームにするとスクリプトにエラーが出るので、最初にプラグインの有無をチェックする.
		if( Directory.Exists(Application.dataPath + "/GoogleARCore") == false ){
			EditorUtility.DisplayDialog("エラー", "ARCoreのプラグインがインポートされていないため、処理を中断しました。", "OK");

			return;
		}

		//プラットフォームがAndroidになっていない場合は切り替える.
#if !UNITY_ANDROID
		EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildTargetGroup.Android, BuildTarget.Android);
#endif
		//ARCore設定を有効化する.
#if UNITY_2018_2_OR_NEWER && !UNITY_2018_2_0
		UnityEditor.PlayerSettings.Android.ARCoreEnabled = true;
#endif

		return;
	}


	[MenuItem("Hologla/ARCore/Initialize Scene with ARCore")]
	static void InitSceneForARCore( )
	{
		//ARCoreプラグインがないままぷAndroidプラットフォームにするとスクリプトにエラーが出るので、最初にプラグインの有無をチェックする.
		if( Directory.Exists(Application.dataPath + "/GoogleARCore") == false ){
			EditorUtility.DisplayDialog("エラー", "ARCoreのプラグインがインポートされていないため、処理を中断しました。", "OK");

			return;
		}

		//デフォルトのMainCameraをDeactivate
		if( null != Camera.main ){
			Undo.RecordObject(Camera.main.gameObject, "Deactivate Camera");
			Camera.main.gameObject.SetActive(false);
		}

		//Hologlaのオブジェクト群を配置
		GameObject hologlaCameraParentRoot = InstantiatePrefab(LoadAssetAtPath<GameObject>(HOLOGLA_CAMERA_PARENT_PATH)) as GameObject;
		Undo.RegisterCreatedObjectUndo(hologlaCameraParentRoot, "Create " + hologlaCameraParentRoot.name);
		GameObject hologlaInput = InstantiatePrefab(LoadAssetAtPath<GameObject>(HOLOGLA_INPUT_PATH)) as GameObject;
		Undo.RegisterCreatedObjectUndo(hologlaInput, "Create " + hologlaInput.name);
//		var playMenu = InstantiatePrefab(LoadAssetAtPath<GameObject>(PLAYMENU_PATH)) as GameObject;
//		Undo.RegisterCreatedObjectUndo(playMenu, "Create " + playMenu.name);

		//ルートのHologlaCameraParentの子のHologlaCameraを取得(HologlaCameraManagerが付いてる方).
		GameObject hologlaCameraParent = hologlaCameraParentRoot.transform.Find("HologlaCamera").gameObject;

		//ARCameraManagerを配置
#if UNITY_ANDROID
		//ARCoreSessionコンポーネントをPlugin内のPrefabからコピーしてくる.
		GameObject arCoreSessionSrcObj = LoadAssetAtPath<GameObject>(AR_CORE_SESSION_PREFAB_PATH);
		GoogleARCore.ARCoreSession arCoreSessionComp ;
		GameObject arCoreSessionObj = new GameObject("ARCoreSession");

		arCoreSessionComp = arCoreSessionSrcObj.GetComponent<GoogleARCore.ARCoreSession>( );
		if( null != arCoreSessionComp ){
			UnityEditorInternal.ComponentUtility.CopyComponent(arCoreSessionComp);	
			//※ARCoreSessionコンポーネントを追加した瞬間、OnValidate関数が実行されて、そこから値が設定されていない旨のエラーメッセージが生成されてしまう.
			UnityEditorInternal.ComponentUtility.PasteComponentAsNew(arCoreSessionObj);
		}

		TrackedPoseDriver trackedPoseDriver ;
		trackedPoseDriver = hologlaCameraParent.GetComponent<HologlaCameraManager>( ).gameObject.AddComponent<TrackedPoseDriver>( );
		trackedPoseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRDevice, TrackedPoseDriver.TrackedPose.ColorCamera);
#endif

		//HologlaCameraManagerのArBackgroundMaterialにARBackgroundを設定
		Material arBackgroundMaterial = LoadAssetAtPath<Material>(HOLOGLA_AR_CORE_AR_MATERIAL_PATH);
		HologlaCameraManager hologlaCameraManager = hologlaCameraParent.GetComponent<HologlaCameraManager>( );
		hologlaCameraManager.arBackgroundMaterial = arBackgroundMaterial;

		ApplyHologlaInputSetting(hologlaCameraManager, hologlaInput.GetComponent<HologlaInput>( ));

		return;
	}


	//HologlaInputに初期の関連づけを行う.
	static void ApplyHologlaInputSetting(HologlaCameraManager hologlaCameraManager, HologlaInput hologlaInput)
	{
		GazeInput gazeInput ;

		gazeInput = hologlaCameraManager.GetComponent<GazeInput>( );
		UnityEventTools.RemovePersistentListener(hologlaInput.onPressLeftAndRight, gazeInput.InputLeftAndRightEvent);
		UnityEventTools.AddPersistentListener(hologlaInput.onPressLeftAndRight, gazeInput.InputLeftAndRightEvent);
		UnityEventTools.RemovePersistentListener(hologlaInput.LeftButtonComp.onClick, gazeInput.InputLeftEvent);
		UnityEventTools.AddPersistentListener(hologlaInput.LeftButtonComp.onClick, gazeInput.InputLeftEvent);
		UnityEventTools.RemovePersistentListener(hologlaInput.RightButtonComp.onClick, gazeInput.InputRightEvent);
		UnityEventTools.AddPersistentListener(hologlaInput.RightButtonComp.onClick, gazeInput.InputRightEvent);

		return;
	}

}
