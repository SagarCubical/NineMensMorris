using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDropHandler,IPointerClickHandler {
	
[HideInInspector]	public int 		indexOfSlot;
					public Slot[] 	neibourSlots;
					public bool 	isRemovableToken = false;
					GameHandler gh;


	void Start(){
		indexOfSlot = int.Parse (transform.gameObject.name);
		gh = GameHandler.instance;
	}

	//Check if slot is Empty
	public bool isEmpty(){
		if (transform.childCount == 0)
			return true;
		return false;	

	}

	public bool isNeighbourSlot(){

		int noOfNeighbours = Token.startParent.GetComponent<Slot>().neibourSlots.Length;
		
		for(int i=0;i<noOfNeighbours;i++){
//			print("N indexOfSlot"+indexOfSlot+" "+i+" "+noOfNeighbours);
			if(indexOfSlot == Token.startParent.GetComponent<Slot>().neibourSlots[i].indexOfSlot)
				return true;
		}

		return false;
	}

	//for human
	public bool isTokenCanFly(){

		if (GameHandler.currentGameType == GameType.Fly) {
			//get my token and check if only 3 remain
			if(NewgameManager.currentgameOption == (int)GameModes.Human)
			{
				if(GameHandler.instance.currentPlayer.myToken.tag == GameConstants.BLACK_TOKEN_NAME &&  Map.instance.blackTokensDeleted >= 6)
					return true;
				else if(GameHandler.instance.currentPlayer.myToken.tag == GameConstants.WHITE_TOKEN_NAME && Map.instance.whiteTokensDeleted >= 6) 
						return true;
			}

			if(NewgameManager.currentgameOption == (int)GameModes.MultiPlayerAnonymous || NewgameManager.currentgameOption == (int)GameModes.MultiPlayerFacebookFriend)
			{	
				if(Map.instance.tokenPrefab.tag == GameConstants.BLACK_TOKEN_NAME &&  Map.instance.blackTokensDeleted >= 6)
					return true;
				else if(Map.instance.tokenPrefab.tag == GameConstants.WHITE_TOKEN_NAME && Map.instance.whiteTokensDeleted >= 6) 
					return true;
			}
		} 
		
		return false;	
	}

	#region IDropHandler implementation

	public void OnDrop (PointerEventData eventData)
	{
//		print ("OnDrop");

		if (NewgameManager.currentgameOption == (int)GameModes.MultiPlayerAnonymous  || NewgameManager.currentgameOption == (int)GameModes.MultiPlayerFacebookFriend) {
			if (Map.instance.canMoveOnMap && Token.itemBiengDragged != null) {
				if (isEmpty () && (GameHandler.currentGameType == GameType.NonFly && isNeighbourSlot()||(GameHandler.currentGameType == GameType.Fly && ( isNeighbourSlot() || isTokenCanFly())) )) {

					int viewid = Token.itemBiengDragged.GetComponent<Token> ().myphotonview.viewID;
					Token.itemBiengDragged.GetComponent<Token> ().myphotonview.RPC ("rePositionParent", PhotonTargets.All, indexOfSlot, viewid, Token.itemBiengDragged.tag);

//					print ("MILL Created " + GameHandler.instance.checkForMills (indexOfSlot));
					if (GameHandler.instance.checkForMills (indexOfSlot)) {
						GameHandler.instance.gteRemovableTokensAfterMill ();
						GameHandler.instance.indicateRemovableToken ();
						Map.instance.isMillCreated = true;
					} else
						Token.itemBiengDragged.GetComponent<Token> ().myphotonview.RPC ("setMyTurn", PhotonTargets.All, null);


				} 
			}
		}else
		if(NewgameManager.currentgameOption == (int)GameModes.Human){

			if (GameHandler.instance.currentPlayer.canMove  && Token.itemBiengDragged != null) {
//				print(""+isNeighbourSlot()+""+isTokenCanFly()+""+GameHandler.currentGameType);
				if(isEmpty() && ( (GameHandler.currentGameType == GameType.NonFly && isNeighbourSlot())||(GameHandler.currentGameType == GameType.Fly && ( isNeighbourSlot() || isTokenCanFly())) ))
				{

					Token.itemBiengDragged.transform.SetParent (transform);
//					print("MILL Created "+GameHandler.instance.checkForMills(indexOfSlot));
					if(GameHandler.instance.checkForMills(indexOfSlot)){
						GameHandler.instance.gteRemovableTokensAfterMill();
						GameHandler.instance.indicateRemovableToken();
						Map.instance.isMillCreated = true;
					}else
						setHumanTurn();

				} 
			}
		}else 
		if ((NewgameManager.currentgameOption == (int)GameModes.SinglePlayerEasy  || NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium ||NewgameManager.currentgameOption == (int)GameModes.SinglePlayerHard) && GameHandler.isMyTurn) {
			if (Map.instance.canMoveOnMap && Token.itemBiengDragged != null) {
				if (isEmpty () && (GameHandler.currentGameType == GameType.NonFly && isNeighbourSlot()||(GameHandler.currentGameType == GameType.Fly && ( isNeighbourSlot() || isTokenCanFly())) )) {
					//reset parent
					Token.itemBiengDragged.transform.SetParent (transform);
					Map.instance.save.toSlot = indexOfSlot;
//					print ("MILL Created " + GameHandler.instance.checkForMills (indexOfSlot));
					if (GameHandler.instance.checkForMills (indexOfSlot)) {
						GameHandler.instance.gteRemovableTokensAfterMill ();
						GameHandler.instance.indicateRemovableToken ();
						Map.instance.isMillCreated = true;
						Map.instance.save.isMillCreated = true;
						GameHandler.instance.canUndo = false;
					} else{
						Map.instance.save.isMillCreated = false;
						Token.itemBiengDragged.GetComponent<Token> ().myphotonview.RPC ("setMyTurn", PhotonTargets.All, null);
					}
				} 
			}
		}
	}

	#endregion

	#region IPointerClickHandler implementation
	public void OnPointerClick (PointerEventData eventData)
	{
		//For Anonymous
		if (NewgameManager.currentgameOption == (int)GameModes.MultiPlayerAnonymous  || NewgameManager.currentgameOption == (int)GameModes.MultiPlayerFacebookFriend) {

			//To create new Token in the start of game
			if (Map.instance.noOfTokens < GameConstants.MAX_NO_OF_TOKENS && isEmpty () && !Map.instance.canMoveOnMap && !Map.instance.isMillCreated) {
				GameObject token;
				token = PhotonNetwork.Instantiate (Map.instance.tokenPrefab.name, Vector3.zero, Quaternion.identity, 0);

//				print ("Id  " + token.GetComponent<Token> ().myphotonview.viewID + " " + indexOfSlot);
				int viewid = token.GetComponent<Token> ().myphotonview.viewID;

				token.GetComponent<Token> ().myphotonview.RPC ("setTokenParent", PhotonTargets.All, indexOfSlot);

				Map.instance.noOfTokens++;
				if (Map.instance.noOfTokens == GameConstants.MAX_NO_OF_TOKENS)
					Map.instance.canMoveOnMap = true;

				if (GameHandler.instance.checkForMills (indexOfSlot)) {
					GameHandler.instance.gteRemovableTokensAfterMill ();
					GameHandler.instance.indicateRemovableToken ();
					Map.instance.isMillCreated = true;
				} else
					token.GetComponent<Token> ().myphotonview.RPC ("setMyTurn", PhotonTargets.All, null);
			
			}

		} else if (NewgameManager.currentgameOption == (int)GameModes.Human) {
		
			if (isRemovableToken) {
				transform.GetChild (0).gameObject.GetComponent<Token> ().removeToken (indexOfSlot);
				//set turn
				setHumanTurn ();
			}
			
			//To create new Token in the start of game
			if (gh.currentPlayer.myTokensCount < GameConstants.MAX_NO_OF_TOKENS && isEmpty () && !gh.currentPlayer.canMove && !Map.instance.isMillCreated) {
				GameObject token;

				token = Instantiate (gh.currentPlayer.myToken, Vector3.zero, Quaternion.identity) as GameObject;
				token.transform.SetParent (transform);
				token.transform.localScale = Vector3.one;

				gh.currentPlayer.myTokensCount++;
				if (gh.currentPlayer.myTokensCount == GameConstants.MAX_NO_OF_TOKENS)
					gh.currentPlayer.canMove = true;
				
//				print ("MILL Created " + GameHandler.instance.checkForMills (indexOfSlot));
				if (GameHandler.instance.checkForMills (indexOfSlot)) {
					GameHandler.instance.gteRemovableTokensAfterMill ();
					GameHandler.instance.indicateRemovableToken ();
					Map.instance.isMillCreated = true;
				} else {
					//setNext turn
					setHumanTurn ();
				}
				

			}
		} else if(NewgameManager.currentgameOption == (int)GameModes.SinglePlayerEasy || NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium ||NewgameManager.currentgameOption == (int)GameModes.SinglePlayerHard){
		
			if (isRemovableToken) {
				transform.GetChild (0).gameObject.GetComponent<Token> ().removeToken(indexOfSlot);
				Map.instance.save.deleteSlot = indexOfSlot;
				transform.GetChild (0).gameObject.GetComponent<Token> ().setMyTurn();
				GameHandler.instance.canUndo = true;
			}
			
			//To create new Token in the start of game
			if (Map.instance.noOfTokens < GameConstants.MAX_NO_OF_TOKENS && isEmpty () && !Map.instance.canMoveOnMap && !Map.instance.isMillCreated) {
				GameObject token;
				token = PhotonNetwork.Instantiate (Map.instance.tokenPrefab.name, Vector3.zero, Quaternion.identity, 0);
							
				token.GetComponent<Token> ().setTokenParent(indexOfSlot);
				Map.instance.save.canMove = Map.instance.canMoveOnMap;
				Map.instance.save.toSlot = indexOfSlot;
				
				Map.instance.noOfTokens++;

				if (Map.instance.noOfTokens == GameConstants.MAX_NO_OF_TOKENS)
					Map.instance.canMoveOnMap = true;

				if (GameHandler.instance.checkForMills (indexOfSlot)) {
					GameHandler.instance.gteRemovableTokensAfterMill ();
					GameHandler.instance.indicateRemovableToken ();
					Map.instance.isMillCreated = true;	
					Map.instance.save.isMillCreated = true;
					GameHandler.instance.canUndo = false;
				} else{
					Map.instance.save.isMillCreated = false;
					token.GetComponent<Token> ().setMyTurn();
				}
				
			}
		
		
		}


	}

	#endregion

	public void setHumanTurn(){
		if (gh.currentPlayer == gh.player1) {
			gh.currentPlayer = gh.player2;
			gh.messageText.GetComponent<Text>().text = "";
			gh.Myturnimg.GetComponent<Image>().sprite = gh.offSprite;
			gh.OpponentTurnimg.GetComponent<Image>().sprite = gh.onSprite;
		} else {
			gh.currentPlayer = gh.player1;
			gh.messageText.GetComponent<Text>().text = "";
			gh.Myturnimg.GetComponent<Image>().sprite = gh.onSprite;
			gh.OpponentTurnimg.GetComponent<Image>().sprite = gh.offSprite;
		}
		GameHandler.instance.remainingTime = GameConstants.MAX_TIME_OF_CHANCE;
		GameHandler.instance.StartTime = Time.time;

		if (gh.currentPlayer.canMove) {
			if(!gh.checkifHasFreeTokensToMove(gh.currentPlayer.myToken.tag)){
				AllPopups.instance.winLoosePopUp.SetActive (true);
				if (GameHandler.instance.currentPlayer.myToken.tag == GameConstants.BLACK_TOKEN_NAME)
					GameObject.Find ("WinLooseMsg").GetComponent<Text> ().text = "White Win... ";
				else
					GameObject.Find ("WinLooseMsg").GetComponent<Text> ().text = "Black Win... ";
				GameHandler.instance.ResetGame ();	
				myAd.instance.showInterAd();
			}
		}
	}

}
