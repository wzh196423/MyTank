using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class ShopItemDetailsController : MonoBehaviour {
	public GameObject shopPanel;            //商城面板

	public Image itemImage;                 //道具图片
	public Text itemName;                   //道具名字
	public Text itemDescription;            //道具描述
	public Text price;                      //道具价格
	public Button confirmButton;            //确认购买按钮
	public Text goldCurrencyCount;          //玩家金币数量显示

	private CatalogItem item;
	Dictionary<string, string> customData;

	// 显示选中道具的详细信息
	void OnEnable () {
		item = ShopPanelController.shopItems [ShopPanelController.selectedItem];
		itemName.text = item.DisplayName;
		if (item.VirtualCurrencyPrices.ContainsKey ("JB")) {
			price.text = "金币:" + item.VirtualCurrencyPrices ["JB"].ToString ();
		}
		//显示道具的详细信息（PlayFab GameManager存储的道具自定义属性）
		customData = PlayFabSimpleJson.DeserializeObject<Dictionary<string,string>>(item.CustomData);
		itemDescription.text = "";
		if (GameInfo.weaponName2Img.ContainsKey(item.ItemId.Split('-')[0]))
			itemImage.sprite = GameInfo.weaponName2Img [item.ItemId.Split('-')[0]];  //显示道具的图片
		else
			itemImage.sprite = null;
		string temp = "";
		foreach (KeyValuePair<string,string> kvp in customData) {
			temp += "\n" + kvp.Key+":"+kvp.Value;
		}
		itemDescription.text = temp.Substring (1);
		confirmButton.GetComponentInChildren<Text>().text = "购买";
		confirmButton.interactable = true;
		// 动态绑定购买按钮的事件
		confirmButton.onClick.RemoveAllListeners();
		confirmButton.onClick.AddListener (delegate {
			confirmButton.GetComponentInChildren<Text>().text = "购买中";
			confirmButton.interactable = false;
			if (item.VirtualCurrencyPrices.ContainsKey ("JB")) {
				PurchaseItemRequest request = new PurchaseItemRequest () {
					CatalogVersion = PlayFabUserData.catalogVersion,
					VirtualCurrency = "JB",
					Price = (int)item.VirtualCurrencyPrices ["JB"],
					ItemId = item.ItemId
				};
				PlayFabClientAPI.PurchaseItem(request, OnPurchaseItem, OnPlayFabPurchaseError);
			}
		});
	}
	
	// 购买成功后重新获取玩家的仓库信息
	void OnPurchaseItem (PurchaseItemResult result) {
		confirmButton.GetComponentInChildren<Text>().text = "已拥有";
		GetUserInventoryRequest req = new GetUserInventoryRequest ();
		PlayFabClientAPI.GetUserInventory (req, OnGetUserInventory, OnPlayfabError);
	}

	// 
	void OnGetUserInventory (GetUserInventoryResult result){
		// 更新货币数量
		goldCurrencyCount.text = "金币:" + result.VirtualCurrency ["JB"].ToString ();
		// 重新获取玩家道具
		ShopPanelController.userItems = result.Inventory;
		// 更新商店列表
		shopPanel.GetComponent<ShopPanelController>().ShowItems();
	}

	// 购买失败
	void OnPlayFabPurchaseError(PlayFabError error){
		Debug.LogError (error.ErrorDetails);
	}



	void OnPlayfabError(PlayFabError error){
		Debug.Log (error.ErrorDetails);
	}

	//“取消”按钮，取消道具的购买，关闭道具详细信息面板
	public void ClickCancelButton()
	{
		gameObject.SetActive(false);
	}
}
