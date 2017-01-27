using UnityEngine;
using System.Collections;
using System.IO;

public class leaderBoardShare : MonoBehaviour {

	public void shareScoreBoard(){
		if (InternetChecker.isInternetOn)
			StartCoroutine (shareImage ());
		else
			AllPopups.instance.internetCheckPopUp.SetActive (true);
	}

	//for ios
	#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void sampleMethod (string iosPath, string message);
	#endif
	
	string subject = "Nine Men's morris Fantasy";
	string linkName = "https://play.google.com/store/apps/details?id=com.demetergames.ninemenmorrisfantasy";

	//Shares a image with text
	IEnumerator shareImage(){
		
		yield return new WaitForEndOfFrame ();
		
		//To take screenshot
		Texture2D MyImage = new Texture2D(Screen.width, Screen.height,TextureFormat.RGB24,true);
		// put buffer into texture
		MyImage.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height),0,0);
		// apply
		MyImage.Apply();
		
		byte[] bytes = MyImage.EncodeToPNG();
		string path = Application.persistentDataPath + "/MyImage.png";
#if !UNITY_WEBPLAYER
		File.WriteAllBytes(path, bytes);
#endif
		
		#if UNITY_ANDROID
		
		AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
		AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
		intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
		AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
		
		intentObject.Call<AndroidJavaObject>("setType", "image/*");
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), linkName);
		
		AndroidJavaClass fileClass = new AndroidJavaClass("java.io.File");		
		AndroidJavaObject fileObject = new AndroidJavaObject("java.io.File", path);// Set Image Path Here
		AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromFile", fileObject);
		
		bool fileExist = fileObject.Call<bool>("exists");
		Debug.Log("File exist : " + fileExist);
		if (fileExist)
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
		
		AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
		currentActivity.Call("startActivity", intentObject);
		
		#endif
		
		#if UNITY_IPHONE || UNITY_IPAD
			sampleMethod(path ,linkName);
		#endif
		
	}
}
