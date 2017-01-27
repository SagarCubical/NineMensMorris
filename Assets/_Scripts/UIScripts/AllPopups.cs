using UnityEngine;
using System.Collections;

public class AllPopups : MonoBehaviour {

	public static AllPopups instance;

	public GameObject newGamePopUp;
	public GameObject leaveGameWarningPopUp;
	public GameObject facebookFrndsPopUp;
	public GameObject facebookFrndsRequestsPopUp;
	public GameObject facebookFrndNotRespondingPopUp;
	public GameObject winLoosePopUp;
	public GameObject menuPopUp;
//	public GameObject facebookLoginPopUp;
	public GameObject internetCheckPopUp;
	public GameObject StorePopUp;
	public GameObject leaderBoardPopUp;
	public GameObject acheivementsPopUp;
	public GameObject gamePausedPopUp;
	public GameObject fbMessagesPopUp;

	void Awake(){
		instance = this;
	}
	public void closeAllpopUps(){
		newGamePopUp.SetActive (false);
		leaveGameWarningPopUp.SetActive (false);
		facebookFrndsPopUp.SetActive (false);
		facebookFrndsRequestsPopUp.SetActive (false);
		winLoosePopUp.SetActive (false);
		newGamePopUp.SetActive (false);
		menuPopUp.SetActive (false);
		internetCheckPopUp.SetActive (false);
		facebookFrndNotRespondingPopUp.SetActive (false);
		StorePopUp.SetActive(false);
		leaderBoardPopUp.SetActive(false);
		acheivementsPopUp.SetActive (false);
		gamePausedPopUp.SetActive (false);
		fbMessagesPopUp.SetActive (false);
	}
	// Use this for initialization
	void Start () {
//		closeAllpopUps ();
	}
	
	// Update is called once per frame
	void Update () {
		if (facebookFrndNotRespondingPopUp.activeInHierarchy) {
			if(PhotonNetwork.inRoom && PhotonNetwork.room.playerCount == 2)
				facebookFrndNotRespondingPopUp.SetActive(false);
		}
	}
	public void lederBoardpopupokClicked(){
		leaderBoardPopUp.SetActive(false);
		menuPopUp.SetActive(true);
	}

	public bool ifOngameplay(){
		if (
			!newGamePopUp.activeInHierarchy &&
			!facebookFrndsPopUp.activeInHierarchy &&
			!facebookFrndsRequestsPopUp.activeInHierarchy &&
			!winLoosePopUp.activeInHierarchy &&
			!newGamePopUp.activeInHierarchy &&
			!menuPopUp.activeInHierarchy &&
			!StorePopUp.activeInHierarchy &&
			!leaderBoardPopUp.activeInHierarchy &&
			!acheivementsPopUp.activeInHierarchy &&
			!fbMessagesPopUp.activeInHierarchy)
			return true;

		return false;
	}

}
