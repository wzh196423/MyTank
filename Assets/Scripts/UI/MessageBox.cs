using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour {
	
	//public GameObject Box;
	//public RectTransform RectTran;

	void Start() {
		//RectTran = Box.GetComponent<RectTransform> ();
		//print (RectTran);
	}

	public void click() {
		Destroy (gameObject);
	}


	public void Show(string message) {

		RectTransform rt = GetComponent<RectTransform> ();
		GetComponentInChildren<Text> ().text = message;
		rt.SetParent (GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>());
		//rt.anchoredPosition3D = new Vector3(0, 0, 0);
		rt.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		rt.sizeDelta = new Vector2(1.0f, 1.0f);

	}

}
