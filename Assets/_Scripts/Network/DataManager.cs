using UnityEngine;
using System.Collections;

public class DataManager : MonoBehaviour {

	public static DataManager instance;

	void Awake(){
		instance = this;
	}

	public enum RequestCode{
		REGISTRATION 			=	1,
		GET_USER_DETAILS 	    =	2,
		UPDATE_USER_DETAILS 	=	3,
		GLOBAL_HIGH_SCORE	 	=	4,
	}
	
	string url_registration = "http://www.dogergjiki.com";
	string url_update_user_details = "http://www.dogergjiki.com";
	string url_global_high_score = "http://www.dogergjiki.com";
	string url_get_user_details = "http://www.dogergjiki.com";


	//?fb_id,name,email,win,loss
	void registerUser(string fb_id,string name,string email,string score , RequestCode req){
//		print ("Register user called");
		WWWForm form = new WWWForm ();
		form.AddField ("fb_id",fb_id);
		form.AddField ("name",name);
		form.AddField ("email",email);
		form.AddField ("win",score);
		form.AddField ("loss","0");
//		print ("added fields");
		WWW download = new WWW (url_registration,form);
		StartCoroutine (onRegistration(download,req));
	}

	
	//user_id=735";
	void getUserDetails(string user_id,RequestCode req){
		WWWForm form = new WWWForm ();
		form.AddField ("user_id",user_id);
		WWW download = new WWW (url_get_user_details,form);
		StartCoroutine (onRegistration(download,req));
	}

	void getGlobalHighScore(RequestCode req){
		WWW download = new WWW (url_global_high_score);
		StartCoroutine (onRegistration(download,req));
	}

	//user_id=735";
	void setUserScore(string user_id,string score ,RequestCode req){
		WWWForm form = new WWWForm ();
		form.AddField ("user_id",user_id);
		form.AddField ("win",score);
		WWW download = new WWW (url_update_user_details,form);
		StartCoroutine (onRegistration(download,req));
	}


	IEnumerator onRegistration(WWW download , RequestCode req){
		yield return download;	//wait untill download gtas the data
		if (!string.IsNullOrEmpty (download.error)) {
//			print (req + " cannot download data " + download.error);
		} 
		else 
		{
			if (download.isDone)
//				print (req + "data download succesfull" + download.text);
			if (req == RequestCode.REGISTRATION) {
				//{"result":"Successfully Inserted","user_id":"2"} or {"result":"user already exists","user_id":"2"}
				int uid;
				JSONObject jobj = new JSONObject (download.text);
				string status, userid;
				status = jobj.GetField ("result").ToString ().Replace ("\"", "");
				userid = jobj.GetField ("user_id").ToString ().Replace ("\"", "");
				PlayerPrefs.SetString (GameConstants.KEY_UNIQUEID_OF_USER, userid);
				PlayerPrefs.Save();
				//get Score of user from db 
				getUserDetailsClicked();
				getHighScoreClicked();
			}
			if(req == RequestCode.GLOBAL_HIGH_SCORE){
				JSONObject jobj = new JSONObject (download.text);
				string hscore;
				hscore = jobj.GetField ("higher_win").ToString ().Replace ("\"", "");
				GameHandler.globalHighScore = int.Parse(hscore);
			}
		
			if(req==RequestCode.GET_USER_DETAILS)
			{
				//[{"user_id":"2","fb_id":"833399340088170","name":"Rakhi","email":"rakhichavhan11@gmail.com","win":"0","loss":"0"}]
				int uid ;
				JSONObject jobj = new JSONObject(download.text)[0];
//				string fbid,userid
					string score;
				//	fbid = jobj.GetField("fb_id").ToString().Replace ("\"", "");
				//	userid = jobj.GetField("user_id").ToString().Replace ("\"", "");
				score = jobj.GetField("win").ToString().Replace ("\"", "");
//				print("details : Score is: "+score+" ");
				//set score of game
				GameHandler.myTotalScore = int.Parse(score);
			}

		}
	}

	public void registerUserClicked(){
		string name = PlayerPrefs.GetString (GameConstants.KEY_NAME_OF_USER);
		string email = PlayerPrefs.GetString (GameConstants.KEY_EMAIL_OF_USER);
		string id = PlayerPrefs.GetString (GameConstants.KEY_FBID_OF_USER);
		string score = "0";

		registerUser (id,name,email,score,RequestCode.REGISTRATION);
	}

	public void getUserDetailsClicked(){
		string userid = PlayerPrefs.GetString (GameConstants.KEY_UNIQUEID_OF_USER);
		getUserDetails (userid.ToString(),RequestCode.GET_USER_DETAILS);
	}
	public void getHighScoreClicked(){
		getGlobalHighScore (RequestCode.GLOBAL_HIGH_SCORE);
	}

	public void setUserScoreClicked(int score){
		string userid = PlayerPrefs.GetString (GameConstants.KEY_UNIQUEID_OF_USER);
		setUserScore (userid.ToString(), score.ToString() ,RequestCode.UPDATE_USER_DETAILS);
	}


}
