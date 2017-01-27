using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
//using System;

public static class GameConstants{
	
	public static int MAX_NO_OF_TOKENS 				= 	9;
	public static int MIN_NO_OF_FRIEND_INVITES 		= 	0;//1
	public static int MAX_TIME_OF_CHANCE 			= 	30;//1
	public static int WIN_POINTS 					=	1;//100;//1
	public static int LOOSE_POINTS					= 	0;//-50;//1
	public static float UPDATE_FRIEND_LIST_TIME 	= 	12f;//in seconds, should be >11
	public static string BLACK_TOKEN_NAME 			= 	"TokenBlack";//1
	public static string WHITE_TOKEN_NAME 			= 	"TokenWhite";//1
	public static string FACEBOOK_UNDO_OBJECT_ID 	= 	"";//1
	
	public static string KEY_PLAYER_PREFERENCES_INITIALIZED 		= 	"key_player_pref_initialized";
	public static string KEY_NAME_OF_USER 							= 	"key_name_of_user";
	public static string KEY_EMAIL_OF_USER 							= 	"key_email_of_user";
	public static string KEY_FBID_OF_USER 							= 	"key_fbid_of_user";
	public static string KEY_UNIQUEID_OF_USER 						= 	"key_uniqueid_of_user";
	public static string KEY_NO_OF_UNDO_COUNTS			 			= 	"key_no_of_undo_counts";
//	public static string KEY_NO_OF_GAME_WINS			 			= 	"key_no_of_game_wins";
	public static string key_playerPrefs_Remove_Ads 				= 	"key_playerPrefs_Remove_Ads";
}


public class GameHandler : MonoBehaviour {
	
	public static GameHandler instance;
	public static int myTotalScore, globalHighScore;
	
	public static bool isMyTurn;
	public static bool isStartGame;
	public static bool isResultDeclared = false;
	public static bool isFrndListCreated;
	public static GameType currentGameType;
	public GameObject ActivateGameBoard;
	public GameObject messageText;
	public GameObject Myturnimg,OpponentTurnimg;
	public GameObject MyTokenimg,OpponentTokenimg;
	public GameObject timerText;
	public GameObject undoButton,pauseButton;
	
	public Text myName;
	public Text opponentName;
	
	public Image timerImage;
	
	public Sprite onSprite,offSprite;
	public Text blackCount,whiteCount;
	public Sprite blackTokenSprite,whiteTokenSprite,emptyTokenSprite;

	public bool canUndo = true;
	public bool isGamePaused = false;
	
	
	int[,] mills = new int[16,3]{
		{0,1,2},{3,4,5},{6,7,8},{9,10,11},
		{12,13,14},{15,16,17},{18,19,20},{21,22,23},
		{0,9,21},{3,10,18},{6,11,15},{1,4,7},
		{16,19,22},{8,12,17},{5,13,20},{2,14,23}
	};
	public Slot[] allSlots;
	public List<Slot> millElementsList, nonMillElemntsList;
	
	//	Human extra parameters
	
	public Player player1,player2;
	public Player currentPlayer;
	
	void Awake(){
		instance = this;
	}

	public static void printText(string text){
		print (text);
	}
	
	// Use this for initialization
	void Start () {
		initializePlayerPreferences ();

		GameObject[] slotObjects = GameObject.FindGameObjectsWithTag ("Slot");
		allSlots = new Slot[slotObjects.Length];
		for(int m=0;m<allSlots.Length;m++){
			allSlots[m] = slotObjects[m].GetComponent<Slot>();
		}
//		print ("current score "+myTotalScore);
		isFrndListCreated = false;
	}

	public void isEnableundobutton(bool showBtn){
		if (showBtn) 
			undoButton.SetActive (true);
		else 
			undoButton.SetActive (false);
	}
	public void isEnablePausebutton(bool showBtn){
		if (showBtn) 
			pauseButton.SetActive (true);
		else 
			pauseButton.SetActive (false);
		
	}
	
