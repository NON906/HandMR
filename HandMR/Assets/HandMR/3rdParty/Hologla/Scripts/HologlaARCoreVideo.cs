#undef UNITY_ANDROID

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
namespace Hologla
{
	public class HologlaARCoreVideo : GoogleARCore.ARCoreBackgroundRenderer
	{
		Camera camera = null ;
		Matrix4x4 projMatrix ;

		private void Awake( )
		{
			camera = GetComponent<Camera>( );
			if( null != camera ){
				//カメラのプロジェクション行列の初期値を保持しておく.
				projMatrix = camera.projectionMatrix;
			}
			
			return;
		}

		private void LateUpdate( )
		{
			const string topLeftRight = "_UvTopLeftRight" ;
			const string bottomLeftRight = "_UvBottomLeftRight" ;
			Vector4 topUv, bottomUv ;
			GoogleARCore.DisplayUvCoords uvCoords ;

			uvCoords = GoogleARCore.Frame.CameraImage.DisplayUvCoords;
			topUv = new Vector4(uvCoords.TopLeft.x, uvCoords.TopLeft.y, uvCoords.TopRight.x, uvCoords.TopRight.y);
			bottomUv = new Vector4(uvCoords.BottomLeft.x, uvCoords.BottomLeft.y, uvCoords.BottomRight.x, uvCoords.BottomRight.y);

			//2眼モードの場合は横幅の表示領域が半分になるため、使用するカメラの映像の範囲を半分にする.
			if( HologlaCameraManager.EyeMode.TwoEyes == UserSettings.eyeMode ){
				topUv.x += 0.25f;
				topUv.z = (topUv.z * 0.5f) + 0.25f;
				bottomUv.x += 0.25f;
				bottomUv.z = (bottomUv.z * 0.5f) + 0.25f;
			}

			BackgroundMaterial.SetVector(topLeftRight, topUv);
			BackgroundMaterial.SetVector(bottomLeftRight, bottomUv);

			//ARCoreBackgroundRendererのUpdateで書き換えられるため、カメラのプロジェクション行列を設定し直す.
			if( null != camera ){
				camera.projectionMatrix = projMatrix;
			}

			return;
		}
	}
}
#endif
