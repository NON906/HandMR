using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaunchScene : MonoBehaviour {

	public string initialLoadSceneName = "" ;
	public string gameSceneName = "" ;

	// Use this for initialization
	void Start () {
		
		if( false == Hologla.UserSettings.isLaunchGameScene ){
			if( 0 < initialLoadSceneName.Length ){
				SceneManager.LoadScene(initialLoadSceneName, LoadSceneMode.Additive);
			}
		}
		else{
			if( 0 < gameSceneName.Length ){
				SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
			}
		}

		return;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
