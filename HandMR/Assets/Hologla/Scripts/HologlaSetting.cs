using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using System.Collections.ObjectModel;

namespace Hologla{
	public static class UserSettings {

		//AR/MR/VR.
		public static HologlaCameraManager.ViewMode viewMode = HologlaCameraManager.ViewMode.MR ;
		//1眼/2眼.
		public static HologlaCameraManager.EyeMode eyeMode = HologlaCameraManager.EyeMode.TwoEyes ;
		//瞳孔間距離.
		public static float interpupillaryDistance = 64.0f ;
		//起動時にゲームシーンをすぐに開くかどうか.
		public static bool isLaunchGameScene = false ;
		//表示領域サイズ設定.
		public static HologlaCameraManager.ViewSize viewSize = HologlaCameraManager.ViewSize.Size1 ;

		//設定データのバージョン情報.
		private static int dataVersion = 1 ;

		//表示領域サイズに対応したビューポートサイズリスト.
		//以下の機種ごとの画面の高さ、幅情報を元に、一番小さいものを基準に比率によって計算した値を設定しておく.
		//【iPhone 6S-8】105,59.
		//【iPhone X-XS】135,62.
		//【iPhone XR】142,67.
		//【iPhone 6S-8Plus】121,68.
		//【iPhone XS Max】149,69.
		public static readonly ReadOnlyCollection<Vector2> viewportSizeList = new ReadOnlyCollection<Vector2>(new Vector2[]
#if UNITY_ANDROID
			{new Vector2(0.5f * 1.0f, 1.0f * 1.0f),
			new Vector2(0.5f * 0.945945946f, 1.0f * 0.936507937f),
			new Vector2(0.5f * 0.8203125f, 1.0f * 0.951612903f),
			new Vector2(0.5f * 0.833333333f, 1.0f * 0.819444444f),
			new Vector2(0.5f * 0.734265734f, 1.0f * 0.855072464f),});
#else
			{new Vector2(0.5f * 1.0f, 1.0f * 1.0f),
			new Vector2(0.5f * 0.777777778f, 1.0f * 0.951612903f),
			new Vector2(0.5f * 0.73943662f, 1.0f * 0.880597015f),
			new Vector2(0.5f * 0.867768595f, 1.0f * 0.867647059f),
			new Vector2(0.5f * 0.704697987f, 1.0f * 0.855072464f),});
#endif

		private const string SETTING_KEY_DATA_VERSION = "Hologla_DataVersion" ;

		private const string SETTING_KEY_VIEW_MODE = "Hologla_ViewMode" ;
		private const string SETTING_KEY_EYE_MODE = "Hologla_EyeMode" ;
		private const string SETTING_KEY_IPD = "Hologla_InterpupillaryDistance" ;
		private const string SETTING_KEY_VIEW_SIZE = "Hologla_ViewSize" ;
		private const string SETTING_KEY_IS_LAUNCH_GAME = "Hologla_IsLaunchGameScene" ;


		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void ReadSettings( )
		{
			//有効なデータがない場合は初期値データを出力しておく.
			if( false == HasValidSettingData( ) ){
				WriteSettings( );
			}

			viewMode = (HologlaCameraManager.ViewMode)PlayerPrefs.GetInt(SETTING_KEY_VIEW_MODE, (int)viewMode);
			eyeMode = (HologlaCameraManager.EyeMode)PlayerPrefs.GetInt(SETTING_KEY_EYE_MODE, (int)eyeMode);
			interpupillaryDistance = PlayerPrefs.GetFloat(SETTING_KEY_IPD, interpupillaryDistance);
			viewSize = (HologlaCameraManager.ViewSize)PlayerPrefs.GetInt(SETTING_KEY_VIEW_SIZE, (int)viewSize);
			isLaunchGameScene = bool.Parse(PlayerPrefs.GetString(SETTING_KEY_IPD, isLaunchGameScene.ToString( )));

			return;
		}


		public static void WriteSettings( )
		{
			PlayerPrefs.SetInt(SETTING_KEY_DATA_VERSION, dataVersion);

			PlayerPrefs.SetInt(SETTING_KEY_VIEW_MODE, (int)viewMode);
			PlayerPrefs.SetInt(SETTING_KEY_EYE_MODE, (int)eyeMode);
			PlayerPrefs.SetFloat(SETTING_KEY_IPD, interpupillaryDistance);
			PlayerPrefs.SetInt(SETTING_KEY_VIEW_SIZE, (int)viewSize);
			PlayerPrefs.SetString(SETTING_KEY_IPD, isLaunchGameScene.ToString( ));

			return;
		}


		private static bool HasValidSettingData( )
		{
			if( false == PlayerPrefs.HasKey(SETTING_KEY_VIEW_MODE) ){
				return false;
			}

			return true;
		}


	}
}
