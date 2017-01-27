using UnityEngine;
using System.Collections;

public class FbPopUpManager : MonoBehaviour {

	public static	FbPopUpManager instance;

	void Awake(){
		instance = this;
	}

	public void upFbScreen(){
		GetComponent<Animation> ().Play ();
		AllPopups.instance.menuPopUp.SetActive (true);

	}

	public void nextButtonClickFbScreen(){
//			if (PlayerPrefs.GetInt (GameConstants.KEY_NO_OF_FRIENDS_INVITED) >= GameConstants.MIN_NO_OF_FRIEND_INVITES) {
				upFbScreen ();
//			} else {
//				StartCoroutine(FacebookManager.instance.showmsgScreenFbmsg());
//				FacebookManager.instance.fbmsg.text = "Invite your friends to continue ";
//			}		
	}

	public void onInviteFbFriendsclicked(){
		if (InternetChecker.isInternetOn) {
			GameObject fbHolder = GameObject.Find("FacebookHolder");
			if(!fbHolder.GetComponent<FacebookManager>().enabled)
				fbHolder.GetComponent<FacebookManager>().enabled = true;
			FacebookManager.invitingFriends = true;
			FacebookManager.instance.loginFbClicked ();
		} else {
			AllPopups.instance.internetCheckPopUp.SetActive (true);
		}
	}

	public void onPlayFbFriendsClicked(){
		if (InternetChecker.isInternetOn) {
			GameObject fbHolder = GameObject.Find("FacebookHolder");
			if(!fbHolder.GetComponent<FacebookManager>().enabled)
				fbHolder.GetComponent<FacebookManager>().enabled = true;
			FacebookManager.invitingFriends=false;
			FacebookManager.instance.loginFbClicked();

		} else
			AllPopups.instance.internetCheckPopUp.SetActive (true);
	}
}
