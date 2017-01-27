using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Save{
	public  bool canMove;
	public  int toSlot;
	public  int fromSlot;
	public  bool isMillCreated;
	public  int deleteSlot;
}

public class Map : MonoBehaviour {
	public static Map instance;

	public GameObject	tokenPrefab;
	public	int 		noOfTokens;	
	public	int 		blackTokensDeleted,whiteTokensDeleted;	
	public	int 		lastMoveIndex;
	public bool			canMoveOnMap;
	public bool			isMillCreated;
	public Save save;

	void Awake(){
		instance = this;
		blackTokensDeleted = 0;
		whiteTokensDeleted = 0;
	}

	// Use this for initialization
	void Start () {
		save = new Save ();
	}

	List<Save> savedDataList = new List<Save>();
	public void saveUndoData(){
		savedDataList.Add (save);
		save = new Save ();
	}

	public void clearUndoList(){
		savedDataList.Clear ();
	}

	public void undoButtonClicked(){
		//last is of ai then mine in single player took out 2 Nodes
		int currentUndoAvail = PlayerPrefs.GetInt (GameConstants.KEY_NO_OF_UNDO_COUNTS);
		if(currentUndoAvail>0){
			currentUndoAvail--;
			PlayerPrefs.SetInt(GameConstants.KEY_NO_OF_UNDO_COUNTS,currentUndoAvail);
			PlayerPrefs.Save();
		int noOfTime = 2;
		//for single player run loop 2(for player and ai) human 1
		for (int i=0; i<noOfTime && savedDataList.Count>0 && GameHandler.instance.canUndo; i++) {
			int lastData = savedDataList.Count - 1;			
			//for placing
			if (!savedDataList [lastData].canMove) {
				if (i % 2 == 0) {//for Ai
					//remove ai token from to slot
					Destroy (GameHandler.instance.getSlotOfIndex (savedDataList [lastData].toSlot).transform.GetChild (0).gameObject);
					AIPlayer.instance.noOfTokens--;

					if (savedDataList [lastData].isMillCreated) {
						//regenerateopponent token
						GameObject token;
						token = Instantiate (tokenPrefab, Vector3.zero, Quaternion.identity) as GameObject;
						token.GetComponent<Token> ().setTokenParent (GameHandler.instance.getSlotOfIndex (savedDataList [lastData].deleteSlot).indexOfSlot);

						if (tokenPrefab.gameObject.tag == GameConstants.BLACK_TOKEN_NAME) 
							Map.instance.blackTokensDeleted--;
						else if (tokenPrefab.gameObject.tag == GameConstants.WHITE_TOKEN_NAME) 
							Map.instance.whiteTokensDeleted--;
					}
					//remove undo data node from list
					savedDataList.RemoveAt (lastData);
				} else {
					if (noOfTokens == GameConstants.MAX_NO_OF_TOKENS)
						canMoveOnMap = false;
					//remove ai token from to slot
					Destroy (GameHandler.instance.getSlotOfIndex (savedDataList [lastData].toSlot).transform.GetChild (0).gameObject);
					noOfTokens--;

					if (savedDataList [lastData].isMillCreated) {
						//regenerateopponent token
						GameObject token;
						token = Instantiate (AIPlayer.instance.tokenPrefab, Vector3.zero, Quaternion.identity) as GameObject;
						token.GetComponent<Token> ().setTokenParent (GameHandler.instance.getSlotOfIndex (savedDataList [lastData].deleteSlot).indexOfSlot);

						if (AIPlayer.instance.tokenPrefab.gameObject.tag == GameConstants.BLACK_TOKEN_NAME) 
							Map.instance.blackTokensDeleted--;
						else if (AIPlayer.instance.tokenPrefab.gameObject.tag == GameConstants.WHITE_TOKEN_NAME) 
							Map.instance.whiteTokensDeleted--;
					}
					//remove undo data node from list
					savedDataList.RemoveAt (lastData);
				}
			} else {
				//move token from toSlot to fromSlot
				Slot toslot = GameHandler.instance.getSlotOfIndex (savedDataList [lastData].toSlot);
				Slot frmslot = GameHandler.instance.getSlotOfIndex (savedDataList [lastData].fromSlot);
				toslot.transform.GetChild (0).transform.SetParent (frmslot.transform);

				if (savedDataList [lastData].isMillCreated) {
					//regenerateopponent token
					GameObject token;

					if (i % 2 == 0) {
						token = Instantiate (tokenPrefab, Vector3.zero, Quaternion.identity) as GameObject;
						token.GetComponent<Token> ().setTokenParent (GameHandler.instance.getSlotOfIndex (savedDataList [lastData].deleteSlot).indexOfSlot);

						if (tokenPrefab.gameObject.tag == GameConstants.BLACK_TOKEN_NAME) 
							Map.instance.blackTokensDeleted--;
						else if (tokenPrefab.gameObject.tag == GameConstants.WHITE_TOKEN_NAME) 
							Map.instance.whiteTokensDeleted--;
					
					} else {
						token = Instantiate (AIPlayer.instance.tokenPrefab, Vector3.zero, Quaternion.identity) as GameObject;
						token.GetComponent<Token> ().setTokenParent (GameHandler.instance.getSlotOfIndex (savedDataList [lastData].deleteSlot).indexOfSlot);
				
						if (AIPlayer.instance.tokenPrefab.gameObject.tag == GameConstants.BLACK_TOKEN_NAME) 
							Map.instance.blackTokensDeleted--;
						else if (AIPlayer.instance.tokenPrefab.gameObject.tag == GameConstants.WHITE_TOKEN_NAME) 
							Map.instance.whiteTokensDeleted--;
					}
				}
				
				//remove undo data node from list
				savedDataList.RemoveAt (lastData);

			}
		}
	}
	}

}
