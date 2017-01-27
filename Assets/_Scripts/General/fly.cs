using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class fly : MonoBehaviour {

	Vector3 startPos,endPos;
	Animator animator;
	Image butterflyImg;


	float canvasWidth ,canvasHeight;
	float objectWidth ,objectHeight;
	float fract = 0f;
	int direction = -1;//-1 for left ,1 for right
	int dirVal = 0;

	
	public float bSpeed = 0.1f;
	float minWait,maxwait;
	// Use this for initialization
	void Start () {
		canvasWidth = GameObject.Find ("Canvas").GetComponent<RectTransform> ().rect.width;
		canvasHeight = GameObject.Find ("Canvas").GetComponent<RectTransform> ().rect.height;
		objectWidth = GetComponent<RectTransform> ().rect.width;
		objectHeight = GetComponent<RectTransform> ().rect.height;
		animator = GetComponent<Animator> ();
		butterflyImg = GetComponent<Image> ();
		StartCoroutine (setTarget());
		butterflyImg.enabled = false;
		minWait = 5f;
		maxwait = 15f;
	}
	IEnumerator setTarget(){
		yield return new WaitForSeconds (Random.Range(minWait,maxwait));
		dirVal = Random.Range (-1,1);
		if (dirVal < 0)
			direction = -1;
		else
			direction = 1;

		startPos 	= new Vector3 (canvasWidth/2f * direction, Random.Range(-(canvasHeight/2f-objectHeight/2f),(canvasHeight/2f-objectHeight/2f)),0f );
		endPos 		= new Vector3 (-canvasWidth/2f * direction ,Random.Range(-(canvasHeight/2f-objectHeight/2f),(canvasHeight/2f-objectHeight/2f)),0f );
		//set direction in animator
		animator.SetInteger ("Direction",direction);
		drawButterfly = true;
		butterflyImg.enabled = true;
	}


	bool drawButterfly = false;
	// Update is called once per frame
	void Update () {
		if (drawButterfly) {
			transform.localPosition = Vector3.Lerp (startPos, endPos, fract);
			if (fract < 1) 
				fract += bSpeed * Time.deltaTime;
			else {
				drawButterfly = false;
				butterflyImg.enabled = false;
				fract = 0.0f;
				StartCoroutine (setTarget ());
			}
		}
	}
}
