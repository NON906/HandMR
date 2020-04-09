using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Hologla;
using System.Collections.ObjectModel;

public class DanglaMenuSample : MonoBehaviour {

/*	enum MenuItem{
		MRMode,
		VRMode,
		ARMode,
		IPDSetting,
		IPDSettingMenu,
		EyeModeSetting,
		MultiEyeMode,
		SingleEyeMode,
	}*/

	private static readonly ReadOnlyCollection<string> VIEW_MODE_ITEM_NAME = new ReadOnlyCollection<string>(new string[]
		{"_AR",
		"_MR",
		"_VR",}) ;
	private static readonly ReadOnlyCollection<string> EYE_MODE_ITEM_NAME = new ReadOnlyCollection<string>(new string[]
		{"_SingleEye",
		"_TwoEye",}) ;
	private static readonly ReadOnlyCollection<string> VIEW_SIZE_ITEM_NAME = new ReadOnlyCollection<string>(new string[]
		{"_Small",
		"_Mid",
		"_Big",
		"_ExBig",}) ;
	private const string GAME_LAUNCH_ITEM_NAME = "_Game" ;
	private const string MENU_LAUNCH_ITEM_NAME = "_Menu" ;

//	private const float MENU_INTERVAL = 2.0f ;

//	[SerializeField]private GameObject mainMenuObj = null ;
//	[SerializeField]private GameObject eyeModeMenuObj = null ;
//	[SerializeField]private GameObject ipdMenuObj = null ;
	[SerializeField]private HologlaCameraManager hologlaManager = null ;
	[SerializeField]private HologlaInput hologlaInput = null ;
	[SerializeField]private TextMesh ipdTextObj = null ;

//	private MenuItem currentItem = MenuItem.MRMode ;

	private bool isLaunchGameScene = false ;
	private GazeInteractive[] gazeInteractiveArray ;

	// Use this for initialization
	void Start( )
	{
		isLaunchGameScene = UserSettings.isLaunchGameScene;

		if( null == hologlaManager ){
			hologlaManager = GameObject.FindObjectOfType<HologlaCameraManager>( );
		}
		if( null == hologlaInput ){
			hologlaInput = GameObject.FindObjectOfType<HologlaInput>( );
		}
		RegisterMenuTransReset( );
		ResetMenuPosition( );
		ResetMenuRotation( );

		UpdateSelectFrame( );
		
		return;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void Awake( )
	{
		gazeInteractiveArray = gameObject.GetComponentsInChildren<GazeInteractive>( );

		return;
	}

	//各種メニュー項目の選択状態を更新する.
	public void UpdateSelectFrame( )
	{
		foreach( GazeInteractive interactive in gazeInteractiveArray ){
			interactive.SwitchSelectFrame(IsSelectMenuItem(interactive.gameObject));
		}

		return;
	}

	//設定項目の選択状態を判別する.
	private bool IsSelectMenuItem(GameObject checkObject)
	{
		List<string> itemSelectNameList ;

		//現在の設定から対応するゲームオブジェクトに含まれる名称リストを作る.
		itemSelectNameList = new List<string>( );
		itemSelectNameList.Add(VIEW_MODE_ITEM_NAME[(int)hologlaManager.CurrentViewMode]);
		itemSelectNameList.Add(EYE_MODE_ITEM_NAME[(int)hologlaManager.CurrentEyeMode]);
		itemSelectNameList.Add(VIEW_SIZE_ITEM_NAME[(int)hologlaManager.CurrentViewSize]);
		if( true == isLaunchGameScene ){
			itemSelectNameList.Add(GAME_LAUNCH_ITEM_NAME);
		}
		else{
			itemSelectNameList.Add(MENU_LAUNCH_ITEM_NAME);
		}
		//ゲームオブジェクトの名前に含まれるワードから、選択状態を判断する.
		foreach( string itemSelectName in itemSelectNameList ){
			if( true == checkObject.name.Contains(itemSelectName) ){
				return true;
			}
		}

		return false;
	}

	public void SwitchScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

		return;
	}

	//現在のIPD値表示テキストを更新する.
	public void UpdateIPDText( )
	{
		if( null == ipdTextObj || null == hologlaManager ){
			return;
		}
		ipdTextObj.text = string.Format("{0:.0}mm", hologlaManager.InterpupillaryDistance);

		return;
	}

