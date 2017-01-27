using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Facebook.MiniJSON;

public class facebookRequestsManager : MonoBehaviour {

	public Transform parentHolder;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	List<GameObject> selectedChilds = new List<GameObject>();
	List<string> sendMovesTo = new List<string>();
	public void acceptMessagesClicked(){
		//get all selected messages using fbMsgCheckBox
		selectedChilds.Clear ();
		sendMovesTo.Clear ();
		for(int i=0;i<FacebookManager.instance.fbMessagesParent.transform.childCount;i++){
			GameObject child = FacebookManager.instance.fbMessagesParent.transform.GetChild(i).gameObject;
			bool isSelected = child.transform.FindChild("fbMsgCheckBox").GetComponent<Toggle>().isOn;
			if(isSelected){
				selectedChilds.Add(child);
				//delete entry from scrollview
				Destroy(child);
			}
		}
		print ("Selected childs count "+selectedChilds.Count);
		//if data is send - add undo moves to player , if data is ask then send undomove to that friend and delete the entry
		foreach (GameObject g in selectedChilds) 
		{
			string dataVal = g.transform.FindChild("data").GetComponent<Text>().text; 
			string reqVal = g.transform.FindChild("reqId").GetComponent<Text>().text; 
			if(dataVal == "send"){
				//give undomove to player
				print("Got 1 undo move");

				//give 1 undo on each invite
				int uCount = PlayerPrefs.GetInt(GameConstants.KEY_NO_OF_UNDO_COUNTS);
				uCount += 1;
				PlayerPrefs.SetInt(GameConstants.KEY_NO_OF_UNDO_COUNTS , uCount);
				PlayerPrefs.Save();
				MenuManager.instance.setUndoCountofMenu();
				//delete request
				FB.API("/"+reqVal,Facebook.HttpMethod.DELETE,reqDeleteCallback);
			}else// if(dataVal == "ask")
			{
				//add fbid to to list to send undo moves
				sendMovesTo.Add(g.transform.FindChild("FromFbId").GetComponent<Text>().text);
				print("delete req id "+reqVal);
				//delete request
				FB.API("/"+reqVal,Facebook.HttpMethod.DELETE,reqDeleteCallback);
			}
		}
		print ("send count "+sendMovesTo.Count);
		if (sendMovesTo.Count != 0) {
		//send undo moves to to-ids
			FB.AppRequest(
				"has Sent you undo move",
				Facebook.OGActionType.Send,
				GameConstants.FACEBOOK_UNDO_OBJECT_ID,
				sendMovesTo.ToArray(),
				"send",
				"Send Undo Moves",
				undoSendCallback);
		}
	}

	void undoSendCallback(FBResult result){
		if (result != null)                                                                                                        
		{                                                                                                                          
			var responseObject = Json.Deserialize(result.Text) as Dictionary<string, object>;                                      
			object obj = 0;                                                                                                        
			if (responseObject.TryGetValue ("cancelled", out obj))                                                                 
			{                                                                                                                      
				Util.Log("undo sending Request cancelled");                                                                                  
			}                                                                                                                      
			else if (responseObject.TryGetValue ("request", out obj))                                                              
			{                
				Util.Log("undo sending Request sent "+result.Text);   
				
			}                                                                                                                      
		} 
	}
	void reqDeleteCallback(FBResult result){
		if (result != null)                                                                                                        
		{                                                                                                                          
			var responseObject = Json.Deserialize(result.Text) as Dictionary<string, object>;                                      
			object obj = 0;                                                                                                        
			if (responseObject.TryGetValue ("cancelled", out obj))                                                                 
			{                                                                                                                      
				Util.Log("delete Request cancelled");                                                                                  
			}                                                                                                                      
			else if (responseObject.TryGetValue ("request", out obj))                                                              
			{                
				Util.Log("delete Request sent "+result.Text);   
				
			}                                                                                                                      
		} 
	}

	public void closefbMsgsPopUp(){
		//delete old requests
		for(int i=0;i<FacebookManager.instance.fbMessagesParent.transform.childCount;i++){
			Destroy(FacebookManager.instance.fbMessagesParent.transform.GetChild(i).gameObject);
		}
		//NoMessagesText,LoadingText
		AllPopups.instance.fbMessagesPopUp.transform.FindChild("LoadingText").gameObject.SetActive(true);
		AllPopups.instance.fbMessagesPopUp.transform.FindChild("NoMessagesText").GetComponent<Text>().text = "";
		AllPopups.instance.fbMessagesPopUp.transform.FindChild("Bg").FindChild("deactiveButtons").gameObject.SetActive(true);
		AllPopups.instance.fbMessagesPopUp.SetActive (false);
	}

	public void selectAllClicked(Toggle toggle){
		if (parentHolder.childCount != 0) {

			for(int i=0;i<parentHolder.childCount;i++)
			{
				parentHolder.GetChild(i).GetComponentInChildren<Toggle>().isOn = toggle.isOn;
			}

		}
	}

}
