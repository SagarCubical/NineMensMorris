using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum GameModes{
	SinglePlayerEasy = 11,
	SinglePlayerMedium = 12,
	SinglePlayerHard = 13,
	MultiPlayerAnonymous = 21,
	MultiPlayerFacebookFriend = 22,
	Human = 30,
};

public enum GameType{
	Fly = 1,
	NonFly = 2,
};

public class NewgameManager : MonoBehaviour {

	public static NewgameManager instance;

//	public Text GameplayPlayerName;
	public Text gameName, gameNameGamePlay;
	public Text infoText;
	public RectTransform infoPanel;
	public GameObject NetworkObject,myPlayerPrefab;
	public GameObject aiPlayer;

	public static int currentgameOption=0;

	int currentMode,currentSubLevel;

	void Awake(){
		instance = this;
	}

	void Start(){
		checkTypeofNewgame (10);
		GameHandler.currentGameType = GameType.Fly;
	}

	public void changeFlyNonFlyText(int index){

		switch (index) {
		
		case 1:
			gameName.text				= "9 Men's Morris Fantasy(fly)";
			gameNameGamePlay.text	 	= "9 Men's Morris Fantasy(fly)";
			GameHandler.currentGameType = GameType.Fly;
			infoPanel.localPosition = new Vector3(infoPanel.localPosition.x , 0f , 0f);
			infoPanel.sizeDelta = new Vector2(infoPanel.rect.width, 1688f);
			infoText.text				= "The game begins with an empty board. The players determine who plays first, then take turns placing their men one "+
				"per play on empty points. If a player is able to place three of his pieces in a straight line, vertically or "+
				"horizontally, he has formed a mill and may remove one of his opponent's piece from the board and the game. Any "+
				"piece can be chosen for the removal, but a piece not in an opponent's mill must be selected, if possible.";
			break;
		case 2:
			gameName.text				 = "9 Men's Morris Fantasy(non fly)";
			gameNameGamePlay.text		 = "9 Men's Morris Fantasy(non fly)";
			GameHandler.currentGameType	 = GameType.NonFly;
			infoPanel.localPosition =  new Vector3(infoPanel.localPosition.x , 0f , 0f);
			infoPanel.sizeDelta = new Vector2(infoPanel.rect.width, 1088f);
			infoText.text				 = "When a player is reduced to three pieces, there is no longer a limitation on that player of moving to only "+
				"adjacent points: The player's men may fly, hop or jump from any point to any vacant point.";
			break;
		}

	}

	public void checkTypeofNewgame(int index){
		GameObject single, multi, human;
		single = GameObject.Find ("SinglePlayerOption");
		multi = GameObject.Find ("MultiPlayerOption");
		human = GameObject.Find ("HumanOption");

		switch(index){
		case 10:
			if(single !=null && single.GetComponent<Toggle>().isOn)
				checkSubLevelSinglePlayer();
			break;
		case 20:
			if(multi !=null && multi.GetComponent<Toggle>().isOn)
				checkSubLevelMultiPlayer();
			break;
		case 30:
			if(human !=null && human.GetComponent<Toggle>().isOn)
				currentgameOption = (int)GameModes.Human;
			break;
		}

	}


	public void checkSubLevelSinglePlayer(){

		GameObject easy, medium, hard;
		easy = GameObject.Find ("Easy");
		medium = GameObject.Find ("Medium");
		hard = GameObject.Find ("Hard");

		if (easy == null || medium == null || hard == null) {
		}else{

			if(easy.GetComponent<Toggle>().isOn){
				currentgameOption = (int)GameModes.SinglePlayerEasy;
			}
			if(medium.GetComponent<Toggle>().isOn){
				currentgameOption = (int)GameModes.SinglePlayerMedium;
			}
			if(hard.GetComponent<Toggle>().isOn){
				currentgameOption = (int)GameModes.SinglePlayerHard;
			}
		}
	}

