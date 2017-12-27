using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class ScreenShot : MonoBehaviour {
	//public GameObject MessageBoxPrefab;
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
		string filename = string.Format ("Record_{0}_{1}", PlayFabUserData.username, timestamp);
		//File.WriteAllBytes(Application.streamingAssetsPath + "/" + filename+".png", bytes);
		Debug.Log("send a pic "+filename);

		WWWForm form = new WWWForm();
		form.AddBinaryData(filename, bytes);
		WWW www = new WWW ("http://120.25.238.161:3000/upload.json", form);

		yield return www;
		// do something after the upload is done or failed
		Debug.Log ("upload file response: "+www.text);

		//GameObject messageBox = Instantiate(MessageBoxPrefab);
		if (www.text.Contains("success")) {
			//messageBox.GetComponent<MessageBox> ().Show ("战绩截图分享成功");
			// 告诉服务器有人分享了战绩截图
			string dest = string.Format("http://localhost:1337/userdata/updateRecord?username={0}&url={1}.png&time={2}"
				,PlayFabUserData.username,"http://120.25.238.161/PM/tank/record-images/"+filename,timestamp);
			WWW newWWW = new WWW (dest);
			yield return newWWW;
			if (newWWW.text.Contains ("success")) {
				shareButton.GetComponentInChildren<Text> ().text = "已分享";
			} else {
				shareButton.interactable = true;
			}
			Debug.Log ("请求返回:" + newWWW.text);
		} else {
			Debug.Log (www.error);
			//messageBox.GetComponent<MessageBox> ().Show ("分享失败，请重试!");
			shareButton.interactable = true;
		}       
	}

	public void ClickShareButton(){
		shareButton.interactable = false;
		StartCoroutine(SaveImage());
	}
}
