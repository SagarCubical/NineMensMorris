using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Facebook.MiniJSON;
using System.Linq;

public class FacebookManager : MonoBehaviour {

	public static FacebookManager instance = null;

//	public GameObject fbMsgScreen;
//	public Text fbmsg;
	public static bool isUserLoggedIn = false ;
	public static bool isfbinitialized;
	public static bool invitingFriends;
	public Dictionary<string,string> profile=null;
	public GameObject fbMessagesParent , fbMsgPrefab;

	void Awake(){
		instance = this;
	}

	void Start () {
		enabled = true;
//		if (InternetChecker.isInternetON())
//			initializeFb ();
	}

	public void initializeFb(){
		FB.Init (onFbInitComplete,FB.OnHideUnity,null);
	}

	void onFbInitComplete(){
		isfbinitialized = true;

			if (FB.IsLoggedIn) 
			{
				OnLoggedin();
			}
	}
	//public_profile,user_friends,publish_actions,email,read_custom_friendlists
	public void loginFbClicked(){
		if (isfbinitialized) {
			if (!FB.IsLoggedIn) {
				FB.Login ("public_profile,user_friends,publish_actions,email", onFbLogIn);
			} else if (FB.IsLoggedIn) {
				OnLoggedin ();
			}
		} else
			initializeFb ();
	}


	string meQueryString = "/v2.3/me?fields=id,name,email,friends.limit(10).fields(name,id,picture.width(182).height(182))";


	string lastResponse;
	void onFbLogIn(FBResult result){
		if (result.Error != null)
			lastResponse = "Error Response:\n" + result.Error;
		else if (!FB.IsLoggedIn)
		{
			lastResponse = "Login cancelled by Player";

		}
		else
		{
			lastResponse = "Login was successful!";
			OnLoggedin ();
		}
	}

	void OnLoggedin(){
		isUserLoggedIn = true;
		//redierect to gameplay
		if (invitingFriends) 
			inviteFriends ();
		NetworkManager.instance.ConnectWithPhotonserver ();
//		print ("Get info of user");
		//Get informations of user and its friends
		FB.API(meQueryString, Facebook.HttpMethod.GET, APICallback);
	}

	public IEnumerator showmsgScreenFbmsg(){
//		fbMsgScreen.SetActive (true);
		yield return new WaitForSeconds (5.0f);
//		fbMsgScreen.SetActive (false);
	}
	public List<object> friends;
	void APICallback(FBResult result)
	{
		Debug.Log("APICallback"+result.Text);

		if (result.Error != null)
		{
			Util.LogError("api error "+result.Error);
//			 Let's just try again
			FB.API(meQueryString, Facebook.HttpMethod.GET, APICallback);
			return;
		}
		print ("Friends : "+result.Text);
		friends = Util.DeserializeJSONFriends(result.Text);
		profile = Util.DeserializeJSONProfile(result.Text);
		PlayerPrefs.SetString (GameConstants.KEY_NAME_OF_USER , profile["name"]);
		PlayerPrefs.SetString (GameConstants.KEY_EMAIL_OF_USER , profile["email"]);
		PlayerPrefs.SetString (GameConstants.KEY_FBID_OF_USER , profile["id"]);
		PlayerPrefs.Save();
		//connect with photon
		NetworkManager.instance.ConnectWithPhotonserver ();
		//Register User
		DataManager.instance.registerUserClicked ();
	}
	//list of challenges by frnds 
	public GameObject challengeframePrefab, challengefriendParentPanel;
	public void createchallengesList(){

		string[] challegeRooms = NetworkManager.instance.getListOfMyChalllenges ();

		print ("FM createchallengesList called"+challegeRooms.Length);
		if (challengefriendParentPanel.transform.childCount != 0) {
			for(int i=0;i<challengefriendParentPanel.transform.childCount;i++)
				if(challengefriendParentPanel.transform.GetChild(i).gameObject.activeInHierarchy)
					Destroy(challengefriendParentPanel.transform.GetChild(i).gameObject);
			AllPopups.instance.facebookFrndsRequestsPopUp.SetActive (false);
		}

		if (challengefriendParentPanel.transform.childCount == 0) {
			
			for (int i=0; i<challegeRooms.Length ; i++) {

				string separator = "_";
				string[] mid  = challegeRooms[i].Split(separator.ToCharArray());
				string challenger = getNameofFriendById(mid[1]);
				print ("Challenger friends Name " +challenger+" "+challegeRooms[i]);

				GameObject cfriendFrame = Instantiate (challengeframePrefab)  as GameObject;
				cfriendFrame.name = ""+i;
				RectTransform myrect = challengefriendParentPanel.transform.GetComponent<RectTransform>();
				myrect.sizeDelta = new Vector2(myrect.rect.width ,( myrect.rect.height + 230f));
				cfriendFrame.transform.parent = challengefriendParentPanel.transform;
				cfriendFrame.GetComponent<RectTransform> ().localScale = Vector3.one;

				cfriendFrame.transform.FindChild ("RequestText").GetComponent<Text> ().text = challenger+" has challenged You";
				cfriendFrame.transform.FindChild ("inviteRoomName").GetComponent<Text> ().text = challegeRooms[i];

			}
		}

		if (challegeRooms.Length > 0) {
			AllPopups.instance.closeAllpopUps ();
			AllPopups.instance.facebookFrndsRequestsPopUp.SetActive (true);
		} 

	}

