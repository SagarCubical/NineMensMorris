using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPlayer : MonoBehaviour {

	public static AIPlayer instance;

	GameHandler gh;
	
	public GameObject	tokenPrefab;
	public	int 		noOfTokens;	
	public bool			isMillCreated;

	Slot[] emptySlots;

	void Awake(){
		instance = this;
	}

	void Start(){
		resetValues ();
		gh = GameHandler.instance;
	}

	public void resetValues(){
//		tokenPrefab = Resources.Load(GameConstants.WHITE_TOKEN_NAME) as GameObject;
		noOfTokens = 0;
		isMillCreated = false;
	}

	//gets all empty slots on board ant store in emptyslots
	void getAllEmptySlots(){
		emptySlots = null;
		List<Slot> emptySlotList = new List<Slot>();
		for(int i=0;i<gh.allSlots.Length;i++)
		{
			if(gh.allSlots[i].isEmpty())
			{
				emptySlotList.Add(gh.allSlots[i]);
			}
		}
	
		emptySlots = emptySlotList.ToArray ();
//		print ("Empty array "+emptySlots.Length);
	}

	//gives index of a slot to place Ai token
	int findBestSlotOnBoard(){
		List<Slot> maxOppList = new List<Slot>();
		int maxNeighbourOpponents = 0;

		//make my mill

		for (int i=0; i<emptySlots.Length; i++) {
//			print ("" + emptySlots [i].indexOfSlot + " " + gh.checkIfCanMill (emptySlots [i].indexOfSlot, tokenPrefab.tag));
			if (gh.checkIfCanMill (emptySlots [i].indexOfSlot, tokenPrefab.tag)) {
				return emptySlots [i].indexOfSlot;
			}
		}
		//check if opp is making any mill anywhere to block opp
		int index = 0;
		int randomno = Random.Range (0, 100);
//		print ("randomno "+randomno+" "+probValue+"");
		if (randomno <= probValue) {
			for (int i=0; i<emptySlots.Length; i++) {
				//block if oppont is making mill
//				print ("" + emptySlots [i].indexOfSlot + " " + gh.checkIfCanMill (emptySlots [i].indexOfSlot, Map.instance.tokenPrefab.tag));
				if (gh.checkIfCanMill (emptySlots [i].indexOfSlot, Map.instance.tokenPrefab.tag)) {
					return emptySlots [i].indexOfSlot;
				}
				//block opponent
				int hasOppNeighbours = 0;
				foreach (Slot neighbour in emptySlots[i].neibourSlots) {
					if (neighbour != null && !neighbour.isEmpty () && neighbour.transform.GetChild (0).gameObject.tag == Map.instance.tokenPrefab.tag)
						hasOppNeighbours++;
				}
				if (hasOppNeighbours > maxNeighbourOpponents) {
					maxOppList.Clear ();
					maxNeighbourOpponents = hasOppNeighbours;
					maxOppList.Add (emptySlots [i]);
				} else if (hasOppNeighbours == maxNeighbourOpponents) {
					maxOppList.Add (emptySlots [i]);
				}

			}
			Slot[] mos = maxOppList.ToArray ();
			index = mos [Random.Range (0, maxOppList.Count)].indexOfSlot;
//			print ("maxNeighbourOpponents " + maxNeighbourOpponents + " count " + maxOppList.Count + " " + index);
		} else {
			index = emptySlots[Random.Range (0, emptySlots.Length)].indexOfSlot;
//			print("Random "+index);
		}

		return index;
	}
	bool checkIfCanFly(){
		//check if fly mode
		if (GameHandler.currentGameType == GameType.Fly) {
			if (tokenPrefab.tag == GameConstants.BLACK_TOKEN_NAME && Map.instance.blackTokensDeleted >= 6)
				return true;
			else if (tokenPrefab.tag == GameConstants.WHITE_TOKEN_NAME && Map.instance.whiteTokensDeleted >= 6) {
				return true;
			}
		}
		return false;
	}

	void findBestSlotToMoveToken(){

		Slot moveToSlot = null;
		bool isMoved = false;

		//get all my tokens
		List<Slot> mySloList = new List<Slot>();
		for(int i=0;i<GameHandler.instance.allSlots.Length;i++){
			if(!GameHandler.instance.allSlots[i].isEmpty() && GameHandler.instance.allSlots[i].transform.GetChild(0).tag == tokenPrefab.tag){
				mySloList.Add(GameHandler.instance.allSlots[i]);
			}
		}
		Slot[] mySlotArray = mySloList.ToArray();

		if (checkIfCanFly ()) {
			print("Can fly");
			getAllEmptySlots ();

		}
		
//		print ("CheckPoint : 2");
		if (!isMoved){

			//check if my mill anywhere on board to reuse

			List<List<Slot>> lls = gh.checkIfHaveAnyMill(tokenPrefab.tag);
			if(lls.Count!=0){
//				print("Already Has Mills "+lls.Count);
				foreach(List<Slot> ls in lls){	
					foreach(Slot s in ls){
						Slot[] neighbors = s.neibourSlots;
						for(int i=0;i<neighbors.Length;i++)
						{
							if(neighbors[i].isEmpty()){
								bool move = true;
								for(int j=0;j<(neighbors.Length);j++){
									if(!neighbors[j].isEmpty() && neighbors[j].transform.GetChild(0).tag != tokenPrefab.tag)
										move=false;
								}
								if(move && !isMoved){
									moveToSlot = neighbors[i];
//									print("Moving "+s.indexOfSlot+" to "+moveToSlot.indexOfSlot);
									moveAiTokenonBoard (s, moveToSlot);
									isMoved = true;
								}
							}
						}
					}
				}
			}
			//find all free token to move which will not give any chance to make opp mill
			if(!isMoved){
						List<Slot> aiFreeTokens = new List<Slot>();
						for (int i=0; i<mySlotArray.Length ; i++) {
							Slot[] myTokenNeighbours = mySlotArray [i].neibourSlots;
							for (int j=0; j<myTokenNeighbours.Length  ; j++) {
								if (myTokenNeighbours [j].isEmpty ()) {
									bool move = false;
									if(myTokenNeighbours.Length==4){
										for(int k=0;k<myTokenNeighbours.Length;k++){
											if(myTokenNeighbours [k].isEmpty () || myTokenNeighbours [k].transform.GetChild(0).tag == tokenPrefab.tag){
												if(myTokenNeighbours [k].indexOfSlot != myTokenNeighbours [j].indexOfSlot)
													move = true;
											}
										}
										if(move){
											aiFreeTokens.Add(mySlotArray [i]);
//											print ("Added "+mySlotArray [i].indexOfSlot +" neigh"+myTokenNeighbours [j].indexOfSlot);
											break;
										}
									}
									else{
										aiFreeTokens.Add(mySlotArray [i]);
//										print ("Added "+mySlotArray [i].indexOfSlot +" neigh"+myTokenNeighbours [j].indexOfSlot);
										break;
									}
								}
							} 
						}
				Slot[] aiFreeTokensArray = aiFreeTokens.ToArray();
//				print("Free...."+aiFreeTokens.Count+" "+aiFreeTokensArray.Length);
				while(!isMoved){
					//find first empty and move
					for(int i=0;i<GameHandler.instance.allSlots.Length && !isMoved;i++){
						if(!GameHandler.instance.allSlots[i].isEmpty() && GameHandler.instance.allSlots[i].transform.GetChild(0).tag == tokenPrefab.tag){
							Slot[] neigh = GameHandler.instance.allSlots[i].neibourSlots;
							for (int k=0; k<neigh.Length && !isMoved; k++) {
								if (neigh [k].isEmpty ()) {
									moveToSlot = neigh [k];
									moveAiTokenonBoard (GameHandler.instance.allSlots[i], moveToSlot);
									isMoved = true;
								}
							}
						}
					}

				}
			}

		}

//		print ("CheckPoint : 3");

		if (moveToSlot != null && GameHandler.instance.checkForMills (moveToSlot.indexOfSlot)) {

			Map.instance.save.isMillCreated = true;
			//Remove opponent token
			deleteOpponentToken(moveToSlot.transform.GetChild(0).gameObject.GetComponent<Token>());
			
		} else if (moveToSlot != null) {
			Map.instance.save.isMillCreated = false;
			moveToSlot.transform.GetChild(0).gameObject.GetComponent<Token>().setMyTurn();
			return;
		} 

	}

	void deleteOpponentToken(Token token){

		GameHandler.instance.gteRemovableTokensAfterMill ();
		if (GameHandler.instance.nonMillElemntsList.Count != 0) {

			Slot[] nonmillElmentsArray = GameHandler.instance.nonMillElemntsList.ToArray ();
			int removeTokenOfIndex = 0;
			if(Random.Range(0,100)<=probValue)
				 removeTokenOfIndex = getBestIndexToDelete(nonmillElmentsArray);
			else
				removeTokenOfIndex = nonmillElmentsArray[Random.Range(0,nonmillElmentsArray.Length)].indexOfSlot;

			print ("NonMillDeleted opp token no " + removeTokenOfIndex);
			token.removeToken (removeTokenOfIndex);
			Map.instance.save.deleteSlot = removeTokenOfIndex;
			token.setMyTurn();
			return;
		}else if (GameHandler.instance.millElementsList.Count != 0) {
						
			Slot[] millElmentsArray = GameHandler.instance.millElementsList.ToArray ();
			int removeTokenOfIndex = 0;
			if(Random.Range(0,100)<=probValue)
				removeTokenOfIndex = getBestIndexToDelete(millElmentsArray);
			else
				removeTokenOfIndex = millElmentsArray[Random.Range(0,millElmentsArray.Length)].indexOfSlot;
			print ("MillDeleted opp token no " + removeTokenOfIndex);
			token.removeToken (removeTokenOfIndex);
			Map.instance.save.deleteSlot = removeTokenOfIndex;
			token.setMyTurn();
			return;
		} 
	}

	int getBestIndexToDelete(Slot[] slotsToDelete){
//		print ("GetBest to delete");
		//check if is there any place where opp will make mill
		getAllEmptySlots ();
//		print ("empty slots "+emptySlots.Length);
		for(int i=0;i<emptySlots.Length;i++){
			if(gh.checkIfCanMill(emptySlots[i].indexOfSlot,Map.instance.tokenPrefab.tag)){

				//getmill elemnts of this index and check if able to delete
				Slot[] slots = gh.getAllSlotOfMIll(emptySlots[i].indexOfSlot,Map.instance.tokenPrefab.tag);
				if( noOfTokens < GameConstants.MAX_NO_OF_TOKENS){
					for(int j=0;j<slots.Length;j++){
						for(int k=0;k<slotsToDelete.Length;k++){
							if(slots[j].indexOfSlot == slotsToDelete[k].indexOfSlot){
//								print("Trying to make mill i will remove you"+slots[j].indexOfSlot);
								return slots[j].indexOfSlot;
							}
						}
					}
				}else{
					//check if it has extra element to move and make mill
					Slot[] neigh = emptySlots[i].neibourSlots;
					bool checkToDel = false;

					for(int m=0;m<neigh.Length;m++){
						bool remove = true;
						for(int j=0;j<slots.Length;j++){
							if(neigh[m].isEmpty())
								remove = false;   
							else if(neigh[m].indexOfSlot == slots[j].indexOfSlot)
								remove = false;
						}
						if(remove){
							if(neigh[m].transform.GetChild(0).tag == Map.instance.tokenPrefab.tag)
									checkToDel = true; 
						}
					}

				}
			}
		}

		//try to find free slot
		List<Slot> freeOpp = new List<Slot> ();
		for (int i=0; i<slotsToDelete.Length; i++) {
			Slot[] neighbours = slotsToDelete [i].neibourSlots;
			for (int j=0; j<neighbours.Length; j++) {
				if (neighbours [j].isEmpty ()) {
//					print("Free token "+slotsToDelete[i].indexOfSlot);
					freeOpp.Add (slotsToDelete[i]);
					break;
				}
			}
		}
//		print ("Total no of free token "+freeOpp.Count);
		Slot[] freeOppArray = freeOpp.ToArray ();


//			print("random delete");
		return slotsToDelete[Random.Range (0,slotsToDelete.Length)].indexOfSlot;
	}
	
	int probValue ;
	public void easyAiTurn(){
		if (!GameHandler.isMyTurn) {
//			print("Ai turn called");
			//has to place tokens on board
			if (noOfTokens < GameConstants.MAX_NO_OF_TOKENS) {

				Map.instance.save.canMove = false;

				//search for a slot to place token
				getAllEmptySlots ();
				getProbabilityValue();
				int indexSlot = findBestSlotOnBoard();
				//place token on slot
				GameObject token;
				token = Instantiate (tokenPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				token.GetComponent<Token> ().setTokenParent(indexSlot);
				Map.instance.save.toSlot = indexSlot;

				noOfTokens++; 

				if (GameHandler.instance.checkForMills (indexSlot)) 
				{
					Map.instance.save.isMillCreated = true;
					deleteOpponentToken(token.GetComponent<Token> ());

				} 
				else
				{
					Map.instance.save.isMillCreated = false;
					token.GetComponent<Token> ().setMyTurn();
				}
			
			}else{
				Map.instance.save.canMove = true;
				if(gh.checkifHasFreeTokensToMove(tokenPrefab.tag))
					findBestSlotToMoveToken();
				else{//All token BLOCKED
//					print("Token blocked");
					NetworkManager.instance.photonView.RPC("timeOutResultDeclaration",PhotonTargets.All);
				}
			}
			CancelInvoke("easyAiTurn");
		}

	}

	void getProbabilityValue(){
		switch(NewgameManager.currentgameOption){
		case (int)GameModes.SinglePlayerEasy:
			probValue = 30;
			break;
		case (int)GameModes.SinglePlayerMedium:
			probValue = 65;
			break;
		case (int)GameModes.SinglePlayerHard:
			probValue = 100;
			break;

		}
	}

	void moveAiTokenonBoard(Slot fromSlot,Slot toSlot){
//		print ("moving "+fromSlot.indexOfSlot+" to "+toSlot.indexOfSlot);

		Transform itemTransform = fromSlot.transform.GetChild (0);
		float fract = 0;
		//move token slowly 
		while (fract < 1) {
			itemTransform.position = Vector3.Lerp (fromSlot.transform.position, toSlot.transform.position, fract);
			fract += Time.deltaTime;
		}
		fromSlot.transform.GetChild(0).transform.SetParent(toSlot.transform);

	}

}