	public void SwitchLaunchGameScene(bool isGameScene)
	{
		isLaunchGameScene = isGameScene;

		return;
	}

	//現在のユーザー設定を保存する.
	public void SaveCurrentSetting( )
	{
		UserSettings.isLaunchGameScene = isLaunchGameScene;
		if( null != hologlaManager ){
			hologlaManager.SaveCurrentSetting( );
		}
		else{
			UserSettings.WriteSettings( );
		}

		return;
	}

	//メニューオブジェクトの位置を視点正面位置にリセットする.
	public void ResetMenuPosition( )
	{
		if( null != hologlaManager ){
			transform.position = hologlaManager.transform.position;
		}
		else{
			transform.position = Camera.main.transform.position;
		}

		return;
	}
	//メニューオブジェクトの向きを視点正面方向にリセットする.
	public void ResetMenuRotation(bool isValidRoll = false)
	{
		Quaternion rotation ;

		if( null != hologlaManager ){
			rotation = hologlaManager.transform.rotation;
		}
		else{
			rotation = Camera.main.transform.rotation;
		}
		if( false == isValidRoll ){
			transform.rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, 0.0f);
		}
		else{
			transform.rotation = rotation;
		}

		return;
	}

	//1眼/2眼の切り替え.
	#region
	public void SwitchEyeModeSingle( ){if( null != hologlaManager ){hologlaManager.SwitchEyeMode(HologlaCameraManager.EyeMode.SingleEye);}}
	public void SwitchEyeModeTwo( ){if( null != hologlaManager ){hologlaManager.SwitchEyeMode(HologlaCameraManager.EyeMode.TwoEyes);}}
	#endregion
	//AR/MR/VRの切り替え.
	#region
	public void SwitchViewModeAR( ){if( null != hologlaManager ){hologlaManager.SwitchViewMode(HologlaCameraManager.ViewMode.AR);}}
	public void SwitchViewModeMR( ){if( null != hologlaManager ){hologlaManager.SwitchViewMode(HologlaCameraManager.ViewMode.MR);}}
	public void SwitchViewModeVR( ){if( null != hologlaManager ){hologlaManager.SwitchViewMode(HologlaCameraManager.ViewMode.VR);}}
	#endregion
	//表示領域サイズの切り替え.
	#region
	public void SwitchViewSize1( ){if( null != hologlaManager ){hologlaManager.SwitchViewSize(HologlaCameraManager.ViewSize.Size1);}}
	public void SwitchViewSize2( ){if( null != hologlaManager ){hologlaManager.SwitchViewSize(HologlaCameraManager.ViewSize.Size2);}}
	public void SwitchViewSize3( ){if( null != hologlaManager ){hologlaManager.SwitchViewSize(HologlaCameraManager.ViewSize.Size3);}}
	public void SwitchViewSize4( ){if( null != hologlaManager ){hologlaManager.SwitchViewSize(HologlaCameraManager.ViewSize.Size4);}}
	public void SwitchViewSize5( ){if( null != hologlaManager ){hologlaManager.SwitchViewSize(HologlaCameraManager.ViewSize.Size5);}}
	#endregion
	//瞳孔間距離の設定.
	public void AddIPD(float addValue){if( null != hologlaManager ){hologlaManager.AddIPD(addValue);}}


	#if false
	public void LeftClickEvent( )
	{
		Vector3 move ;

		move = Vector3.zero;
		switch( currentItem ){
			case MenuItem.MRMode:
				currentItem = MenuItem.IPDSetting;
				move = Vector3.right * MENU_INTERVAL;
				break;
			case MenuItem.VRMode:
				currentItem = MenuItem.MRMode;
				move = Vector3.right * MENU_INTERVAL;
				break;
			case MenuItem.ARMode:
				currentItem = MenuItem.VRMode;
				move = Vector3.right * MENU_INTERVAL;
				break;
			case MenuItem.IPDSetting:
				currentItem = MenuItem.EyeModeSetting;
				move = Vector3.right * MENU_INTERVAL;
				break;
			case MenuItem.SingleEyeMode:
				currentItem = MenuItem.MultiEyeMode;
				move = Vector3.right * MENU_INTERVAL;
				hologlaManager.SwitchEyeMode(HologlaCameraManager.EyeMode.TwoEyes);
				break;
			case MenuItem.IPDSettingMenu:
				UserSettings.interpupillaryDistance -= 0.5f;
				UserSettings.interpupillaryDistance = Mathf.Max(0.0f, UserSettings.interpupillaryDistance);
				ipdTextObj.text = string.Format("{0:.0}mm", UserSettings.interpupillaryDistance);
				hologlaManager.ApplyIPD(UserSettings.interpupillaryDistance);
				break;
		}
		transform.position += move;
		Debug.Log("currentItem:" + currentItem);

		return;
	}
	public void RightClickEvent( )
	{
		Vector3 move ;

		move = Vector3.zero;
		switch( currentItem ){
			case MenuItem.MRMode:
				currentItem = MenuItem.VRMode;
				move = Vector3.left * MENU_INTERVAL;
				break;
			case MenuItem.VRMode:
				currentItem = MenuItem.ARMode;
				move = Vector3.left * MENU_INTERVAL;
				break;
			case MenuItem.IPDSetting:
				currentItem = MenuItem.MRMode;
				move = Vector3.left * MENU_INTERVAL;
				break;
			case MenuItem.EyeModeSetting:
				currentItem = MenuItem.IPDSetting;
				move = Vector3.left * MENU_INTERVAL;
				break;
			case MenuItem.MultiEyeMode:
				currentItem = MenuItem.SingleEyeMode;
				move = Vector3.left * MENU_INTERVAL;
				hologlaManager.SwitchEyeMode(HologlaCameraManager.EyeMode.SingleEye);
				break;
			case MenuItem.IPDSettingMenu:
				UserSettings.interpupillaryDistance += 0.5f;
				ipdTextObj.text = string.Format("{0:.0}mm", UserSettings.interpupillaryDistance);
				hologlaManager.ApplyIPD(UserSettings.interpupillaryDistance);
				break;
		}
		transform.position += move;
		Debug.Log("currentItem:" + currentItem);

		return;
	}
	public void LeftAndRightPressEvent( )
	{
		switch( currentItem ){
			case MenuItem.MRMode:
				UserSettings.viewMode = HologlaCameraManager.ViewMode.MR;
				SceneManager.LoadScene("HelloDangla", LoadSceneMode.Single);
				break;
			case MenuItem.VRMode:
				UserSettings.viewMode = HologlaCameraManager.ViewMode.VR;
				SceneManager.LoadScene("HelloDangla", LoadSceneMode.Single);
				break;
			case MenuItem.ARMode:
				UserSettings.viewMode = HologlaCameraManager.ViewMode.AR;
				SceneManager.LoadScene("HelloDangla", LoadSceneMode.Single);
				break;
			case MenuItem.IPDSetting:
				currentItem = MenuItem.IPDSettingMenu;
				mainMenuObj.SetActive(false);
				ipdMenuObj.SetActive(true);
				break;
			case MenuItem.EyeModeSetting:
				currentItem = MenuItem.MultiEyeMode;
				mainMenuObj.SetActive(false);
				eyeModeMenuObj.SetActive(true);
				break;
			case MenuItem.MultiEyeMode:
				currentItem = MenuItem.MRMode;
				UserSettings.eyeMode = HologlaCameraManager.EyeMode.TwoEyes;
				eyeModeMenuObj.SetActive(false);
				mainMenuObj.SetActive(true);
				break;
			case MenuItem.SingleEyeMode:
				currentItem = MenuItem.MRMode;
				UserSettings.eyeMode = HologlaCameraManager.EyeMode.SingleEye;
				eyeModeMenuObj.SetActive(false);
				mainMenuObj.SetActive(true);
				break;
			case MenuItem.IPDSettingMenu:
				currentItem = MenuItem.MRMode;
				mainMenuObj.SetActive(true);
				ipdMenuObj.SetActive(false);
				break;
		}
		transform.localPosition = Vector3.zero;

		return;
	}
	#endif


	private void RegisterMenuTransReset( )
	{
		hologlaInput.LeftButtonComp.onClick.AddListener(ResetMenuPosition);
		hologlaInput.LeftButtonComp.onClick.AddListener(( ) => ResetMenuRotation(false));

		return;
	}

}