	public string[] getAllfrndsFbids(){
		string[] frndids = new string[friends.Count];
		for (int i=0; i<friends.Count; i++) {
			Dictionary<string, string> myFriendsOfGame = Util.GetIndexFriend (friends, i);
			frndids [i] = myFriendsOfGame ["id"];
		}
		return frndids;
	}

	string getNameofFriendById(string frndFbId){

		string fname = "";
		for (int i=0; i<friends.Count; i++) {
			Dictionary<string, string> myFriendsOfGame = Util.GetIndexFriend (friends, i);
//			print(" "+myFriendsOfGame["id"]+" "+frndFbId+" "+myFriendsOfGame["name"]);
			if(myFriendsOfGame["id"] == frndFbId){
				fname = myFriendsOfGame["name"];
			}
		}
		return fname;
	}

	//frnd list to challenge

	public GameObject framePrefab, friendParentPanel;
	GameObject friendFrame;
//	List<GameObject> online = new List<GameObject>();
//	List<GameObject> offline = new List<GameObject>();
	public void createfreiendsList(){

		if (friendParentPanel.transform.childCount != 0) {
			foreach(Transform child in friendParentPanel.transform)
				Destroy(child.gameObject);
		}

		if (friendParentPanel.transform.childCount == 0) {
			AllPopups.instance.facebookFrndsPopUp.transform.FindChild("LoadingText").gameObject.SetActive(false);
//			print("create frnd list");
			FriendInfo[] fInfo = PhotonNetwork.Friends.ToArray();
					
			for (int i=0; i<friends.Count ; i++) {

				Dictionary<string, string> myFriendsOfGame = Util.GetIndexFriend (friends, i);
				friendFrame = Instantiate (framePrefab)  as GameObject;
				friendFrame.name = ""+i;

				RectTransform myrect = friendParentPanel.transform.GetComponent<RectTransform>();
				myrect.sizeDelta = new Vector2(myrect.rect.width ,( myrect.rect.height + 230f));

				friendFrame.transform.SetParent(friendParentPanel.transform);
				friendFrame.GetComponent<RectTransform> ().localScale = Vector3.one;

				//set parameters
				friendFrame.transform.FindChild ("Friendname").GetComponent<Text> ().text = myFriendsOfGame ["name"];

				if(fInfo[i].Name == myFriendsOfGame ["id"] ){
//					print(""+fInfo[i].Name+""+fInfo[i].IsOnline);
					if(fInfo[i].IsOnline){
						friendFrame.transform.FindChild ("ChallengeButton").gameObject.SetActive(true);
//						online.Add(friendFrame);
					}
					else{
						friendFrame.transform.FindChild ("ChallengeButton").gameObject.SetActive(false);
//						offline.Add(friendFrame);
					}
				}
				//getPicture
//				print("url "+myFriendsOfGame["image_url"]);
				LoadPictureURL(myFriendsOfGame["image_url"],friendFrame,FriendPictureCallback);
			}

			GameHandler.isFrndListCreated = true;
			StartCoroutine(updateFriendListIteratively());
		}

	}

	//call this after 20 sec and then after 11 sec update list
	IEnumerator updateFriendListIteratively(){
		yield return new WaitForSeconds (1f);
		string[] fIds = getAllfrndsFbids ();
		if (PhotonNetwork.connected && (AllPopups.instance.newGamePopUp.activeInHierarchy || AllPopups.instance.facebookFrndsPopUp.activeInHierarchy)) {
			if(fIds.Length != 0){
				PhotonNetwork.FindFriends (fIds);
				StartCoroutine (updateMyFriendsOnlineList ());
			}
		}
		else
			StartCoroutine(updateFriendListIteratively());
	}

