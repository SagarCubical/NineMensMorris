using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InternetChecker : MonoBehaviour {
	public static InternetChecker instance;
	public static bool isInternetOn = false;

	// Use this for initialization
	void Awake () {
		instance = this;
		callCheckNet ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void callCheckNet(){
		StartCoroutine (checkInternetConnection());
	
	}

	IEnumerator checkInternetConnection(){
		WWW www = new WWW("http://google.com");
		yield return www;
		if (www.error != null) {
			isInternetOn = false;
//			print("net OFF");
			//retry after 
		} else {
//			print("net ON");
			isInternetOn = true;
			//initialize fb
			if(!FacebookManager.isfbinitialized)
				FacebookManager.instance.initializeFb();
			if(!myAd.isInitialized)
				myAd.instance.initializeApplovindata();
//			if(!OpenIABTest.instance._isInitialized)
//			{
//				print("int check iab init call");
//				OpenIABTest.instance.initializeOpenIAB();
//			}
		}
	} 

	//from link http://forum.unity3d.com/threads/how-can-you-tell-if-there-exists-a-network-connection-of-any-kind.68938/
	public static bool isInternetON(){
		#if UNITY_IPHONE
		if (Application.internetReachability != NetworkReachability.NotReachable)
		{
			isInternetOn = true;
		}else
			isInternetOn = false;
		#endif
		#if UNITY_ANDROID
		if (Application.internetReachability != NetworkReachability.NotReachable)
		{
			return true;
		}else
			return false;
		#endif

		return false;
	}
}
