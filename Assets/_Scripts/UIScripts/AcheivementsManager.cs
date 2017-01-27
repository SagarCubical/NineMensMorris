using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AcheivementsManager : MonoBehaviour {
	public static AcheivementsManager instance;

//	public GameObject acheivementPrefab, noAchMsg;
//	public Transform acheivementsParent;
	public Image[] undoImages;

	void Awake(){
		instance = this;
	}

	// Use this for initialization
	void OnEnable () {
	
		checkCurrentAcheivements ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator checkCurrentAcheivements(){
//		int currentwins = PlayerPrefs.GetInt(GameConstants.KEY_NO_OF_GAME_WINS);
		int currentwins = 0;
		if (InternetChecker.isInternetOn)
			DataManager.instance.getUserDetailsClicked ();
		yield return new WaitForSeconds (10.0f);
		currentwins = GameHandler.myTotalScore;
		for (int i=25; i<=100; i+=25) {
			if(i<=currentwins)
				undoImages[(i/25)-1].color = Color.white;
			else
				undoImages[(i/25)-1].color = Color.black;
		}
		if(currentwins>=150)
			undoImages[undoImages.Length-1].color = Color.white;
		else
			undoImages[undoImages.Length-1].color = Color.black;
	}

	public void giveRewardForAcheivement(int wins){
		int rewarValue = 0;
		if (wins == 25)
			rewarValue = 20;
		else if (wins == 50)
			rewarValue = 40;
		else if (wins == 75)
			rewarValue = 80;
		else if (wins == 100)
			rewarValue = 100;
		else if (wins == 150)
			rewarValue = 125;
		else
			rewarValue = 0;
		int undo = PlayerPrefs.GetInt (GameConstants.KEY_NO_OF_UNDO_COUNTS);
		undo += rewarValue;
		PlayerPrefs.SetInt (GameConstants.KEY_NO_OF_UNDO_COUNTS , undo);
		PlayerPrefs.Save ();
	}
}
