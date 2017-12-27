using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomPanelController : MonoBehaviour {
	//public GameObject createRoomPanel;		//创建房间面板
	//public GameObject roomLoadingPanel;		//禁用游戏房间加载提示信息
	public Text roomName;					//房间名称文本
	public Text roomNameHint;				//房间名称提示文本
	public GameObject maxPlayerToggle;		//最大玩家个数开关组

	private byte[] maxPlayerNum = { 4, 6, 8 };	//最大玩家个数

	//创建房间面板激活时调用
	void OnEnable(){
//		roomNameHint.text = "";	//清空房间名称提示文本
	}

	//"确认创建"按钮事件处理函数
	public void ClickConfirmCreateRoomButton(){
		RoomOptions roomOptions=new RoomOptions();
		RectTransform toggleRectTransform = maxPlayerToggle.GetComponent<RectTransform> ();
		int childCount = toggleRectTransform.childCount;
		//根据最大玩家个数开关组的打开情况，确认房间最大玩家个数
		for (int i = 0; i < childCount; i++) {
			if (toggleRectTransform.GetChild (i).GetComponent<Toggle> ().isOn == true) {
				roomOptions.maxPlayers = maxPlayerNum [i];
				break;
			}
		}

		RoomInfo[] roomInfos = PhotonNetwork.GetRoomList();	//获取游戏大厅内所有游戏房间
		bool isRoomNameRepeat = false;
		//遍历游戏房间，检查新创建的房间名是否与已有房间重复
		foreach (RoomInfo info in roomInfos) {
			if (roomName.text == info.name) {
				isRoomNameRepeat = true;
				break;
			}
		}
		//如果房间名称重复，房间名称提示文本显示"房间名称重复！"
		if (isRoomNameRepeat) {
			roomNameHint.text = "房间名称重复!";
		}
		//否则，根据玩家设置的房间名、房间玩家人数创建房间
		else {
			PhotonNetwork.CreateRoom (roomName.text, roomOptions, TypedLobby.Default);	//在默认游戏大厅中创建游戏房间
			gameObject.SetActive (false);	//禁用创建房间面板
			//roomLoadingPanel.SetActive (true);	//启用游戏房间加载提示信息
		}
	}

	//"取消创建"按钮事件处理函数
	public void ClickCancelCreateRoomButton(){
		gameObject.SetActive (false);		//禁用创建房间面板
		roomNameHint.text = "";					//清空房间名称提示文本
	}



	/*
	public GameObject RoomWindow;

	//UserThread thread;
	UnitySocket socket;
	void Start() {
		//socket = GameObject.Find("Socket").GetComponent<UserSocket>().socket;
	}

	public void BeginMatchingBtn() {
		try
		{
			socket.Send(CommandID.BEGIN_MATCHING);
		}catch(Exception e){
			MonoBehaviour.print(e.ToString());
		}
	}

	public void CreateRoomBtn() {
		try
		{
			socket.Send(CommandID.CREATE_ROOM);


		}catch(Exception e){
			MonoBehaviour.print(e.ToString());
		}
	}

	public void DisplayRoomWindow() {
		transform.parent.gameObject.SetActive (false);
		RoomWindow.SetActive (true);
		if (!GameObject.Find ("UserInfo").GetComponent<UserInfo> ().isHost)
			GameObject.Find ("UserUI/Canvas/RoomWindow/BeginBtn").SetActive (false);
		else
			GameObject.Find ("UserUI/Canvas/RoomWindow/BeginBtn").SetActive (true);
	}

	void OnApplicationQuit() {
		//print (12389);
		//socket.DoLogout ();
	}
	
	public void ClickQuitBtn() {
		Application.Quit ();
	}
	
	*/
}