	public void ResetGame(){
		
		normalizeRemovableToken ();
		StopAllCoroutines ();
		CancelInvoke ("easyAiTurn");
		Map.instance.noOfTokens = 0;
		messageText.GetComponent<Text>().text = "";
		Myturnimg.GetComponent<Image>().sprite = offSprite;
		OpponentTurnimg.GetComponent<Image>().sprite = offSprite;
		MyTokenimg.GetComponent<Image>().sprite = emptyTokenSprite;
		OpponentTokenimg.GetComponent<Image>().sprite = emptyTokenSprite;
		Map.instance.blackTokensDeleted = 0;
		Map.instance.whiteTokensDeleted = 0;
		Map.instance.canMoveOnMap = false;
		Map.instance.isMillCreated = false;
		ActivateGameBoard.SetActive (true);
		//		currentTimeRemain = 0;
		remainingTime = 0;
		GameHandler.instance.timerText.GetComponent<Text>().text = ""+remainingTime;
		GameHandler.instance.opponentName.text = "Opponent :" ;
		timerImage.fillAmount = 1f;
		isStartGame = false;
		//human
		
		deleteAlltokens ();
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		foreach(GameObject g in players){
			Destroy(g);
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (!isGamePaused) {
			if (NewgameManager.currentgameOption == (int)GameModes.MultiPlayerAnonymous || NewgameManager.currentgameOption == (int)GameModes.MultiPlayerFacebookFriend) {
				if (isStartGame) {
				
					if (isMyTurn) {
						ActivateGameBoard.SetActive (false);
						messageText.GetComponent<Text> ().text = "";
						Myturnimg.GetComponent<Image> ().sprite = onSprite;
						OpponentTurnimg.GetComponent<Image> ().sprite = offSprite;
					} else { 
						ActivateGameBoard.SetActive (true);
						messageText.GetComponent<Text> ().text = "";
						Myturnimg.GetComponent<Image> ().sprite = offSprite;
						OpponentTurnimg.GetComponent<Image> ().sprite = onSprite;
					}
					checkWinLooseCondtions ();
					setTime ();
				
				} else 
					GameHandler.instance.messageText.GetComponent<Text> ().text = "Waiting for a Player...";
			} else 
		if (NewgameManager.currentgameOption == (int)GameModes.Human) {
			if(isStartGame)
				setLocalTime ();
		} else
		if (NewgameManager.currentgameOption == (int)GameModes.SinglePlayerEasy || NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium || NewgameManager.currentgameOption == (int)GameModes.SinglePlayerHard) {
				if (isStartGame)
					setLocalTime ();
				if (isStartGame && isMyTurn) {
					ActivateGameBoard.SetActive (false);
					messageText.GetComponent<Text> ().text = "";
					Myturnimg.GetComponent<Image> ().sprite = onSprite;
					OpponentTurnimg.GetComponent<Image> ().sprite = offSprite;
				}

				GameHandler.instance.messageText.GetComponent<Text> ().text = "Current Undo Moves: "+PlayerPrefs.GetInt(GameConstants.KEY_NO_OF_UNDO_COUNTS);
			
			}
			blackCount.text = Map.instance.blackTokensDeleted + "";
			whiteCount.text = Map.instance.whiteTokensDeleted + "";
		}

		if (!AllPopups.instance.ifOngameplay ()) {
			if(PhotonNetwork.room != null){
				NetworkManager.instance.removePlayerFromRoom();
				ResetGame();
			}
		}
	}
	int SecondsPerTurn = 30;                  // time per round/turn
	public double StartTime; 
	public double remainingTime;
	
	void setTime(){
		
		
		double elapsedTime = (PhotonNetwork.time - StartTime);
		remainingTime = SecondsPerTurn - (elapsedTime % SecondsPerTurn);
		
		GameHandler.instance.timerText.GetComponent<Text>().text = ""+(int)remainingTime;
		timerImage.fillAmount = ((float)remainingTime/(float)GameConstants.MAX_TIME_OF_CHANCE);
		
		if (remainingTime < 0.1) {
			NetworkManager.instance.photonView.RPC("timeOutResultDeclaration",PhotonTargets.All);
		}
		
	}
	
	
	void setLocalTime(){
		
		
		double elapsedTime = (Time.time - StartTime);
		remainingTime = SecondsPerTurn - (elapsedTime % SecondsPerTurn);
		
		GameHandler.instance.timerText.GetComponent<Text>().text = ""+(int)remainingTime;
		timerImage.fillAmount = ((float)remainingTime/(float)GameConstants.MAX_TIME_OF_CHANCE);
		
		if (remainingTime < 0.1) {
			AllPopups.instance.winLoosePopUp.SetActive (true);
			if (NewgameManager.currentgameOption == (int)GameModes.Human){
				if (GameHandler.instance.currentPlayer.myToken.tag == GameConstants.BLACK_TOKEN_NAME){
					GameObject.Find ("WinLooseMsg").GetComponent<Text> ().text = "White Win... ";
				}
				else
					GameObject.Find ("WinLooseMsg").GetComponent<Text> ().text = "Black Win... ";
			}
			else if (NewgameManager.currentgameOption == (int)GameModes.SinglePlayerEasy || NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium ||NewgameManager.currentgameOption == (int)GameModes.SinglePlayerHard)
			{
				if(isMyTurn){
					setScore(GameConstants.LOOSE_POINTS);
					if (Map.instance.tokenPrefab.tag == GameConstants.BLACK_TOKEN_NAME)
						GameObject.Find ("WinLooseMsg").GetComponent<Text> ().text = "White Win... ";
					else
						GameObject.Find ("WinLooseMsg").GetComponent<Text> ().text = "Black Win... ";
				}else{
					setScore(GameConstants.WIN_POINTS);
					if (AIPlayer.instance.tokenPrefab.tag == GameConstants.BLACK_TOKEN_NAME)
						GameObject.Find ("WinLooseMsg").GetComponent<Text> ().text = "White Win... ";
					else
						GameObject.Find ("WinLooseMsg").GetComponent<Text> ().text = "Black Win... ";
				}
			}
			GameHandler.instance.ResetGame ();	
			myAd.instance.showInterAd();
		}
	}
	
	
	
	
	void checkWinLooseCondtions(){
		
		if (PhotonNetwork.room.playerCount == 1 && !isResultDeclared) {
//			print("checkWinLooseCondtions Inside");
			ActivateGameBoard.SetActive (true);
			isResultDeclared=true;
			ResetGame();
			NetworkManager.instance.removePlayerFromRoom();
			isStartGame = false;
			AllPopups.instance.winLoosePopUp.SetActive(true);
			setScore(GameConstants.WIN_POINTS);
			GameObject.Find("WinLooseMsg").GetComponent<Text>().text = "You Win... Opponent left";
			myAd.instance.showInterAd();
		}
	}

	public void setScore(int scorePoint){
		myTotalScore += scorePoint;
		//update Score on database
		//set for winning only
		if (InternetChecker.isInternetOn) {
			DataManager.instance.setUserScoreClicked(myTotalScore);
			if(myTotalScore%25==0 && myTotalScore!=125 && myTotalScore<=150){
				//give player acheivement points
				AcheivementsManager.instance.giveRewardForAcheivement(myTotalScore);
			}
		}
	}
	
	public void deleteAlltokens(){
		
		GameObject[] alltokenToDelete= GameObject.FindGameObjectsWithTag(GameConstants.BLACK_TOKEN_NAME);
		for(int i=0;i<alltokenToDelete.Length;i++)
			Destroy(alltokenToDelete[i]);
		
		GameObject[] alltokenToDelete1= GameObject.FindGameObjectsWithTag(GameConstants.WHITE_TOKEN_NAME);
		for(int i=0;i<alltokenToDelete1.Length;i++)
			Destroy(alltokenToDelete1[i]);
		
	}
	
	public bool checkForMills(int elementIndex){
		
		bool isMill = true;
		bool isContainLastIndex = false;
		
		for(int i=0;i<16;i++){
			for(int j=0;j<3;j++){
				if(mills[i,j]==elementIndex )
				{
					isMill = true;
					for(int k=0;k<3;k++)
					{
						Slot s = getSlotOfIndex(mills[i,k]);
						if(s.isEmpty())
						{
							isMill = false;
							break;
						}
						else{
							
							if((NewgameManager.currentgameOption == (int)GameModes.MultiPlayerAnonymous  || NewgameManager.currentgameOption == (int)GameModes.MultiPlayerFacebookFriend) &&(s.transform.GetChild(0).tag != Map.instance.tokenPrefab.tag)){
								isMill = false;
								break;
							}
							else if((NewgameManager.currentgameOption == (int)GameModes.SinglePlayerEasy || NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium ||NewgameManager.currentgameOption == (int)GameModes.SinglePlayerHard)){

								if(!GameHandler.isMyTurn && (s.transform.GetChild(0).tag != AIPlayer.instance.tokenPrefab.tag)){
									isMill = false;
									break;
								}
							}
						}
						
						if(k==2 && isMill)
							return isMill;
					}
				}
			}
		}
		return isMill;
	}
	
	//which token tag at which index will have to check for mill
	public bool checkIfCanMill(int elementIndex, string toKenTag){
		
		bool isMill = true;
		bool isContainLastIndex = false;
		
		for(int i=0;i<16;i++){
			for(int j=0;j<3;j++){
				if(mills[i,j]==elementIndex )
				{
					isMill = true;
				}
			}
		}
		return isMill;
	}
	
	public bool checkIfOnMoveCanMill(int elementIndex, string toKenTag, int currentPos){
		
		bool isMill = true;
		bool isContainLastIndex = false;
		
		for(int i=0;i<16;i++){
			for(int j=0;j<3;j++){
				if(mills[i,j]==elementIndex)
				{
					isMill = true;
					for(int k=0;k<3;k++)
					{
						Slot s = getSlotOfIndex(mills[i,k]);
						
						if((NewgameManager.currentgameOption == (int)GameModes.SinglePlayerEasy || NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium ||NewgameManager.currentgameOption == (int)GameModes.SinglePlayerHard)){
							
							if(s.indexOfSlot != elementIndex)
							{

								if(s.transform.GetChild(0).tag != toKenTag){
									isMill = false;
									break;
								}
							}
							
						}
						if(k==2 && isMill)
							return isMill;
					}
				}
			}
		}
		return isMill;
	}
	
	public List<List<Slot>> checkIfHaveAnyMill(string toKenTag){
		
		bool isMill = true;
		bool isContainLastIndex = false;
		List<List<Slot>> allMillSlots = new List<List<Slot>> ();
		
		for(int i=0;i<16;i++){
			for(int j=0;j<3;j++){
				isMill = true;
				Slot s = getSlotOfIndex(mills[i,j]);
				
				if((NewgameManager.currentgameOption == (int)GameModes.SinglePlayerEasy || NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium ||NewgameManager.currentgameOption == (int)GameModes.SinglePlayerHard)){							
					if(s.isEmpty())
					{
						isMill = false;
						break;
					}
					if(s.transform.GetChild(0).tag != toKenTag){
						isMill = false;
						break;
					}							
				}
				if(j==2 && isMill){
					List<Slot> myMillSlots = new List<Slot>();
					for(int l=0;l<3;l++)
						myMillSlots.Add(getSlotOfIndex(mills[i,l]));
					allMillSlots.Add(myMillSlots);
				}
			}
		}
		return allMillSlots;
	}
	
	public Slot[] getAllSlotOfMIll(int indexOfElement,string toKenTag){
		
		bool isMill = true;
		bool isContainLastIndex = false;
		List<Slot> allMillSlots = new List<Slot> ();
		
		for(int i=0;i<16;i++){
			for(int j=0;j<3;j++){
				isMill = true;
				Slot s = getSlotOfIndex(mills[i,j]);
				
				if((NewgameManager.currentgameOption == (int)GameModes.SinglePlayerEasy || NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium ||NewgameManager.currentgameOption == (int)GameModes.SinglePlayerHard)){	
					if(s.indexOfSlot != indexOfElement)
					{

						if(s.transform.GetChild(0).tag != toKenTag){
							isMill = false;
							break;
						}	
					}
				}
				
				if(j==2 && isMill){
					for(int l=0;l<3;l++){
						if(getSlotOfIndex(mills[i,l]).indexOfSlot != indexOfElement)
							allMillSlots.Add(getSlotOfIndex(mills[i,l]));
					}
					return allMillSlots.ToArray();
				}
			}
		}
		return allMillSlots.ToArray();
	}
	
	public bool checkMillsForRemoval(int elementIndex){
		
		bool isMill = true;
		bool isContainLastIndex = false;
		
		for(int i=0;i<16;i++){
			for(int j=0;j<3;j++){
				if(mills[i,j]==elementIndex )
				{
					isMill = true;
					for(int k=0;k<3;k++)
					{
						Slot s = getSlotOfIndex(mills[i,k]);
						if(s.isEmpty())
						{
							isMill = false;
							break;
						}
						else{
							if((NewgameManager.currentgameOption == (int)GameModes.MultiPlayerAnonymous  || NewgameManager.currentgameOption == (int)GameModes.MultiPlayerFacebookFriend) && s.transform.GetChild(0).tag == Map.instance.tokenPrefab.tag){
								isMill = false;
								break;
							}
							else if((NewgameManager.currentgameOption == (int)GameModes.SinglePlayerEasy || NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium ||NewgameManager.currentgameOption == (int)GameModes.SinglePlayerHard)){
								if(GameHandler.isMyTurn && (s.transform.GetChild(0).tag == Map.instance.tokenPrefab.tag))
								{
									isMill = false;
									break;
								}
							}
						}
						if(k==2 && isMill)
							return isMill;
					}
				}
			}
		}
		return isMill;
	}
	
	
	public void gteRemovableTokensAfterMill(){
		
		millElementsList.Clear ();
		nonMillElemntsList.Clear ();
		//check if any free token else can break mill
		for(int i=0;i<allSlots.Length;i++){
			if(!getSlotOfIndex(i).isEmpty()){
				if((NewgameManager.currentgameOption == (int)GameModes.MultiPlayerAnonymous  || NewgameManager.currentgameOption == (int)GameModes.MultiPlayerFacebookFriend) && getSlotOfIndex(i).transform.GetChild(0).tag != Map.instance.tokenPrefab.tag)
				{
					if(checkMillsForRemoval(i)){
						millElementsList.Add(getSlotOfIndex(i));
					}
					else{
						nonMillElemntsList.Add(getSlotOfIndex(i));
					}
				}
				else if((NewgameManager.currentgameOption == (int)GameModes.SinglePlayerEasy || NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium ||NewgameManager.currentgameOption == (int)GameModes.SinglePlayerHard)){
					if(GameHandler.isMyTurn && (getSlotOfIndex(i).transform.GetChild(0).tag != Map.instance.tokenPrefab.tag))
					{
						if(checkMillsForRemoval(i)){
							millElementsList.Add(getSlotOfIndex(i));
						}
						else{
							nonMillElemntsList.Add(getSlotOfIndex(i));
						}
					}
					else if(!GameHandler.isMyTurn && (getSlotOfIndex(i).transform.GetChild(0).tag != AIPlayer.instance.tokenPrefab.tag)){
						if(checkMillsForRemoval(i)){
							millElementsList.Add(getSlotOfIndex(i));
						}
						else{
							nonMillElemntsList.Add(getSlotOfIndex(i));
						}
					}
				}
			}
		}
	}
	
	public void indicateRemovableToken(){
		
		if(nonMillElemntsList.Count!=0){
			foreach(Slot s in nonMillElemntsList){
				s.transform.gameObject.GetComponent<Image>().color = new Color(255f,255f,255f,1f);
				s.isRemovableToken = true;
			}
		}else{
			foreach(Slot s in millElementsList){
				s.transform.gameObject.GetComponent<Image>().color = new Color(255f,255f,255f,1f);
				s.isRemovableToken = true;
			}
		}
	}
	public void normalizeRemovableToken(){
//		print ("normalizeRemovableToken "+nonMillElemntsList.Count);
		if(nonMillElemntsList.Count!=0){
			foreach(Slot s in nonMillElemntsList){
				s.transform.gameObject.GetComponent<Image>().color = new Color(255f,255f,255f,0f);
				s.isRemovableToken = false;
			}
		}else{
			foreach(Slot s in millElementsList){
				s.transform.gameObject.GetComponent<Image>().color = new Color(255f,255f,255f,0f);
				s.isRemovableToken = false;
			}
		}
		Map.instance.isMillCreated = false;
	}
	
	
	public Slot getSlotOfIndex(int index){
		
		for(int i=0;i<allSlots.Length;i++){
			if(index == (allSlots[i].indexOfSlot))
				return allSlots[i];
		}
//		print ("Slot not found");
		return null;
	}
	
	void initializePlayerPreferences(){
		
		if (PlayerPrefs.GetInt (GameConstants.KEY_PLAYER_PREFERENCES_INITIALIZED, 1) == 1) {
			
			PlayerPrefs.SetInt(GameConstants.KEY_PLAYER_PREFERENCES_INITIALIZED, 0);
			PlayerPrefs.SetInt (GameConstants.KEY_NO_OF_UNDO_COUNTS , 10);
//			PlayerPrefs.SetInt (GameConstants.KEY_NO_OF_GAME_WINS , 0);
			PlayerPrefs.SetInt (GameConstants.key_playerPrefs_Remove_Ads,0);

			PlayerPrefs.SetString(GameConstants.KEY_NAME_OF_USER, "");
			PlayerPrefs.SetString (GameConstants.KEY_EMAIL_OF_USER , "");
			PlayerPrefs.SetString (GameConstants.KEY_FBID_OF_USER , "");
			PlayerPrefs.SetString (GameConstants.KEY_UNIQUEID_OF_USER , "");


			PlayerPrefs.Save();
		}
		
	}
	
	public static bool isAdShowedOnce = false;
	public void ReplayButtonClicked(){
		isAdShowedOnce = false;
		AllPopups.instance.leaveGameWarningPopUp.SetActive (false);
		AllPopups.instance.winLoosePopUp.SetActive (false);
		ResetGame ();
		if (NewgameManager.currentgameOption == (int)GameModes.MultiPlayerAnonymous) {
			if(!PhotonNetwork.connected)
				NetworkManager.instance.ConnectWithPhotonserver();
			NetworkManager.instance.Invoke("createOrJoinRoom",5.0f);
		}else if(NewgameManager.currentgameOption == (int)GameModes.Human)
			NewgameManager.instance.setHumanInitialParameters();
		else if (NewgameManager.currentgameOption == (int)GameModes.SinglePlayerEasy || NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium ||NewgameManager.currentgameOption == (int)GameModes.SinglePlayerHard)
		{
			NetworkManager.instance.removePlayerFromRoom ();
			if(NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium && !PhotonNetwork.offlineMode){
				NewgameManager.currentgameOption = (int)GameModes.MultiPlayerAnonymous;

				NetworkManager.instance.Invoke("createOrJoinRoom",5.0f);
			}
			else
					NewgameManager.instance.setSinglePlayerInitialParameters ();

		}
		else
			AllPopups.instance.menuPopUp.SetActive (true);
	}
	
	public void MenuWinLooseButtonClicked(){
		isAdShowedOnce = false;
		AllPopups.instance.winLoosePopUp.SetActive (false);
		AllPopups.instance.leaveGameWarningPopUp.SetActive (false);
		AllPopups.instance.menuPopUp.SetActive (true);
		ResetGame ();
		if (NewgameManager.currentgameOption == (int)GameModes.MultiPlayerFacebookFriend || NewgameManager.currentgameOption == (int)GameModes.SinglePlayerEasy || NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium ||NewgameManager.currentgameOption == (int)GameModes.SinglePlayerHard) {
			NetworkManager.instance.removePlayerFromRoom ();
		}
	}
	public void gameBackButtonClicked(){
		if (isStartGame) {
			AllPopups.instance.leaveGameWarningPopUp.SetActive (true);
		} else {
			OkLeaveCurrentGameClicked ();
			NetworkManager.instance.CancelInvoke("createOrJoinRoom");
		}
	}
	public void OkLeaveCurrentGameClicked(){
		
		AllPopups.instance.leaveGameWarningPopUp.SetActive (false);
		if (!isResultDeclared && (NewgameManager.currentgameOption == (int)GameModes.MultiPlayerAnonymous  || NewgameManager.currentgameOption == (int)GameModes.MultiPlayerFacebookFriend || (NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium && !PhotonNetwork.offlineMode )) ) 
		{
			if(isStartGame){
				AllPopups.instance.winLoosePopUp.SetActive (true);
				GameObject.Find ("WinLooseMsg").GetComponent<Text> ().text = "You Lost...";
				setScore(GameConstants.LOOSE_POINTS);
				myAd.instance.showInterAd();
			}else
				AllPopups.instance.menuPopUp.SetActive (true);
			GameHandler.instance.ResetGame ();
			GameHandler.isResultDeclared = true;
			GameHandler.isStartGame = false;
			
			NetworkManager.instance.photonView.RPC ("playerLeftRoom", PhotonTargets.Others, null);
			
			NetworkManager.instance.removePlayerFromRoom ();
		} else {
			AllPopups.instance.menuPopUp.SetActive (true);
			if(NewgameManager.currentgameOption != (int)GameModes.Human)
				NetworkManager.instance.removePlayerFromRoom ();
			GameHandler.instance.ResetGame ();
		}

	}


	public void waitForFrndResponce(){
		StartCoroutine (fbFriendNotResponding());
	}
	public IEnumerator fbFriendNotResponding(){
		yield return new WaitForSeconds (30f);
		if (PhotonNetwork.inRoom && !isStartGame) {
//			print("res invoke");
			AllPopups.instance.facebookFrndNotRespondingPopUp.SetActive (true);
		}
	}

	public void fbFriendNotRespondingYesClicked(){
		AllPopups.instance.facebookFrndNotRespondingPopUp.SetActive (false);
		StartCoroutine (fbFriendNotResponding());
	}

	public void fbFriendNotRespondingNoClicked(){

		GameHandler.instance.ResetGame ();
		GameHandler.isResultDeclared = true;
		GameHandler.isStartGame = false;
		NetworkManager.instance.removePlayerFromRoom ();

		AllPopups.instance.facebookFrndNotRespondingPopUp.SetActive (false);
		AllPopups.instance.menuPopUp.SetActive (true);
	}

	public void gamePausedClicked(){
		Time.timeScale = 0f;
		isGamePaused = true;
		AllPopups.instance.gamePausedPopUp.SetActive (true);
	}
	public void gameResumeClicked(){
		Time.timeScale = 1f;
		isGamePaused = false;
		AllPopups.instance.gamePausedPopUp.SetActive (false);
	}
	public void gamePausedMenuClicked(){
		Time.timeScale = 1f;
		isGamePaused = false;
		AllPopups.instance.gamePausedPopUp.SetActive (false);
		AllPopups.instance.menuPopUp.SetActive (true);
		OkLeaveCurrentGameClicked ();
	}
	//pass token tag to check if user has free token to move 
	public bool checkifHasFreeTokensToMove(string tagOfToken){
		print ("Checking for "+tagOfToken);
		for(int i=0;i<GameHandler.instance.allSlots.Length;i++){
			if(!GameHandler.instance.allSlots[i].isEmpty() && GameHandler.instance.allSlots[i].transform.GetChild(0).tag == tagOfToken){
				Slot[] neigh = GameHandler.instance.allSlots[i].neibourSlots;
				print(" "+GameHandler.instance.allSlots[i].indexOfSlot);
				for (int k=0; k<neigh.Length ; k++) {
					if (neigh [k].isEmpty ()) {
						print("empty "+neigh[k].indexOfSlot+" "+GameHandler.instance.allSlots[i].transform.GetChild(0).tag);
						return true;
					}
				}
			}
		}
		return false;
	} 
}
