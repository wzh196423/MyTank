using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

public class InventoryPanelController : MonoBehaviour {
	
	public GameObject[] itemMessages;		//玩家道具信息
	public GameObject previousButton;		//"上一页"按钮
	public GameObject nextButton;			//"下一页"按钮
	public Text pageMessage;				//仓库页数文本控件
	public GameObject inventoryItemsPanel;		//仓库信息面板
	public GameObject inventoryItemDetails;      //仓库道具详细信息面板
	public Text inventoryLoadingInfo;

	public static int selectedItem;					//选中的仓库道具
	public static List<ItemInstance> userItems;     //玩家道具列表
	public static bool isEquiped;               //玩家是否切换装备的道具

	private int itemsLength;				//玩家道具总是
	private int currentPageNumber;			//当前道具页
	private int maxPageNumber;				//最大道具页
	private int itemsPerPage = 4;			//每页显示道具个数

	//玩家仓库界面启用后，初始化仓库界面信息
	void OnEnable(){
		currentPageNumber = 1;				//初始化当前房间页
		maxPageNumber = 1;					//初始化最大房间页
		isEquiped = false;
		inventoryLoadingInfo.gameObject.SetActive(true);
		inventoryItemDetails.SetActive (false);
		itemsPerPage = itemMessages.Length;
		foreach (GameObject go in itemMessages) {
			go.SetActive (false);
		}

		//获取玩家仓库信息
		GetUserInventoryRequest request = new GetUserInventoryRequest();
		PlayFabClientAPI.GetUserInventory(request, OnGetUserInventory, OnPlayFabError);

	}

	void OnGetUserInventory(GetUserInventoryResult result){
		userItems = result.Inventory;
		itemsLength = userItems.Count;
		currentPageNumber = 1;
		maxPageNumber = (itemsLength - 1) / itemsPerPage + 1;
		pageMessage.text = currentPageNumber.ToString () + "/" + maxPageNumber.ToString ();
		ButtonControl ();               //翻页按钮控制
		ShowItems ();                   //显示玩家的道具列表
		inventoryLoadingInfo.gameObject.SetActive (false);   //仓库信息读取完毕，禁用提示面板
	}

	//翻页按钮控制
	void ButtonControl(){
		if (currentPageNumber == 1)
			previousButton.SetActive (false);
		else
			previousButton.SetActive (true);
		if (currentPageNumber == maxPageNumber)
			nextButton.SetActive (false);
		else
			nextButton.SetActive (true);
	}

	void ShowItems (){
		int start, end, i, j;
		start = (currentPageNumber - 1) * itemsPerPage;
		if (currentPageNumber == maxPageNumber)
			end = itemsLength;
		else
			end = start + itemsPerPage;
		for (i = start,j = 0; i < end; i++,j++) {
			int itemNum = i;

			Text itemName = itemMessages [j].transform.Find ("Name").GetComponent<Text>();        //道具名称
			Image image = itemMessages [j].transform.Find ("Image").GetComponent<Image>();    //道具图片
			//Text equip = itemMessages [j].transform.Find ("Equip").GetComponent<Text>();          //道具是否装备
			Button button = itemMessages [j].transform.Find ("Button").GetComponent<Button>();    //道具装备按钮

			itemName.text = userItems [i].DisplayName;
			if (GameInfo.weaponName2Img.ContainsKey (userItems [i].ItemId.Split ('-') [0]))
				image.sprite = GameInfo.weaponName2Img [userItems [i].ItemId.Split ('-') [0]];
			else
				image.sprite = null;
			if (PlayFabUserData.equipedWeapon == userItems [i].ItemId) {
				button.GetComponentInChildren<Text>().text = "已装备";
				button.interactable = false;
				//equip.gameObject.SetActive(true);
				//button.gameObject.SetActive (false);
			} else {
				button.GetComponentInChildren<Text>().text = "装备";
				button.interactable = true;
				//equip.gameObject.SetActive(false);
				button.gameObject.SetActive (true);
				//为“装备”按钮绑定响应事件
				button.onClick.RemoveAllListeners ();
				button.onClick.AddListener (delegate {
					selectedItem = itemNum;
					inventoryItemDetails.SetActive (true);
				});
			}
			itemMessages [j].SetActive (true);
		}
		for (; j < itemsPerPage; j++)
			itemMessages [j].SetActive (false);
	}

	//PlayFab请求出错时调用，在控制台输出错误信息
	void OnPlayFabError(PlayFabError error){
		Debug.LogError (error.ErrorDetails);
	}

	//上一页按钮
	public void ClickPreviousButton(){
		currentPageNumber--;
		pageMessage.text = currentPageNumber.ToString () + "/" + maxPageNumber.ToString ();
		ButtonControl ();
		ShowItems ();
	}
	//下一页按钮
	public void ClickNextButton(){
		currentPageNumber++;
		pageMessage.text = currentPageNumber.ToString () + "/" + maxPageNumber.ToString ();
		ButtonControl ();
		ShowItems ();
	}

	//如果玩家更新了装备道具，更新玩家仓库信息的显示
	void Update(){
		if (isEquiped) {
			ShowItems ();
			isEquiped = false;
		}
	}
		
}
