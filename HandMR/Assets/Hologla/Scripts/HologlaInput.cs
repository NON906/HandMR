using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hologla{
	public class HologlaInput : MonoBehaviour {

		[SerializeField]private Button leftButton = null ;
		public Button LeftButtonComp => leftButton;
		[SerializeField]private Button rightButton = null ;
		public Button RightButtonComp => rightButton;

		public UnityEvent onPressLeftAndRight = new UnityEvent( ) ;

		private bool isPressLeftButton = false ;
		private bool isPressRightButton = false ;

		//複数のシーンにまたがってもいいように、全て共通のフラグで、連続入力チェックを行う.
		static private bool isCallOnPressLeftAndRight = false ;

		// Use this for initialization
		private void Start( )
		{
			//左右のボタンが同時に押された時を検知できるようにしておく.
			if( null != leftButton ){
				RegistButtonPressAndReleaseEvent(leftButton.gameObject, (data) =>
				{
					isPressLeftButton = true;
					//右ボタン押下中に左ボタンが押下された場合.
					if( true == isPressRightButton && false == isCallOnPressLeftAndRight ){
						onPressLeftAndRight.Invoke( );
						//連続で押された判定がされないようにする.
						isCallOnPressLeftAndRight = true;
						leftButton.interactable = false;
						rightButton.interactable = false;
					}
					else{
						leftButton.interactable = true;
					}
				},
				(data) =>
				{
					isPressLeftButton = false;
					isCallOnPressLeftAndRight = false;
				});
			}
			if( null != rightButton ){
				RegistButtonPressAndReleaseEvent(rightButton.gameObject, (data) =>
				{
					isPressRightButton = true;
					//左ボタン押下中に右ボタンが押下された場合.
					if( true == isPressLeftButton && false == isCallOnPressLeftAndRight ){
						onPressLeftAndRight.Invoke( );
						//連続で押された判定がされないようにする.
						isCallOnPressLeftAndRight = true;
						leftButton.interactable = false;
						rightButton.interactable = false;
					}
					else{
						rightButton.interactable = true;
					}
				},
				(data) =>
				{
					isPressRightButton = false;
					isCallOnPressLeftAndRight = false;
				});
			}

			return;
		}
	
		// Update is called once per frame
		void Update( )
		{
			return;
		}


		private void RegistButtonPressAndReleaseEvent(GameObject buttonObj, UnityAction<BaseEventData> onPress, UnityAction<BaseEventData> onRelease)
		{
			EventTrigger eventTrigger ;
			EventTrigger.Entry entry ;

			eventTrigger = buttonObj.gameObject.AddComponent<EventTrigger>( );

			entry = new EventTrigger.Entry( );
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener(onPress);
			eventTrigger.triggers.Add(entry);

			entry = new EventTrigger.Entry( );
			entry.eventID = EventTriggerType.PointerUp;
			entry.callback.AddListener(onRelease);

			eventTrigger.triggers.Add(entry);
			entry = new EventTrigger.Entry( );
			entry.eventID = EventTriggerType.PointerExit;
			entry.callback.AddListener(onRelease);
			eventTrigger.triggers.Add(entry);

			return;
		}

	}
}

