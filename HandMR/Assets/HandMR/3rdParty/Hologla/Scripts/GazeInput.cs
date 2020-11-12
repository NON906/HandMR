using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hologla{
	public class GazeInput : MonoBehaviour {

		[SerializeField]private GameObject gazeObject = null ;

		[SerializeField]private GameObject cursorObject = null ;
//		[Tooltip("")]
		[SerializeField]private float defaultCursorDistance = 2.0f ;

		private IGazeInteract currentSelectObject = null ;

		// Use this for initialization
		void Start( )
		{
			if( null == gazeObject ){
				gazeObject = gameObject;
			}

			return;
		}
	
		// Update is called once per frame
		void Update( ){
		
			RaycastHit raycastHit ;
			Vector3 cursorPos ;

			cursorPos = gazeObject.transform.position + (gazeObject.transform.forward * defaultCursorDistance);
			if( true == Physics.Raycast(gazeObject.transform.position, gazeObject.transform.forward, out raycastHit) ){
				IGazeInteract gazeInteract ;

				gazeInteract = raycastHit.collider.GetComponent<IGazeInteract>( );
				if( null != currentSelectObject && gazeInteract != currentSelectObject ){
					currentSelectObject.OnDeselect( );
					currentSelectObject = null;
				}
				if( null != gazeInteract && gazeInteract != currentSelectObject ){
					currentSelectObject = gazeInteract;
					currentSelectObject.OnSelect( );
				}
				cursorPos = raycastHit.point;
			}
			else if( null != currentSelectObject ){
				currentSelectObject.OnDeselect( );
				currentSelectObject = null;
			}

			if( null != cursorObject ){
				cursorObject.transform.position = cursorPos;
			}

			return;
		}

		//入力操作時に外から呼び出せる関数.
		public void InputLeftEvent( )
		{
			if( null != currentSelectObject ){
				currentSelectObject.OnClick(ClickType.LeftClick);
			}

			return;
		}
		public void InputRightEvent( )
		{
			if( null != currentSelectObject ){
				currentSelectObject.OnClick(ClickType.RightClick);
			}

			return;
		}
		public void InputLeftAndRightEvent( )
		{
			if( null != currentSelectObject ){
				currentSelectObject.OnClick(ClickType.LeftAndRightClick);
			}

			return;
		}


	}
}