	public void checkSubLevelMultiPlayer(){
		if(!PhotonNetwork.connected)
			NetworkManager.instance.ConnectWithPhotonserver();

		GameObject anonymous, fbfriend;
		anonymous = GameObject.Find ("Anonynous");
		fbfriend = GameObject.Find ("fbFriends");
		
		if (anonymous != null && fbfriend != null) {
			
			if(anonymous.GetComponent<Toggle>().isOn){
				currentgameOption = (int)GameModes.MultiPlayerAnonymous;
			}
			if(fbfriend.GetComponent<Toggle>().isOn){
				currentgameOption = (int)GameModes.MultiPlayerFacebookFriend;
//				if (InternetChecker.isInternetOn) {
//					//enable fb
//					GameObject fbHolder = GameObject.Find("FacebookHolder");
//					if(!fbHolder.GetComponent<FacebookManager>().enabled)
//						fbHolder.GetComponent<FacebookManager>().enabled = true;
//				}
			}
	
		}
}

	public void onFbFriendsBackButtonClicked(){
		AllPopups.instance.facebookFrndsPopUp.SetActive (false);
		AllPopups.instance.newGamePopUp.SetActive (true);
	}

	public void onFbChallengesBackButtonClicked(){
		AllPopups.instance.facebookFrndsRequestsPopUp.SetActive (false);
		AllPopups.instance.facebookFrndsPopUp.SetActive (true);
	}

	public void onFbFriendsClicked(){

		print ("on fb friends clicked");
		//if net is On
		if (InternetChecker.isInternetOn) {
			//connect to photon
			NewgameManager.instance.NetworkObject.SetActive(true);
			if(!PhotonNetwork.connected)
				NetworkManager.instance.ConnectWithPhotonserver();

			if (FacebookManager.isUserLoggedIn) {
				string[] mychallenges = NetworkManager.instance.getListOfMyChalllenges();
				NetworkManager.instance.getFriendsidsAndCreateList ();
//				print ("NGM mychallenges.Length "+mychallenges.Length);
				if(mychallenges.Length != 0){
					FacebookManager.instance.createchallengesList();
					AllPopups.instance.facebookFrndsRequestsPopUp.SetActive(true);
				}
				else{
					AllPopups.instance.facebookFrndsPopUp.SetActive (true);
				}

			} else {
				AllPopups.instance.newGamePopUp.SetActive (true);
				FacebookManager.instance.loginFbClicked ();
			}

		} else {
			AllPopups.instance.newGamePopUp.SetActive (true);
			AllPopups.instance.internetCheckPopUp.SetActive (true);
			InternetChecker.instance.callCheckNet();
		}
		//hide undo button
		GameHandler.instance.isEnableundobutton (false);
		//hide pause button
		GameHandler.instance.isEnablePausebutton (false);
	}

	public void internetCheckOkClicked(){

		AllPopups.instance.internetCheckPopUp.SetActive (false);
	}


	//called on newgamepop up ok button clicked
	public void getcurrentSelectedOption(){
//		print ("get current option");
		GameHandler.instance.ResetGame ();
		AllPopups.instance.closeAllpopUps ();

		switch(currentgameOption){
		case (int) GameModes.SinglePlayerEasy:
			setSinglePlayerInitialParameters();
			break;
		case (int) GameModes.SinglePlayerMedium:
			setSinglePlayerInitialParameters();
			break;
		case (int) GameModes.SinglePlayerHard:
			setSinglePlayerInitialParameters();
			break;
		case (int) GameModes.MultiPlayerAnonymous :
			
			PhotonNetwork.offlineMode = false;
			if(InternetChecker.isInternetOn){
				NetworkObject.SetActive(true);
				if(!PhotonNetwork.connected)
					NetworkManager.instance.ConnectWithPhotonserver();
				NetworkManager.instance.Invoke("createOrJoinRoom",5.0f);
			}
			else{
				AllPopups.instance.newGamePopUp.SetActive (true);
				AllPopups.instance.internetCheckPopUp.SetActive(true);
			}
			//hide undo button
			GameHandler.instance.isEnableundobutton (false);
			//hide pause button
			GameHandler.instance.isEnablePausebutton (false);
			break;
		case (int) GameModes.MultiPlayerFacebookFriend :
			PhotonNetwork.offlineMode = false;
			onFbFriendsClicked();
			break;
		case (int) GameModes.Human :
				setHumanInitialParameters();
			break;
		}

	}
	public void setSinglePlayerInitialParameters(){
		if (PhotonNetwork.connected)
			PhotonNetwork.Disconnect ();

		GameHandler.instance.messageText.GetComponent<Text>().text = "";
		
		NetworkObject.SetActive(true);
		aiPlayer.SetActive(true);
		PhotonNetwork.offlineMode = true;
		NetworkManager.instance.createOrJoinRoom();
		
		GameHandler.instance.StartTime = Time.time;
		GameHandler.isStartGame = true;
		GameHandler.isResultDeclared = false;
		Map.instance.clearUndoList ();
		aiPlayer.GetComponent<AIPlayer> ().resetValues ();
		
		GameHandler.instance.isEnableundobutton (true);
		//is hide pause button
		GameHandler.instance.isEnablePausebutton (true);
	}

