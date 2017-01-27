using UnityEngine;
using System.Collections;

public class AutoScalewithCanvas : MonoBehaviour {

	public GameObject mainCanvas;

	float canvasWidth,canvasHeight;
	float standardWidth = 332f ,standardHeight = 598f;
	float scaleValue;

	// Use this for initialization
	void Start () {

		canvasWidth 	= 	mainCanvas.GetComponent<RectTransform> ().rect.width;
		canvasHeight 	= 	mainCanvas.GetComponent<RectTransform> ().rect.height;
		if (canvasWidth > canvasHeight) //if ladscape then scale acc to height
		{
			scaleValue		=	(canvasHeight/standardWidth) * 0.8f;
		} 
		else  //if portrait then scale acc to width
		{
			scaleValue		=	(canvasWidth/standardWidth) * 0.8f;
		}
		print ("scale "+canvasWidth);
		GetComponent<RectTransform> ().localScale = new Vector3(scaleValue, scaleValue, scaleValue);
	
	}
	
}
