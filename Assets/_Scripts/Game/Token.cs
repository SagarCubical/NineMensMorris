using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Token : Photon.MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler {

	public static GameObject itemBiengDragged;
	Vector3 startPosition;
	public static Transform startParent;
	public static int ownerIndex;
	private bool resetToken = false;
	float step = 0 , smoothValue = 4.0f;

	void Update(){
		ownerIndex = transform.parent.GetComponent<Slot> ().indexOfSlot;
		if (resetToken) {
			step += Time.deltaTime * smoothValue ;
			transform.position = Vector3.Lerp(transform.position , startPosition , step);
			if(step >= 1)
				resetToken = false;
		}
	}

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
		if (!Map.instance.isMillCreated &&
		    (((NewgameManager.currentgameOption == (int)GameModes.MultiPlayerAnonymous  || NewgameManager.currentgameOption == (int)GameModes.MultiPlayerFacebookFriend)&& Map.instance.canMoveOnMap && transform.gameObject.tag == Map.instance.tokenPrefab.tag) ||
		 	(NewgameManager.currentgameOption == (int)GameModes.Human && GameHandler.instance.currentPlayer.canMove && transform.gameObject.tag == GameHandler.instance.currentPlayer.myToken.tag))) {
			itemBiengDragged = transform.gameObject;
			startPosition = transform.position;
			startParent = transform.parent;
			GetComponent<CanvasGroup> ().blocksRaycasts = false;
		} else 
		if ((NewgameManager.currentgameOption == (int)GameModes.SinglePlayerEasy || NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium ||NewgameManager.currentgameOption == (int)GameModes.SinglePlayerHard) && !Map.instance.isMillCreated) { //for other modes single player

			if(Map.instance.canMoveOnMap && GameHandler.isMyTurn && transform.gameObject.tag == Map.instance.tokenPrefab.tag)
			{
				Map.instance.save.canMove = Map.instance.canMoveOnMap;
				Map.instance.save.fromSlot = transform.parent.GetComponent<Slot>().indexOfSlot;

				itemBiengDragged = transform.gameObject;
				startPosition = transform.position;
				startParent = transform.parent;
				GetComponent<CanvasGroup> ().blocksRaycasts = false;
			}
		}
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		if(!Map.instance.isMillCreated && 
		   (((NewgameManager.currentgameOption == (int)GameModes.MultiPlayerAnonymous  || NewgameManager.currentgameOption == (int)GameModes.MultiPlayerFacebookFriend)&& Map.instance.canMoveOnMap && transform.gameObject.tag == Map.instance.tokenPrefab.tag) ||
		 ((NewgameManager.currentgameOption == (int)GameModes.SinglePlayerEasy  || NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium ||NewgameManager.currentgameOption == (int)GameModes.SinglePlayerHard) && Map.instance.canMoveOnMap && GameHandler.isMyTurn && transform.gameObject.tag == Map.instance.tokenPrefab.tag)||
		 ( NewgameManager.currentgameOption == (int)GameModes.Human && GameHandler.instance.currentPlayer.canMove &&transform.gameObject.tag == GameHandler.instance.currentPlayer.myToken.tag))) 
			transform.position = Input.mousePosition;
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{

			itemBiengDragged = null;
			GetComponent<CanvasGroup> ().blocksRaycasts = true;
		if (!Map.instance.isMillCreated && 
		    (( (NewgameManager.currentgameOption == (int)GameModes.MultiPlayerAnonymous  || NewgameManager.currentgameOption == (int)GameModes.MultiPlayerFacebookFriend) && Map.instance.canMoveOnMap && transform.gameObject.tag == Map.instance.tokenPrefab.tag) ||
		 (( NewgameManager.currentgameOption == (int)GameModes.SinglePlayerEasy  || NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium ||NewgameManager.currentgameOption == (int)GameModes.SinglePlayerHard)&& GameHandler.isMyTurn  && Map.instance.canMoveOnMap && transform.gameObject.tag == Map.instance.tokenPrefab.tag) ||
		 (NewgameManager.currentgameOption == (int)GameModes.Human && GameHandler.instance.currentPlayer.canMove && transform.gameObject.tag == GameHandler.instance.currentPlayer.myToken.tag))) {

			if (transform.parent == startParent){
				resetToken = true;
				step = 0;
			}
		}
	}

	#endregion
	//photon 

	public PhotonView myphotonview;

	void Awake(){
		myphotonview = PhotonView.Get (this);
	}

	[RPC]
	public void setTokenParent(int indexOfParentSlot){
		transform.SetParent (GameHandler.instance.getSlotOfIndex(indexOfParentSlot).gameObject.transform);
		transform.localScale = Vector3.one;
	}
	[RPC]
	public void removeToken(int indexOfParentSlot){
		GameObject g = GameHandler.instance.getSlotOfIndex (indexOfParentSlot).gameObject;

		//updtae capture data
		if (g.transform.GetChild (0).gameObject.tag == GameConstants.BLACK_TOKEN_NAME) {
			Map.instance.blackTokensDeleted++;
		}
		Destroy(g.transform.GetChild(0).gameObject);
		GameHandler.instance.normalizeRemovableToken();

		//CheckWinLoose
		if(Map.instance.blackTokensDeleted >= 7 || Map.instance.whiteTokensDeleted >= 7 ){
			AllPopups.instance.winLoosePopUp.SetActive(true);
			if(NewgameManager.currentgameOption == (int)GameModes.MultiPlayerAnonymous  || NewgameManager.currentgameOption == (int)GameModes.MultiPlayerFacebookFriend){
				if(Map.instance.tokenPrefab.tag == GameConstants.BLACK_TOKEN_NAME)
				{
					if(Map.instance.whiteTokensDeleted >= 7){
						GameHandler.instance.setScore(GameConstants.WIN_POINTS);
						GameObject.Find("WinLooseMsg").GetComponent<Text>().text = "You Win... ";
					}
				}else if(Map.instance.tokenPrefab.tag == GameConstants.WHITE_TOKEN_NAME){
					if(Map.instance.blackTokensDeleted >= 7){
						GameHandler.instance.setScore(GameConstants.WIN_POINTS);
						GameObject.Find("WinLooseMsg").GetComponent<Text>().text = "You Win... ";
					}
				}
				
				NetworkManager.instance.removePlayerFromRoom();

			}else if(NewgameManager.currentgameOption == (int)GameModes.SinglePlayerEasy || NewgameManager.currentgameOption == (int)GameModes.SinglePlayerMedium ||NewgameManager.currentgameOption == (int)GameModes.SinglePlayerHard){
					if(GameHandler.isMyTurn)
						GameHandler.instance.setScore(GameConstants.WIN_POINTS);
					else
						GameHandler.instance.setScore(GameConstants.LOOSE_POINTS);

					if(Map.instance.whiteTokensDeleted >= 7)
						GameObject.Find("WinLooseMsg").GetComponent<Text>().text ="Black Win... ";
					else
						GameObject.Find("WinLooseMsg").GetComponent<Text>().text ="White Win... ";	
				NetworkManager.instance.removePlayerFromRoom();
			}
			GameHandler.instance.ResetGame();
			GameHandler.isResultDeclared=true;
			GameHandler.isStartGame = false;
			myAd.instance.showInterAd();
		}

	}

	[RPC]
	public void rePositionParent(int indexOfParentSlot,int pViewId,string tokenName){

		GameObject tokenToReposition = null;
		GameObject[] allTokensOnBoard = GameObject.FindGameObjectsWithTag (tokenName);
		for (int i=0; i<allTokensOnBoard.Length; i++) {
				if(allTokensOnBoard[i].GetComponent<PhotonView>().viewID == pViewId)
					tokenToReposition = allTokensOnBoard[i];
		}
		tokenToReposition.transform.SetParent (GameHandler.instance.getSlotOfIndex(indexOfParentSlot).transform);

	}

	[RPC]
	public void setMyTurn(){
		//before changing turn save turn data
		Map.instance.saveUndoData ();

		if(myphotonview.isMine)
			GameHandler.isMyTurn = !GameHandler.isMyTurn;
		else
			GameHandler.isMyTurn = !GameHandler.isMyTurn;
		GameHandler.instance.remainingTime = GameConstants.MAX_TIME_OF_CHANCE;
		if(NewgameManager.currentgameOption == (int)GameModes.MultiPlayerAnonymous  || NewgameManager.currentgameOption == (int)GameModes.MultiPlayerFacebookFriend)
			GameHandler.instance.StartTime = PhotonNetwork.time;
		else
			GameHandler.instance.StartTime = Time.time;
			
	}

	void checkbolckCond(){
		if(Map.instance.canMoveOnMap && !GameHandler.instance.checkifHasFreeTokensToMove(Map.instance.tokenPrefab.tag))
			NetworkManager.instance.photonView.RPC("timeOutResultDeclaration",PhotonTargets.All);

	}
}
