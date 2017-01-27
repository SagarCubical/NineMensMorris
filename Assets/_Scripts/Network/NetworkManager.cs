using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NetworkManager : Photon.MonoBehaviour {
	public static NetworkManager instance;
	
	RoomInfo[] roomlist;
	
	void Start () {
	}
	void Awake(){
		instance = this;
	}
	
	public void ConnectWithPhotonserver(){
		//Set PhotonNetwork.playerName property before connecting to the server
		PhotonNetwork.playerName = PlayerPrefs.GetString (GameConstants.KEY_FBID_OF_USER);
		if(!PhotonNetwork.connected && InternetChecker.isInternetOn)
			PhotonNetwork.ConnectUsingSettings ("Dog1.0");
	}
	
	
	void Update(){		
	}


	void OnReceivedRoomListUpdate()
	{
		roomlist = PhotonNetwork.GetRoomList ();

		if (!GameHandler.isResultDeclared && NewgameManager.currentgameOption == (int)GameModes.MultiPlayerAnonymous && NewgameManager.currentgameOption != (int)GameModes.MultiPlayerFacebookFriend)
		{	
//			createOrJoinRoom ();
			print("createOrJoinRoom Onrecromm "+NewgameManager.currentgameOption);
		}else {
			getListOfMyChalllenges ();
			FacebookManager.instance.createchallengesList();
		}

	}

	void OnFailedToConnectToPhoton (DisconnectCause cause){
		AllPopups.instance.newGamePopUp.SetActive (true);
		AllPopups.instance.internetCheckPopUp.SetActive (true);
	}

	public string[] getListOfMyChalllenges(){
		
		List<string> challengedRooms = new List<string>();
		string myid = PlayerPrefs.GetString (GameConstants.KEY_FBID_OF_USER);

		if(roomlist == null)
			roomlist = PhotonNetwork.GetRoomList();


		if (roomlist!=null && roomlist.Length != 0) {
			for(int i=0;i<roomlist.Length;i++){
				if(roomlist[i].name.Contains("_"+myid)){
					challengedRooms.Add(roomlist[i].name);
				}
			}
		}
		return challengedRooms.ToArray();
	}

	
	public bool isThisRoomNameExist(string roomName){
		if(roomlist == null)
			roomlist = PhotonNetwork.GetRoomList();

		if (roomlist!=null && roomlist.Length != 0) {
			for(int i=0;i<roomlist.Length;i++){
				if(roomlist[i].name == roomName){
					return true;
				}
			}
		}
		return false;
	}

	public void createOrJoinRoom(){

//		print ("create or join room called");
		//if is still on gameplay else return
		if (!AllPopups.instance.ifOngameplay ())
			return;
		if (!PhotonNetwork.connected)
		{
			ConnectWithPhotonserver();
			createOrJoinRoom();
		}
		else if (PhotonNetwork.room == null) {	

			if(roomlist == null)
				roomlist = PhotonNetwork.GetRoomList();

			if (roomlist.Length != 0) {
				if(GameHandler.currentGameType == GameType.NonFly)
				{
					RoomInfo MyRoom = null;
					for (int i = 0; i < roomlist.Length; i++) {  
						if(!roomlist[i].name.Contains("_") && roomlist[i].name.Contains("RoomNonFly") && roomlist[i].playerCount<roomlist[i].maxPlayers){
							MyRoom = roomlist[i];
						}
					}
					if(MyRoom!=null)
						PhotonNetwork.JoinRoom (MyRoom.name);
					else{
						string thisroomname = getMyRoomName();
						PhotonNetwork.CreateRoom (thisroomname, true, true, 2);
					}
				}
				if(GameHandler.currentGameType == GameType.Fly)
				{
					RoomInfo MyRoom = null;
					for (int i = 0; i < roomlist.Length; i++) {  
						if(!roomlist[i].name.Contains("_") && roomlist[i].name.Contains("RoomFly") && roomlist[i].playerCount<roomlist[i].maxPlayers && roomlist[i].open){
							MyRoom = roomlist[i];
						}
					}
					if(MyRoom!=null)
						PhotonNetwork.JoinRoom (MyRoom.name);
					else{
						string thisroomname = getMyRoomName();
						PhotonNetwork.CreateRoom (thisroomname, true, true, 2);
					}
			
				}
			}
			else{
				PhotonNetwork.CreateRoom ("Room"+GameHandler.currentGameType+""+(roomlist.Length+1), true, true, 2  );
			}
		}
	}

	void OnPhotonCreateRoomFailed (object[ ] codeAndMsg){
//		print ("on create room failed cannot create");
		Invoke ("createOrJoinRoom",5.0f);
	}

	string getMyRoomName(){
	
		string thisroomname = "Room"+GameHandler.currentGameType;
		bool gotmyname = false;
		for(int i=1;i<100000 && !gotmyname;i++){
			thisroomname = thisroomname+""+i;
			for (int j = 0; j < roomlist.Length && !gotmyname; j++) {
				if(thisroomname == roomlist[j].name){
//					print("Room name "+thisroomname+" already exist");
					break;
				}
				if(j==(roomlist.Length-1)){
//					print("Got my name "+thisroomname);
					gotmyname = true;
				}
			}
		}
		return thisroomname;

	}

	public RoomInfo getRoomByName(string roomName){
		
		roomlist = PhotonNetwork.GetRoomList();
		bool gotmyname = false;

		if (roomlist.Length != 0) {
			for (int j = 0; j < roomlist.Length && !gotmyname; j++) {
				if (roomName == roomlist [j].name) {
					return roomlist[j];
				}
			}
		}

		return null;
	} 

	void OnJoinedRoom()
	{

		print ("Joinning room "+PhotonNetwork.room+""+GameHandler.currentGameType+" "+PhotonNetwork.playerName);


		photonView.RPC("setInitialParameters",PhotonTargets.All,null);
		photonView.RPC ("setOpponentsName",PhotonTargets.OthersBuffered,PlayerPrefs.GetString(GameConstants.KEY_NAME_OF_USER));
		
		string name = PlayerPrefs.GetString(GameConstants.KEY_NAME_OF_USER);
//		string name = PlayerPrefs.GetString(GameConstants.KEY_EMAIL_OF_USER);
		if(name!="")
			GameHandler.instance.myName.text = name ;
		else
			GameHandler.instance.myName.text = "You :" ;

		if (PhotonNetwork.room.playerCount == 1) {
			GameHandler.isMyTurn  =  true;
			GameHandler.instance.MyTokenimg.GetComponent<Image>().sprite = GameHandler.instance.blackTokenSprite;
			GameHandler.instance.OpponentTokenimg.GetComponent<Image>().sprite = GameHandler.instance.whiteTokenSprite;
			Map.instance.tokenPrefab = Resources.Load(GameConstants.BLACK_TOKEN_NAME) as GameObject;
			if(PhotonNetwork.offlineMode)
			{
				int rchoose = Random.Range(0,2);
				if(rchoose==0){
					GameHandler.instance.MyTokenimg.GetComponent<Image>().sprite = GameHandler.instance.blackTokenSprite;
					GameHandler.instance.OpponentTokenimg.GetComponent<Image>().sprite = GameHandler.instance.whiteTokenSprite;
					Map.instance.tokenPrefab = Resources.Load(GameConstants.BLACK_TOKEN_NAME) as GameObject;
					AIPlayer.instance.tokenPrefab = Resources.Load(GameConstants.WHITE_TOKEN_NAME) as GameObject;
				}else{
					GameHandler.instance.MyTokenimg.GetComponent<Image>().sprite = GameHandler.instance.whiteTokenSprite;
					GameHandler.instance.OpponentTokenimg.GetComponent<Image>().sprite = GameHandler.instance.blackTokenSprite;
					Map.instance.tokenPrefab = Resources.Load(GameConstants.WHITE_TOKEN_NAME) as GameObject;
					AIPlayer.instance.tokenPrefab = Resources.Load(GameConstants.BLACK_TOKEN_NAME) as GameObject;
				}
			}
			//DemoFake
			if(!PhotonNetwork.offlineMode && NewgameManager.currentgameOption != (int)GameModes.MultiPlayerFacebookFriend){
//				print("Setting Fake");
				NewgameManager.instance.Invoke("setFakePlayerInitialParameters",15.0f);
			}
			
		}else if(PhotonNetwork.room.playerCount == 2){
			GameHandler.isMyTurn  =  false;
			GameHandler.instance.MyTokenimg.GetComponent<Image>().sprite = GameHandler.instance.whiteTokenSprite;
			GameHandler.instance.OpponentTokenimg.GetComponent<Image>().sprite = GameHandler.instance.blackTokenSprite;
			Map.instance.tokenPrefab = Resources.Load(GameConstants.WHITE_TOKEN_NAME) as GameObject;
			PhotonNetwork.room.open = false;
		}
	}

	[RPC]
	void setInitialParameters(){
//		print ("Set initial Values"+PhotonNetwork.room.playerCount);
		if (PhotonNetwork.room.playerCount == 1) {
			GameHandler.isStartGame = false;
			GameHandler.isResultDeclared = false;
			GameHandler.instance.remainingTime = 0;
		}else if(PhotonNetwork.room.playerCount == 2){
			GameHandler.isStartGame = true;
			GameHandler.isResultDeclared = false;
			GameHandler.instance.remainingTime = GameConstants.MAX_TIME_OF_CHANCE;
//			print(""+PhotonNetwork.time+""+GameHandler.instance.remainingTime);
			GameHandler.instance.StartTime = PhotonNetwork.time;
			GameHandler.instance.timerText.GetComponent<Text>().text = ""+GameHandler.instance.remainingTime;
		}
	}

	[RPC]
	void setOpponentsName(string name){
		//		print ("Opponent is :"+name);
		if(name!="")
			GameHandler.instance.opponentName.text = name ;
		else
			GameHandler.instance.opponentName.text = "Opponent :" ;
	}
	
	[RPC]
	void playerLeftRoom(){

//			print ("Other Player Left room");
		if (!GameHandler.isResultDeclared) {
			AllPopups.instance.winLoosePopUp.SetActive (true);
			GameHandler.instance.setScore(GameConstants.WIN_POINTS);
			GameObject.Find ("WinLooseMsg").GetComponent<Text> ().text = "You Win... Opponent left";
			GameHandler.instance.ResetGame ();
			GameHandler.isResultDeclared = true;
			GameHandler.isStartGame = false;
			myAd.instance.showInterAd();
		}
	}

	void OnLeftRoom (){
		print ("Room Left");
		if(NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium && !PhotonNetwork.offlineMode)
			NewgameManager.currentgameOption = (int)GameModes.MultiPlayerAnonymous;
	}

	[RPC]
	void timeOutResultDeclaration(){
		if (!GameHandler.isResultDeclared) {
			AllPopups.instance.winLoosePopUp.SetActive (true);
		
			if (!GameHandler.isMyTurn){
				GameHandler.instance.setScore(GameConstants.WIN_POINTS);
				GameObject.Find ("WinLooseMsg").GetComponent<Text> ().text = "You Win... ";
			}else{
				GameHandler.instance.setScore(GameConstants.LOOSE_POINTS);
				GameObject.Find ("WinLooseMsg").GetComponent<Text> ().text = "You Lost... ";
			}
			GameHandler.instance.ResetGame ();
			GameHandler.isResultDeclared = true;
			NetworkManager.instance.removePlayerFromRoom ();
			GameHandler.isStartGame = false;
			myAd.instance.showInterAd();
		}
	}


	void OnJoinedLobby (){
		print ("onJoinedLobby");
//		getFriendsidsAndCreateList ();
	}

	public void getFriendsidsAndCreateList(){
		if (!GameHandler.isFrndListCreated && FB.IsLoggedIn && PhotonNetwork.connected) {
			//getall ids
			string[] fIds = FacebookManager.instance.getAllfrndsFbids ();
			print ("Find frnds called" + fIds.Length);
			if(fIds.Length!=0){
				PhotonNetwork.FindFriends (fIds);
				StartCoroutine (cretefrndList ());
			}else{
				AllPopups.instance.facebookFrndsPopUp.transform.FindChild("NoFbFrndText").GetComponent<Text>().text = "You have no friends to challenge invite some friends to play with you";
				AllPopups.instance.facebookFrndsPopUp.transform.FindChild("LoadingText").gameObject.SetActive(false);
			}
		}
	}

	IEnumerator cretefrndList(){
//		print ("waiting to create frnd list");
		yield return new WaitForSeconds (11f);
		FacebookManager.instance.createfreiendsList ();
	}

	void OnDisconnectedFromPhoton (){

		print ("Photon disconnected");

		if (!GameHandler.isResultDeclared && GameHandler.isStartGame) {
			AllPopups.instance.winLoosePopUp.SetActive (true);
			GameObject.Find ("WinLooseMsg").GetComponent<Text> ().text = "You Lost ...";
			GameHandler.instance.ResetGame ();
			GameHandler.isResultDeclared = true;
			GameHandler.isStartGame = false;
			GameHandler.isResultDeclared = true;
			myAd.instance.showInterAd();
		}

	}

	public void removePlayerFromRoom(){
		photonView.RPC ("leaveCurrentRoom", PhotonTargets.All, null);
	}
	
	[RPC]
	void leaveCurrentRoom(){
//		print ("Calling leave room");
		PhotonNetwork.LeaveRoom ();
	}

}