	IEnumerator updateMyFriendsOnlineList(){
		yield return new WaitForSeconds (10);
		if (friendParentPanel.transform.childCount != 0) {
			FriendInfo[] fInfo = PhotonNetwork.Friends.ToArray();
			
			for (int i=0; i<friends.Count ; i++) {
				Dictionary<string, string> myFriendsOfGame = Util.GetIndexFriend (friends, i);
				GameObject	childFrame = friendParentPanel.transform.GetChild(i).gameObject;
				string nm = childFrame.transform.FindChild ("Friendname").GetComponent<Text> ().text;

					if(fInfo[i].Name == myFriendsOfGame ["id"] ){

						if(fInfo[i].IsOnline){
							childFrame.transform.FindChild ("ChallengeButton").gameObject.SetActive(true);
						}
						else{
							childFrame.transform.FindChild ("ChallengeButton").gameObject.SetActive(false);
						}
					}

			}
			StartCoroutine(updateFriendListIteratively());
		}
	}

	public void FriendPictureCallback(Texture texture,GameObject frame)
	{
		print ("FriendPictureCallback"+frame.name);
		frame.transform.FindChild ("ProfilePic").GetComponent<RawImage>().texture = texture;
	
	}

	void LoadPictureURL (string url,GameObject frame, LoadPictureCallback callback)
	{
		StartCoroutine(LoadPictureEnumerator(url,frame,callback));		
	}
	IEnumerator LoadPictureEnumerator(string url,GameObject frame, LoadPictureCallback callback)    
	{
		WWW www = new WWW(url);
		yield return www;
		callback(www.texture,frame);
	}

	delegate void LoadPictureCallback (Texture texture,GameObject frame);
	
	//Invite Friends
	
	 string FriendSelectorTitle = "Invite friends to play Nine Men's Morris Fantasy";
	 string FriendSelectorMessage = "Come friends play Nine Men's Morris Fantasy with me";
	 string FriendSelectorFilters = "[\"app_users\"]";
	 string FriendSelectorData = "invite";
	 string FriendSelectorExcludeIds = "";
	 string FriendSelectorMax = "10";
	string[] excludeIds = null;
	List<object> FriendSelectorFiltersArr = null;
	
	public void inviteFriends(){

		print ("Invite frnds called");
		int? maxRecipients = null;
		if (FriendSelectorMax != "")
		{
			try
			{
				maxRecipients = Int32.Parse(FriendSelectorMax);
			}
			catch (Exception e)
			{
				print(""+e.Message);
			}
		}
		
		// include the exclude ids
		if (FB.IsLoggedIn) {
			FB.AppRequest (
				FriendSelectorMessage,
				null,
				FriendSelectorFiltersArr,
				excludeIds,
				maxRecipients,
				FriendSelectorData,
				FriendSelectorTitle,
				appRequestCallback
				);
		}
	
	}
	void appRequestCallback(FBResult result){
		Util.Log("appRequestCallback");                                                                                         
		if (result != null)                                                                                                        
		{                                                                                                                          
			var responseObject = Json.Deserialize(result.Text) as Dictionary<string, object>;                                      
			object obj = 0;                                                                                                        
			if (responseObject.TryGetValue ("cancelled", out obj))                                                                 
			{                                                                                                                      
				Util.Log("Request cancelled");                                                                                  
			}                                                                                                                      
			else if (responseObject.TryGetValue ("request", out obj))                                                              
			{                
				//AddPopupMessage("Request Sent", ChallengeDisplayTime);
				Util.Log("Request sent");
				IEnumerable<object> invitesSent = (IEnumerable<object>) responseObject["to"];
				int noofInvitesSent = invitesSent.Count();
				//give 1 undo on each invite
				int uCount = PlayerPrefs.GetInt(GameConstants.KEY_NO_OF_UNDO_COUNTS);
				uCount += noofInvitesSent;
				PlayerPrefs.SetInt(GameConstants.KEY_NO_OF_UNDO_COUNTS , uCount);
				PlayerPrefs.Save();
				MenuManager.instance.setUndoCountofMenu();

			}                                                                                                                      
		}  
		print ("FrindRequest Callback "+result.Text);
		
	}
	

}
