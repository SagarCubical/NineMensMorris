using UnityEngine;
using System.Collections;

public class SplashScreenManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (removeSplashScreeen());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator removeSplashScreeen(){
		yield return new WaitForSeconds (3.0f);
		gameObject.SetActive (false);
		AllPopups.instance.menuPopUp.SetActive (true);
	}
}
