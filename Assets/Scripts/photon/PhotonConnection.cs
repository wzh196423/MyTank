using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PhotonConnection : PunBehaviour {
	string temp;
	// Use this for initialization
	void Start () {
		temp = PhotonNetwork.connectionStateDetailed.ToString ();
		Debug.Log (temp);
		PhotonNetwork.ConnectUsingSettings ("1.0");
	}
	
	// Update is called once per frame
	void Update () {
		if (temp != PhotonNetwork.connectionStateDetailed.ToString ()) {
			temp = PhotonNetwork.connectionStateDetailed.ToString ();
			Debug.Log (temp);
		}
	}
	public override void OnConnectedToMaster(){
		PhotonNetwork.JoinLobby ();
	}
	public override void OnJoinedLobby(){
		PhotonNetwork.CreateRoom ("");
	}
}
