using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {

	public Image soundImg;
	public Sprite soundOn, soundOff;
	static bool isSoundOn = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void handlesoundOnOff(){

		if(isSoundOn){
			isSoundOn = !isSoundOn;
			soundImg.sprite = soundOff;
		}else
		{
			isSoundOn = !isSoundOn;
			soundImg.sprite = soundOn;
		}

	}
}
