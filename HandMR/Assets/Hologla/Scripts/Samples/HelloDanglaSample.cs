using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelloDanglaSample : MonoBehaviour {

	[SerializeField]private Hologla.HologlaCameraManager hologlaManager = null ;
	[SerializeField]private GameObject spawnObj = null ;
	[SerializeField]private Animator playMenuAnimator = null ;

	private bool isPlayMenuOpen = false ;
	private bool isSystemMenuOpen = false ;

	// Use this for initialization
	void Start( )
	{
		return;	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SpawnMovableObject(GameObject spawnTransObj)
	{
		//メニューを開いている場合は何もしない.
		if( true == IsOpenMenu( ) ){
			return;
		}
		if( null == spawnObj ){
			return;
		}
		GameObject obj ;
		Rigidbody rigidbody ;

		obj = Instantiate(spawnObj, spawnTransObj.transform.position, spawnTransObj.transform.rotation);
		Destroy(obj, 10.0f);
		obj.SetActive(true);
		rigidbody = obj.GetComponent<Rigidbody>( );
		if( null != rigidbody ){
			rigidbody.AddForce(spawnTransObj.transform.forward * 500.0f);
		}

		return;
	}

	public void BackMenuSample( )
	{
		SceneManager.LoadScene("MenuSample", LoadSceneMode.Single);

		return;
	}


	//メニューオブジェクトの位置を視点正面位置にリセットする.
	public void ResetMenuPosition(GameObject menuObj)
	{
		if( null != hologlaManager ){
			menuObj.transform.position = hologlaManager.transform.position;
		}
		else{
			menuObj.transform.position = Camera.main.transform.position;
		}

		return;
	}
	//メニューオブジェクトの向きを視点正面方向にリセットする(Roll回転は無視する).
	public void ResetMenuRotation(GameObject menuObj)
	{
		if( null != hologlaManager ){
			menuObj.transform.rotation = Quaternion.Euler(hologlaManager.transform.eulerAngles.x, hologlaManager.transform.eulerAngles.y, 0.0f);
		}
		else{
			menuObj.transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, 0.0f);
		}

		return;
	}

	public bool IsOpenMenu( )
	{
		return ((isSystemMenuOpen | isPlayMenuOpen));
	}

	public void SwitchPlayMenuOpenFlag(bool isMenu)
	{
		isPlayMenuOpen = isMenu;

		return;
	}
	public void SwitchSystemMenuOpenFlag(bool isMenu)
	{
		isSystemMenuOpen = isMenu;

		return;
	}

	public void OpenPlayMenu( )
	{
		if( null == playMenuAnimator ){
			return;
		}
		//メニューが開いている場合はプレイメニューの位置を補正して終了.
		AnimatorStateInfo animState ;

		animState = playMenuAnimator.GetCurrentAnimatorStateInfo(0);
		if( true == IsOpenMenu( )
			|| (Animator.StringToHash("PlayMenuIn") == animState.shortNameHash /*|| Animator.StringToHash("PlayMenuOut") == animState.shortNameHash*/) ){
			ResetMenuPosition(playMenuAnimator.gameObject);
			ResetMenuRotation(playMenuAnimator.gameObject);
			playMenuAnimator.ResetTrigger("PlayMenuInReady");

			return;
		}
		if( false == isPlayMenuOpen ){
			playMenuAnimator.SetTrigger("PlayMenuInReady");
		}
		SwitchPlayMenuOpenFlag(true);
		playMenuAnimator.SetTrigger("PlayMenuIn");

		return;
	}


}
