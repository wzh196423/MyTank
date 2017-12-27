using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class ScreenShot : MonoBehaviour {
	public GameObject MessageBoxPrefab;
	public Button shareButton;
	public Text uploadingInfo;

	private Texture2D image;
	private int w;
	private int h;
	// Use this for initialization

	void OnEnable(){
		if (uploadingInfo != null)
			uploadingInfo.gameObject.SetActive (false);
	}

	void Start () {
		w = Screen.width;
		h = Screen.height;
		image = new Texture2D(w, h,TextureFormat.RGB24,false);
	}

	IEnumerator SaveImage()
	{
		yield return new WaitForEndOfFrame();
		image.ReadPixels(new Rect(0, 0, w, h), 0, 0, true);//read pixels from screen to texture
		image.Apply();
		byte[] bytes = image.EncodeToPNG();
		DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
		string timestamp = ""+(long)(DateTime.Now - startTime).TotalMilliseconds;
		string filename = "Record_"+PlayFabUserData.username +"_" + timestamp;
		//File.WriteAllBytes(Application.streamingAssetsPath + "/" + filename+".png", bytes);
		Debug.Log("write a pic "+filename);

		WWWForm form = new WWWForm();
		form.AddBinaryData(filename, bytes);
		WWW www = new WWW ("http://120.25.238.161:3000/upload.json", form);

		yield return www;
		// do something after the upload is done or failed
		//uploadingInfo.gameObject.SetActive (false);
		Debug.Log (www.text);

		GameObject messageBox = Instantiate(MessageBoxPrefab);
		if (www.text.Contains("success")) {
			messageBox.GetComponent<MessageBox> ().Show ("战绩截图分享成功");
			shareButton.GetComponentInChildren<Text>().text = "已分享";
		} else {
			Debug.Log (www.error);
			messageBox.GetComponent<MessageBox> ().Show ("分享失败，请重试!");
			shareButton.interactable = true;
		}       
	}

	public void ClickShareButton(){
		shareButton.interactable = false;
		//uploadingInfo.gameObject.SetActive (true);
		Debug.Log ("appear");
		StartCoroutine(SaveImage());
	}
}
