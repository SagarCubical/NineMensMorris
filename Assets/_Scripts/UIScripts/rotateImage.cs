using UnityEngine;
using System.Collections;

public class rotateImage : MonoBehaviour {



	// Use this for initialization
	void Start () {
	
		InvokeRepeating ("rotate",0.0f,0.15f);
	}

	void rotate(){
		transform.Rotate (new Vector3(0f,0f,-360f/8f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
