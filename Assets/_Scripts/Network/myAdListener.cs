using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class myAdListener : MonoBehaviour {
	string msg = "Hello ";
	public Text infoText;
	public Image adImg;
	public Animator rewardAdAnimator;
	// Use this for initialization
	void Start () {
		AppLovin.SetUnityAdListener (this.gameObject.name);
	}

	void onAppLovinEventReceived(string ev){
		if(ev.Contains("DISPLAYEDINTER")) {
			// An ad was shown.  Pause the game.
			msg = "DISPLAYEDINTER";
		}
		else if(ev.Contains(" HIDDENINTER")) {
			// Ad ad was closed.  Resume the game.
			// If you're using PreloadInterstitial/HasPreloadedInterstitial, make a preload call here.
//			myAd.isshown = false;
			if(PlayerPrefs.GetInt(GameConstants.key_playerPrefs_Remove_Ads)==0)
				AppLovin.PreloadInterstitial();
			msg = " HIDDENINTER";
		}
		else if(ev.Contains("LOADEDINTER")) {
			// An interstitial ad was successfully loaded.
			msg = " LOADEDINTER";
		}
		else if(string.Equals(ev, "LOADINTERFAILED")) {
			// An interstitial ad failed to load.
			msg = " LOADINTERFAILED";
		}
		else if(ev.Contains("REWARDAPPROVEDINFO")){
			msg = " REWARDAPPROVEDINFO";
			
			// The format would be "REWARDAPPROVEDINFO|AMOUNT|CURRENCY" so "REWARDAPPROVEDINFO|10|Coins" for example
			string delimeter = "|";

			// Split the string based on the delimeter
			string[] split = ev.Split(delimeter.ToCharArray());
			
			// Pull out the currency amount
			double amount = double.Parse(split[1]);
			
			// Pull out the currency name
			string currencyName = split[2];

			msg = " "+amount+" "+currencyName;
			// Do something with the values from above.  For example, grant the coins to the user.
			//give 2 undo counts for viewing ad
			int undoMoves = PlayerPrefs.GetInt(GameConstants.KEY_NO_OF_UNDO_COUNTS);
			undoMoves += 2;
			PlayerPrefs.SetInt(GameConstants.KEY_NO_OF_UNDO_COUNTS , undoMoves);
			PlayerPrefs.Save();
			MenuManager.instance.setUndoCountofMenu();
		}
		else if(ev.Contains("LOADEDREWARDED")) {
			msg = " LOADEDREWARDED";
			adImg.GetComponent<Image>().color = Color.green;
			rewardAdAnimator.SetBool("isReady",true);

			// A rewarded video was successfully loaded.
		}
		else if(ev.Contains("LOADREWARDEDFAILED")) {
			msg = " LOADREWARDEDFAILED";
			adImg.GetComponent<Image>().color = Color.red;
			// A rewarded video failed to load.
			AppLovin.LoadRewardedInterstitial();
		}
		else if(ev.Contains("HIDDENREWARDED")) {
			msg = " HIDDENREWARDED";
			rewardAdAnimator.SetBool("isReady",false);
			adImg.GetComponent<Image>().color = Color.gray;
//			myAd.isshown = false;
			// A rewarded video was closed.  Preload the next rewarded video.
			AppLovin.LoadRewardedInterstitial();
		}
	}

}