	//when there is no player online
	public void setFakePlayerInitialParameters(){
		if (PhotonNetwork.room !=null && PhotonNetwork.room.playerCount == 1)
		{
			//close this room
				PhotonNetwork.room.open = false;
			//change Mode
			NewgameManager.currentgameOption = (int)GameModes.SinglePlayerMedium;

			GameHandler.instance.messageText.GetComponent<Text> ().text = "";
			aiPlayer.SetActive (true);		
			GameHandler.instance.StartTime = Time.time;
			GameHandler.isStartGame = true;
			GameHandler.isResultDeclared = false;
			aiPlayer.GetComponent<AIPlayer> ().resetValues ();
			//Fake Name
			GameHandler.instance.opponentName.text = fakeNamesList [Random.Range (0, fakeNamesList.Length)];
		}
	}		
			
		
	string[] fakeNamesList = {"Barb Dwyer","Terry Aki","Cory Ander","Jimmy Changa","Barry Wine",
		"Wilma Mumduya","Buster Hyman","Poppa Cherry","Zack Lee","Don Stairs",
		"Saul T. Balls","Peter Pants","Hal Appeno","Moe Fugga","Graham Cracker",
		"Tom Foolery","Al Dente","Bud Wiser","Holly Graham","Cam L. Toe",
		"Pat Agonia","Tara Zona","Phil Anthropist","Marvin Gardens","Donatella Nobatti"};
		
	public void setHumanInitialParameters(){
//		print ("set human initial");
		GameHandler.instance.ActivateGameBoard.SetActive(false);
		
		GameObject g = Instantiate(myPlayerPrefab) as GameObject;
		GameHandler.instance.player1 = g.GetComponent<Player>();
		GameHandler.instance.player1.myTokensCount = 0;
		
		GameObject g1 = Instantiate(myPlayerPrefab) as GameObject;
		GameHandler.instance.player2 = g1.GetComponent<Player>();
		GameHandler.instance.player2.myTokensCount = 0;
		GameHandler.instance.currentPlayer = GameHandler.instance.player1;
		
		GameHandler.instance.messageText.GetComponent<Text>().text = "";
		GameHandler.instance.Myturnimg.GetComponent<Image>().sprite = GameHandler.instance.onSprite;
		GameHandler.instance.OpponentTurnimg.GetComponent<Image>().sprite = GameHandler.instance.offSprite;
		GameHandler.instance.StartTime = Time.time;
		GameHandler.isStartGame = true;


		//to give Ranodom token
		int r = Random.Range (0,2);
		if (r == 0) {
			GameHandler.instance.player1.myToken = Resources.Load(GameConstants.BLACK_TOKEN_NAME) as GameObject;
			GameHandler.instance.player2.myToken = Resources.Load(GameConstants.WHITE_TOKEN_NAME) as GameObject;
			GameHandler.instance.MyTokenimg.GetComponent<Image>().sprite = GameHandler.instance.blackTokenSprite;
			GameHandler.instance.OpponentTokenimg.GetComponent<Image>().sprite = GameHandler.instance.whiteTokenSprite;
		}
		else{
			GameHandler.instance.player1.myToken = Resources.Load(GameConstants.WHITE_TOKEN_NAME) as GameObject;
			GameHandler.instance.player2.myToken = Resources.Load(GameConstants.BLACK_TOKEN_NAME) as GameObject;
			GameHandler.instance.MyTokenimg.GetComponent<Image>().sprite = GameHandler.instance.whiteTokenSprite;
			GameHandler.instance.OpponentTokenimg.GetComponent<Image>().sprite = GameHandler.instance.blackTokenSprite;
		}
		//hide undo button
		GameHandler.instance.isEnableundobutton (false);
		//hide pause button
		GameHandler.instance.isEnablePausebutton (true);

	}

	public void cancelNewGameClicked(){
		AllPopups.instance.newGamePopUp.SetActive (false);
		AllPopups.instance.menuPopUp.SetActive (true);
	}

}
