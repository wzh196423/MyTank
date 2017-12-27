using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon;

public class DropText : PunBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
	public Image containerImage;
	public GameObject receivingText;
	private Color normalColor;
	public Color highlightColor = Color.yellow;

	ExitGames.Client.Photon.Hashtable costomProperties;

	void Start() {
		//socket = GameObject.Find ("Socket").GetComponent<UserSocket> ().socket;
		//isHost = GameObject.Find ("UserInfo").GetComponent<UserInfo> ().isHost;
	}


	public void OnEnable ()
	{
		if (containerImage != null)
			normalColor = containerImage.color;
	}

	public void OnDrop(PointerEventData data)
	{
//		isHost = GameObject.Find ("UserInfo").GetComponent<UserInfo> ().isHost;
		if (!PhotonNetwork.isMasterClient)
			return;
		containerImage.color = normalColor;

		if (receivingText == null)
			return;

		GameObject dropText = GetDropText (data);

		if (dropText != null && dropText.GetComponent<Text> ().text != "") {

			foreach (PhotonPlayer p in PhotonNetwork.playerList) {	//获取房间里所有玩家信息
				string temp = receivingText.GetComponent<Text> ().text;
				costomProperties = p.customProperties;
				if (p.NickName == dropText.GetComponent<Text> ().text) {
					/*
					p.customProperties ["Team"] = receivingText.transform.parent.parent.name;
					p.customProperties ["Character"] = receivingText.transform.parent.name == "Drop Box1" ? "driver":"shooter";
					p.customProperties ["TeamNum"] = receivingText.transform.parent.name == "Drop Box1" ? 0: 1;
*/
					costomProperties = new ExitGames.Client.Photon.Hashtable ()		//重新设置玩家的自定义属性
					{ 
						{ "Team",receivingText.transform.parent.parent.name }, 
						{ "ID", 0},
						{ "Character", receivingText.transform.parent.name == "Drop Box1" ? "driver":"shooter"},
						{ "TeamNum", receivingText.transform.parent.name == "Drop Box1" ? 0: 1 } 
					};
					p.SetCustomProperties (costomProperties);

					if (temp != "") {
						foreach (PhotonPlayer p1 in PhotonNetwork.playerList) {
							if (p1.NickName == temp) {
								/*
								p1.customProperties ["Team"] = dropText.transform.parent.parent.name;
								p1.customProperties ["Character"] = dropText.transform.parent.name == "Drop Box1" ? "driver":"shooter";
								p1.customProperties ["TeamNum"] = dropText.transform.parent.name == "Drop Box1" ? 0: 1;
*/
								costomProperties = new ExitGames.Client.Photon.Hashtable ()		//重新设置玩家的自定义属性
								{ 
									{ "Team", dropText.transform.parent.parent.name }, 
									{ "ID", 0},
									{ "Character", dropText.transform.parent.name == "Drop Box1" ? "driver":"shooter"},
									{ "TeamNum", dropText.transform.parent.name == "Drop Box1" ? 0: 1 } 
								};
								p1.SetCustomProperties (costomProperties);

							}
						}
					}

				
				}
			}
			/*
			string temp = receivingText.GetComponent<Text>().text;
			receivingText.GetComponent<Text>().text = dropText.GetComponent<Text> ().text;
			dropText.GetComponent<Text> ().text = temp;
			costomProperties = PhotonNetwork.player.customProperties;	//获取玩家自定义属性

			costomProperties = new ExitGames.Client.Photon.Hashtable ()		//重新设置玩家的自定义属性
			{ 
				{ "Team",receivingText.transform.parent.parent.name }, 
				{ "Character", receivingText.transform.parent.name == "Drop Box1" ? "driver":"shooter"},
				{ "TeamNum", receivingText.transform.parent.name == "Drop Box1" ? 0: 1 } 
			};
			PhotonNetwork.player.SetCustomProperties (costomProperties);
*/
		}
	}

	public void OnPointerEnter(PointerEventData data)
	{
//		isHost = GameObject.Find ("UserInfo").GetComponent<UserInfo> ().isHost;
		if (!PhotonNetwork.isMasterClient)
			return;
		
		if (containerImage == null)
			return;

		GameObject dropText = GetDropText (data);
		if (dropText != null)
			containerImage.color = highlightColor;
	}

	public void OnPointerExit(PointerEventData data)
	{
	//	isHost = GameObject.Find ("UserInfo").GetComponent<UserInfo> ().isHost;
		if (!PhotonNetwork.isMasterClient)
			return;
		if (containerImage == null)
			return;

		containerImage.color = normalColor;
	}

	private GameObject GetDropText(PointerEventData data)
	{
		var originalObj = data.pointerDrag;
		if (originalObj == null)
			return null;
		return originalObj;
	}
}

