using UnityEngine;
using System.Collections;

public class Leaf : MonoBehaviour {

	float canvasWidth ,canvasHeight;
	float objectWidth ,objectHeight;
	float startDelay;

	// Use this for initialization
	void Start () {
		canvasWidth = GameObject.Find ("Canvas").GetComponent<RectTransform> ().rect.width;
		canvasHeight = GameObject.Find ("Canvas").GetComponent<RectTransform> ().rect.height;
		objectWidth = GetComponent<RectTransform> ().rect.width;
		objectHeight = GetComponent<RectTransform> ().rect.height;
		setPosition ();
	}
	float xPOs,fract;
	public float leafSpeed;
	void setPosition(){
		xPOs = Random.Range (-canvasWidth/2f+objectWidth/2f,canvasWidth/2f-objectWidth/2f);
		transform.localRotation = Quaternion.Euler (0f,0f,Random.Range(-90f,90f));
		fract = 0;
		startDelay = Random.Range (1f, 5f);
		InvokeRepeating ("MoveLeaf",startDelay,0.01f);
	}

	void MoveLeaf(){
		fract += 0.01f * leafSpeed;
		transform.localPosition = new Vector3 (xPOs , Mathf.Lerp(750f,-750f,fract) ,transform.position.z);
		if (fract >= 1f) {
			CancelInvoke ("MoveLeaf");
			setPosition();
		}
	}

}
