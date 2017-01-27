using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Facebook.MiniJSON;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour {

	public static MenuManager instance;
	public Text WinText,GscoreText;//ScoreText;
	public Text undoCountText;

	string gameLink;//com.ovilex.bussimulator2015&hl=en";//com.ch.dogergjiki";

	void Awake(){
		instance = this;
	}

	void Start(){
		gameLink = "https://play.google.com/store/apps/details?id="+Application.bundleIdentifier;
		setUndoCountofMenu ();
	}

	public void setUndoCountofMenu(){
		undoCountText.text = "Current Undo moves: "+PlayerPrefs.GetInt(GameConstants.KEY_NO_OF_UNDO_COUNTS);	
	}

	public void newgameSelected(){
		AllPopups.instance.menuPopUp.SetActive (false);
		AllPopups.instance.newGamePopUp.SetActive (true);
	}

	public void rateUsSelected(){
		if (InternetChecker.isInternetOn) {
			Application.OpenURL(gameLink);
		} else
			AllPopups.instance.internetCheckPopUp.SetActive (true);
	}

	public void storeSelected(){
//		AllPopups.instance.menuPopUp.SetActive (false);
		AllPopups.instance.StorePopUp.SetActive (true);
	}

	public void acheivementsSelected(){
		//		AllPopups.instance.menuPopUp.SetActive (false);
		AllPopups.instance.acheivementsPopUp.SetActive (true);
	}

	public void leaderBoardSelected(){

		if (InternetChecker.isInternetOn) {
			DataManager.instance.getHighScoreClicked ();
			DataManager.instance.getUserDetailsClicked ();
			AllPopups.instance.leaderBoardPopUp.SetActive (true);
			WinText.text = "" + GameHandler.myTotalScore;
			if (GameHandler.globalHighScore < GameHandler.myTotalScore)
				GscoreText.text = "" + GameHandler.myTotalScore;
			else
				GscoreText.text = GameHandler.globalHighScore.ToString ();
		}else
			AllPopups.instance.internetCheckPopUp.SetActive (true);
	}

	public void inviteFriendsSelected(){
		if (InternetChecker.isInternetOn) {
			FacebookManager.invitingFriends = true;
			FacebookManager.instance.loginFbClicked ();
		} else {
			AllPopups.instance.internetCheckPopUp.SetActive (true);
		}
	}

	public void showMessagesClick(){
		if (InternetChecker.isInternetOn) {
			if (FB.IsLoggedIn) {
				print("Apprequests called");
				//get All request
				FB.API ("/v2.3/me/apprequests", Facebook.HttpMethod.GET, requestsCallback);
				AllPopups.instance.fbMessagesPopUp.SetActive (true);
				AllPopups.instance.fbMessagesPopUp.transform.FindChild("Bg").FindChild("selectAllcheckbox").GetComponent<Toggle>().isOn = false;
			} 
			else
				FacebookManager.instance.loginFbClicked ();
		}
		else {
			AllPopups.instance.internetCheckPopUp.SetActive (true);
		}
	}

	//undo move object id : 924010304339123
	public void askForMovesSelected(){
		if (InternetChecker.isInternetOn) {
			if(FB.IsLoggedIn)
			{

				//to delete the request by request id
//				FB.API("/883472665080837", Facebook.HttpMethod.DELETE, requestsCallback);

				//Asking for undo moves
				FB.AppRequest(
					 "asked you for undo move",
					 Facebook.OGActionType.AskFor,
					 GameConstants.FACEBOOK_UNDO_OBJECT_ID,
					 null,
					 null,
					 null,
					 "ask",
					 "Ask for undo move",
					 undoRequestCallback
					);
			}else
				FacebookManager.instance.loginFbClicked ();
		} else {
			AllPopups.instance.internetCheckPopUp.SetActive (true);
		}
	}

	//to load a user image into texture but not used in this game
	IEnumerator UserImage()
	{
		WWW url = new WWW("https://graph.facebook.com/772669122786075/picture?type=large"); 
		Texture2D textFb2 = new Texture2D(128, 128, TextureFormat.DXT1, false); //TextureFormat must be DXT5
		yield return url;
		url.LoadImageIntoTexture(textFb2);
		print ("frind img loaded");
	}

	//ask for : Request sent {"request":"739276736201628","to":["883472665080837"]}
	//send    : Request sent {"request":"126202544398807","to":["883472665080837"]}
	//APICallback{"id":"1469439223361259","name":"Cubical Hub","email":"cubicalhub\u0040gmail.com","friends":{"data":[{"name":"Ajeet Gautam","id":"10205302721713599","picture":{"data":{"height":200,"is_silhouette":false,"url":"https:\/\/fbcdn-profile-a.akamaihd.net\/hprofile-ak-ash2\/v\/t1.0-1\/c0.0.200.200\/p200x200\/10703933_10203081938715412_8439573310382990292_n.jpg?oh=d4834b81ccd692c9296bffdf0992b747&oe=565DF82A&__gda__=1452785487_5d27317e2d532f4cec0240d8104371a9","width":200}}},{"name":"Rakhi Chavhan","id":"883472665080837","picture":{"data":{"height":200,"is_silhouette":false,"url":"https:\/\/fbcdn-profile-a.akamaihd.net\/hprofile-ak-xap1\/v\/t1.0-1\/p200x200\/10676274_740529476041824_8993626882881006383_n.jpg?oh=e3d949e1bacc522d09d71d8f9355b5f7&oe=566425EF&__gda__=1453176298_c5c6d5f70a2cbd870ac7dbb0a985aaf3","width":200}}}],"paging":{"next":"https:\/\/graph.facebook.com\/v2.4\/1469439223361259\/friends?limit=10&fields=name,id,picture.width\u002528182\u002529.height\u002528182\u002529&access_token=CAAU8QZBkwehwBAGzUk7ozFfZA7pd3kN2K2QjPEGDOkMxCinohTbOGW14QACb0awVT12iViIwZCH5RE6Ku6k9YW1g2VcaC6WntyZCGxcyxxRJ8ao75ieTrAL8L3ZCMBZBhsnH52MKVotcebSRtElBhCZCCMknVH3SZCE6ZCg3drMCMX1yimlDZCPcMZCrjZBGVf3lz6zdqxPoiSpVAkQEiNvEJVqr&offset=10&__after_id=enc_AdA5BPbRzAJ3DO6hrUw5PWWyGE1OMTGmePFWdslZAnzevOu39SmhdZBFwe9qvwtIjwZBfZC8cNtBQL4HQxBePiKF9kOg"},"summary":{"total_count":14}}}

	void undoRequestCallback(FBResult result){
		if (result != null)                                                                                                        
		{                                                                                                                          
			var responseObject = Json.Deserialize(result.Text) as Dictionary<string, object>;                                      
			object obj = 0;                                                                                                        
			if (responseObject.TryGetValue ("cancelled", out obj))                                                                 
			{                                                                                                                      
				Util.Log("undo Request cancelled");                                                                                  
			}                                                                                                                      
			else if (responseObject.TryGetValue ("request", out obj))                                                              
			{                
				Util.Log("undo Request sent "+result.Text);   

			}                                                                                                                      
		} 
	}

	object nameH,nameH1;
	string childMsgString = "";
	void requestsCallback(FBResult result){

		Dictionary<string,object> response = (Dictionary<string,object>)Json.Deserialize (result.Text);
		var data = new List<object>();
		data.AddRange((List<object>)(response["data"]));
		print ("data length "+data.Count);

		if (data.Count != 0) {
			//delete old requests
			for(int i=0;i<FacebookManager.instance.fbMessagesParent.transform.childCount;i++){
				Destroy(FacebookManager.instance.fbMessagesParent.transform.GetChild(i).gameObject);
			}
			foreach (object obj in data) {

				Dictionary<string, object> app = (Dictionary<string, object>)obj;
				//do not show invites 
				if (app.TryGetValue ("data", out nameH)) {
					if((string)nameH == "invite"){
						//delete invite request
						if (app.TryGetValue ("id", out nameH)) {
							print ("invite req id " + nameH);
							FB.API("/"+(string)nameH,Facebook.HttpMethod.DELETE,null);
						}
						continue;
					}
				}

				childMsgString = "";
				GameObject childMsg = Instantiate (FacebookManager.instance.fbMsgPrefab) as GameObject;
				childMsg.transform.SetParent (FacebookManager.instance.fbMessagesParent.transform);
				childMsg.transform.localScale = Vector3.one;

				if (app.TryGetValue ("from", out nameH)) {
					Dictionary<string, object> from = (Dictionary<string, object>)nameH;
					if (from.TryGetValue ("name", out nameH1)) {
						print ("name " + nameH1);
						childMsgString += (string)nameH1;
					}
					if (from.TryGetValue ("id", out nameH1)) {
						print ("id " + nameH1);
						childMsg.transform.FindChild ("FromFbId").GetComponent<Text> ().text = (string)nameH1;
						LoadPictureURL("https://graph.facebook.com/"+(string)nameH1+"/picture?type=large",childMsg,FriendPictureCallback);
					}
				}

				if (app.TryGetValue ("id", out nameH)) {
					print ("req id " + nameH);
					childMsg.transform.FindChild ("reqId").GetComponent<Text> ().text = (string)nameH;
				}
				if (app.TryGetValue ("message", out nameH)) {
					print ("msg " + nameH);
					childMsgString += " "+(string)nameH;
				}
				if (app.TryGetValue ("data", out nameH)) {
					print ("data " + nameH);
					childMsg.transform.FindChild ("data").GetComponent<Text> ().text = (string)nameH;
				}
				childMsg.transform.FindChild ("Message").GetComponent<Text> ().text = childMsgString;
			}
			//set values
			AllPopups.instance.fbMessagesPopUp.transform.FindChild("LoadingText").gameObject.SetActive(false);
			AllPopups.instance.fbMessagesPopUp.transform.FindChild("NoMessagesText").GetComponent<Text>().text = "";
			AllPopups.instance.fbMessagesPopUp.transform.FindChild("Bg").FindChild("deactiveButtons").gameObject.SetActive(false);
		} else {
			AllPopups.instance.fbMessagesPopUp.transform.FindChild("LoadingText").gameObject.SetActive(false);
			AllPopups.instance.fbMessagesPopUp.transform.FindChild("NoMessagesText").GetComponent<Text>().text = "You don't have any message";
			AllPopups.instance.fbMessagesPopUp.transform.FindChild("Bg").FindChild("deactiveButtons").gameObject.SetActive(true);

		}
		//if has some invites but no ask/sent undo requests 
		if (FacebookManager.instance.fbMessagesParent.transform.childCount == 0) {
			AllPopups.instance.fbMessagesPopUp.transform.FindChild("LoadingText").gameObject.SetActive(false);
			AllPopups.instance.fbMessagesPopUp.transform.FindChild("NoMessagesText").GetComponent<Text>().text = "You don't have any message";
			AllPopups.instance.fbMessagesPopUp.transform.FindChild("Bg").FindChild("deactiveButtons").gameObject.SetActive(true);

		}
	}
	public void FriendPictureCallback(Texture texture,GameObject frame)
	{
		print ("FriendPictureCallback"+frame.name);
		frame.transform.FindChild ("ProfilePic").GetComponent<RawImage>().texture = texture;
	}

	void LoadPictureURL (string url,GameObject frame, LoadPictureCallback callback)
	{
		StartCoroutine(LoadPictureEnumerator(url,frame,callback));		
	}
	IEnumerator LoadPictureEnumerator(string url,GameObject frame, LoadPictureCallback callback)    
	{
		WWW www = new WWW(url);
		yield return www;
		callback(www.texture,frame);
	}
	delegate void LoadPictureCallback (Texture texture,GameObject frame);

}
