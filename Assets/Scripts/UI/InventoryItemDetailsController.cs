using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class InventoryItemDetailsController : MonoBehaviour {
	public GameObject inventoryPanel;            //仓库面板

	public Image itemImage;                 //道具图片
	public Text itemName;                   //道具名字
	public Text itemDescription;            //道具描述
	public Button confirmButton;            //装备按钮

	private ItemInstance item;			// 选中的道具
	Dictionary<string, string> customData;

	// 显示选中道具的详细信息
	void OnEnable () {
		item = InventoryPanelController.userItems [InventoryPanelController.selectedItem];
		itemName.text = item.DisplayName;
//		itemImage.sprite = GameInfo.guns [item.ItemClass];  //显示道具的图片
		//显示道具的详细信息（PlayFab GameManager存储的道具自定义属性）
		foreach (CatalogItem i in GameInfo.catalogItems) {
			if (i.ItemId == item.ItemId) {
				customData = PlayFabSimpleJson.DeserializeObject<Dictionary<string,string>>(i.CustomData);
				break;
			}
		}
		itemDescription.text = "";
		string temp = "";
		foreach (KeyValuePair<string,string> kvp in customData) {
			temp += "\n" + kvp.Key+":"+kvp.Value;
		}
		itemDescription.text = temp.Substring (1);
		confirmButton.GetComponentInChildren<Text>().text = "装备";
		confirmButton.interactable = true;
		// 动态绑定购买按钮的事件
		confirmButton.onClick.RemoveAllListeners();
		confirmButton.onClick.AddListener (delegate {
			confirmButton.GetComponentInChildren<Text>().text = "装备中";
			confirmButton.interactable = false;
			PlayFabUserData.equipedWeapon = item.ItemId;
			Dictionary<string,string> data = new Dictionary<string, string>();
			data.Add("EquipedWeapon",item.ItemId);
			UpdateUserDataRequest request = new UpdateUserDataRequest(){
				Data = data
			};
			PlayFabClientAPI.UpdateUserData (request, OnUpdateUserData, OnPlayFabError);
			InventoryPanelController.isEquiped = true;
			gameObject.SetActive(false);
		});
	}

	//PlayFab GameManager的Player Data更新成功时调用（这里不作任何处理）
	void OnUpdateUserData(UpdateUserDataResult result){
		//Debug.Log ("Player Data Saved");
		confirmButton.GetComponentInChildren<Text>().text = "已装备";
	}

	//PlayFab请求出错时调用，在控制台输出错误信息
	void OnPlayFabError(PlayFabError error){
		Debug.LogError (error.ErrorDetails);
	}

	//取消道具的装备
	public void ClickCancelButton(){
		gameObject.SetActive (false);
	}
}
