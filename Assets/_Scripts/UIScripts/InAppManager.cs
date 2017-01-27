using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InAppManager : MonoBehaviour {
	public static InAppManager instance;
	public GameObject removebutton;

	void Awake(){
		instance = this;
	}

	// Use this for initialization
	void Start () {

		if (PlayerPrefs.GetInt (GameConstants.key_playerPrefs_Remove_Ads) == 1)
			removebutton.SetActive (false);
		else
			removebutton.SetActive (true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void buyButtonClicked(int buttonNo){
		print ("Buy "+buttonNo);
		OpenIABTest.instance.purchaseItem (buttonNo);
	}


	public static void giveUndoForInAppPurchase(string id){
		int currentUndo = PlayerPrefs.GetInt (GameConstants.KEY_NO_OF_UNDO_COUNTS);
		switch(id){
		case "undo_10":
			currentUndo += 10;
			PlayerPrefs.SetInt (GameConstants.KEY_NO_OF_UNDO_COUNTS,currentUndo);
			break;
		case "undo_20":
			currentUndo += 20;
			PlayerPrefs.SetInt (GameConstants.KEY_NO_OF_UNDO_COUNTS,currentUndo);
			break;
		case "undo_50":
			currentUndo += 50;
			PlayerPrefs.SetInt (GameConstants.KEY_NO_OF_UNDO_COUNTS,currentUndo);
			break;
		case "undo_100":
			currentUndo += 100;
			PlayerPrefs.SetInt (GameConstants.KEY_NO_OF_UNDO_COUNTS,currentUndo);
			break;
		}
		PlayerPrefs.Save ();
		MenuManager.instance.setUndoCountofMenu();
	}

}
