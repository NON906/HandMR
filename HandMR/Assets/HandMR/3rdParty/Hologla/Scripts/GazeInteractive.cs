using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;


namespace Hologla{

	public enum ClickType{
		LeftClick,
		RightClick,
		LeftAndRightClick,
	};

	public interface IGazeInteract{
		void OnSelect( );
		void OnDeselect( );
		void OnClick(ClickType clickType);
	}

	public class GazeInteractive : MonoBehaviour, IGazeInteract{

		[SerializeField]private UnityEvent onGazeSelect = new UnityEvent( ) ;
		[SerializeField]private UnityEvent onGazeDeselect = new UnityEvent( ) ;
		[SerializeField]private UnityEvent onLeftClick = new UnityEvent( ) ;
		[SerializeField]private UnityEvent onRightClick = new UnityEvent( ) ;
		[SerializeField]private UnityEvent onLeftAndRightClick = new UnityEvent( ) ;
		[SerializeField]private UnityEvent onAnyClick = new UnityEvent( ) ;

		[SerializeField]private GameObject selectFrame = null ;

		// Use this for initialization
		void Start( )
		{
			return;
		}
	
		// Update is called once per frame
		void Update( )
		{
			return;
		}

		public void OnClick(ClickType clickType)
		{
			switch( clickType ){
				case ClickType.LeftClick:
					onLeftClick.Invoke( );
					break;
				case ClickType.RightClick:
					onRightClick.Invoke( );
					break;
				case ClickType.LeftAndRightClick:
					onLeftAndRightClick.Invoke( );
					break;
			}
			onAnyClick.Invoke( );

			return;
		}

		public void OnSelect( )
		{
			onGazeSelect.Invoke( );

			return;
		}

		public void OnDeselect( )
		{
			onGazeDeselect.Invoke( );

			return;
		}

		public void SwitchSelectFrame(bool isValid)
		{
			if( null != selectFrame ){
				selectFrame.SetActive(isValid);
			}

			return;
		}

	}
}
