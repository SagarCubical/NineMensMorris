using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Facebook.MiniJSON;

public class ChallengeManager : MonoBehaviour {

	public void onchallengeButtonClicked(){
//		print ("You have challenged "+gameObject.transform.parent.name);
		int index = int.Parse (gameObject.transform.parent.name);
		Dictionary<string, string> myFriendsOfGame = Util.GetIndexFriend (FacebookManager.instance.friends, index);
		string myid = FacebookManager.instance.profile["id"];
		string frndid = myFriendsOfGame ["id"];

//		print ("My id : "+myid+"Challenge frnd id : "+frndid);

		//create a room
		string roomname = "Room"+GameHandler.currentGameType+"_"+myid+"_"+frndid;
		if (PhotonNetwork.connected) {
			//check this room exist
//			if(!NetworkManager.instance.isThisRoomNameExist(roomname))
				PhotonNetwork.CreateRoom (roomname, true, true, 2);
		}
		else {
			NetworkManager.instance.ConnectWithPhotonserver();
			StartCoroutine(createRoomWhenJoined(roomname));
		}

		AllPopups.instance.facebookFrndsPopUp.SetActive (false);
		//Send fb notification of challenge
		string[] id = new string[1];
		id [0] = frndid;
		FB.AppRequest (
			"Play Nine Men's Morris Fantasy with me",
			id,
			null,
			null,
			null,
			"Havin fun with Nine Men's Morris Fantasy",
			"Nine Men's Morris Fantasy Challenge",
			appRequestCallback
			);
		GameHandler.instance.waitForFrndResponce();
	}

	public IEnumerator createRoomWhenJoined(string roomname){
//		print ("calling room after 3 sec.");
		yield return new WaitForSeconds (3.0f);
		if (!PhotonNetwork.inRoom) {
//			if (!NetworkManager.instance.isThisRoomNameExist (roomname))
				PhotonNetwork.CreateRoom (roomname, true, true, 2);
		}
	}

	public void onChallengeAcceptButtonClicked(){

		//inviteRoomName
//		print ("Challenge Accepted ");
		string rName =  gameObject.transform.parent.FindChild("inviteRoomName").GetComponent<Text>().text;
		RoomInfo rInfo = NetworkManager.instance.getRoomByName (rName);
		if (rInfo == null) {
			print("rInfo is null");
			gameObject.transform.parent.gameObject.SetActive(false);
		}
//		print ("Room "+rName+" "+rInfo.playerCount );
//		if (rInfo.playerCount == 1) {
			PhotonNetwork.JoinRoom (rName);
//			PhotonNetwork.room.open = false;
			NewgameManager.currentgameOption = (int)GameModes.MultiPlayerFacebookFriend;
			//set mode fly/nonfly as of frnd
			if(rName.Contains("RoomNonFly")){
				GameHandler.currentGameType = GameType.NonFly;
				GameObject.Find("GameplayGameName").GetComponent<Text>().text = "Nine Men's Morris Fantasy(non fly)";
			}

		AllPopups.instance.closeAllpopUps ();
		AllPopups.instance.facebookFrndsRequestsPopUp.SetActive (false);
//		}
	}

	private void appRequestCallback (FBResult result)                                                                              
	{                                                                                                                              
//		Util.Log("appRequestCallback");                                                                                         
		if (result != null)                                                                                                        
		{                                                                                                                          
			var responseObject = Json.Deserialize(result.Text) as Dictionary<string, object>;                                      
			object obj = 0;                                                                                                        
			if (responseObject.TryGetValue ("cancelled", out obj))                                                                 
			{                                                                                                                      
				Util.Log("Request cancelled");                                                                                  
			}                                                                                                                                                                                                                                          
		}                                                                                                                          
	}   

	public void onChallengeRejectButtonClicked(){
//		print ("Challenge Declined ");
		string rName =  gameObject.transform.parent.FindChild("inviteRoomName").GetComponent<Text>().text;
//		print ("Room "+rName);
		Destroy (gameObject.transform.parent.gameObject);
//		gameObject.transform.parent.gameObject.SetActive (false);
		
	}

}
