using UnityEngine;
using System.Collections;

public class myAd : MonoBehaviour {

	public static myAd instance;
	public static bool isInitialized = false;
	float waitTimeForAd  = 60f;

	void Awake(){
		instance = this;
	}

	// Use this for initialization
	void Start () {
		if(InternetChecker.isInternetOn && !isInitialized)
			initializeApplovindata ();
	}

	public void initializeApplovindata(){
			//nmmf sdk key
			AppLovin.SetSdkKey ("abcd1234");
			AppLovin.InitializeSdk ();
			Invoke ("reloadRewardedAd",5.0f);
			if (PlayerPrefs.GetInt (GameConstants.key_playerPrefs_Remove_Ads) == 0) {
				AppLovin.PreloadInterstitial ();
				InvokeRepeating("showAdRegularly",waitTimeForAd,waitTimeForAd);
			}
			isInitialized = true;
	}

	// Update is called once per frame
	void Update () {
	}

	bool isshown = false;
	void showAdRegularly(){
		if (!isshown && !AllPopups.instance.ifOngameplay () && PlayerPrefs.GetInt (GameConstants.key_playerPrefs_Remove_Ads) == 0) {
			showInterAd ();
		}
	}

	public void showInterAd(){
		if (InternetChecker.isInternetOn && !isInitialized)
			initializeApplovindata ();
		if (!GameHandler.isAdShowedOnce && PlayerPrefs.GetInt(GameConstants.key_playerPrefs_Remove_Ads)==0) {
			AppLovin.ShowInterstitial ();
			GameHandler.isAdShowedOnce=true;
		}
	}
	public void reloadAd(){
		if(PlayerPrefs.GetInt(GameConstants.key_playerPrefs_Remove_Ads)==0)
			AppLovin.PreloadInterstitial();
	}

	public void showRewardedAd(){
		if (!InternetChecker.isInternetOn)
			AllPopups.instance.internetCheckPopUp.SetActive (true);

		if(AppLovin.IsIncentInterstitialReady()){
				AppLovin.ShowRewardedInterstitial();
		}

	}
	public void reloadRewardedAd(){
			AppLovin.LoadRewardedInterstitial ();
	}
}
