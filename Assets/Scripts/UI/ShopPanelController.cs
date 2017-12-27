using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Photon;
using PlayFab;
using PlayFab.ClientModels;

public class ShopPanelController : MonoBehaviour {


	public GameObject[] itemMessages;		//游戏道具信息
	public GameObject previousButton;		//"上一页"按钮
	public GameObject nextButton;			//"下一页"按钮
	public Text pageMessage;				//商店页数文本控件
	public GameObject shopItemsPanel;		//商店信息面板
	public GameObject shopItemDetails;      //商城道具详细信息面板
	public Text shopLoadingInfo;

	public static int selectedItem;					//选中的商城道具
	public static List<CatalogItem> shopItems;      //商城道具列表
	public static List<ItemInstance> userItems;     //玩家道具列表

	private int itemsLength;
	private int currentPageNumber;			//当前道具页
	private int maxPageNumber;				//最大道具页
	private int itemsPerPage = 4;			//每页显示道具个数

	//商城面板激活时调用，获取商城道具信息，显示在商城面板中
	void OnEnable(){
		currentPageNumber = 1;				//初始化当前房间页
		maxPageNumber = 1;					//初始化最大房间页	
		shopLoadingInfo.gameObject.SetActive(true);
		shopItemDetails.SetActive (false);
		//lobbyLoadingLabel.SetActive (true);	//启用游戏大厅加载提示信息
		//roomLoadingLabel.SetActive (false);	//禁用游戏房间加载提示信息
		//获取房间信息面板
//		RectTransform rectTransform = shopItemsPanel.GetComponent<RectTransform> ();
//		itemsPerPage = rectTransform.childCount;		//获取房间信息面板的条目数
//
//		//初始化每条房间信息条目
//		itemMessage = new GameObject[itemsPerPage];	
//		for (int i = 0; i < itemsPerPage; i++) {
//			itemMessage [i] = rectTransform.GetChild (i).gameObject;
//			itemMessage [i].SetActive (false);			//禁用房间信息条目
//		}
		itemsPerPage = itemMessages.Length;
		foreach (GameObject go in itemMessages) {
			go.SetActive (false);
		}

		//获取玩家仓库信息
		GetUserInventoryRequest request = new GetUserInventoryRequest();
		PlayFabClientAPI.GetUserInventory(request, OnGetUserInventory, OnPlayFabError);


//		backButton.onClick.RemoveAllListeners ();		//移除返回按钮绑定的所有监听事件
//		backButton.onClick.AddListener (delegate() {	//为返回按钮绑定新的监听事件
//			PhotonNetwork.Disconnect();					//断开客户端与Photon服务器的连接
//			loginPanel.SetActive(true);					//启用游戏登录面板
//			lobbyPanel.SetActive(false);				//禁用游戏大厅面板
//			userMessage.SetActive (false);				//禁用玩家昵称信息
//			backButton.gameObject.SetActive (false);	//禁用返回按钮
//		});
	}

	//玩家仓库信息获取成功时调用
	void OnGetUserInventory(GetUserInventoryResult result)
	{
		userItems = result.Inventory;

		//获取商城道具列表
		GetCatalogItemsRequest request = new GetCatalogItemsRequest()
		{
			CatalogVersion = PlayFabUserData.catalogVersion
		};
		PlayFabClientAPI.GetCatalogItems(request, OnGetCatalogItems, OnPlayFabError);
	}


	//商城道具列表获取成功后调用
	void OnGetCatalogItems(GetCatalogItemsResult result)
	{
		GameInfo.catalogItems = result.Catalog;
		List<CatalogItem> temp = result.Catalog;
		for (int i = temp.Count - 1; i >= 0; i--)
		{
			if (temp[i].VirtualCurrencyPrices.ContainsKey("JB") && temp[i].VirtualCurrencyPrices.ContainsValue(0))    //剔除普通炮弹在商店中的显示（普通炮弹是免费的）
				temp.RemoveAt(i);
		}
		//计算商城道具个数，计算商城面板页数
		shopItems = temp;
		itemsLength = temp.Count;
		currentPageNumber = 1;
		maxPageNumber = (itemsLength - 1) / itemsPerPage + 1;
		pageMessage.text = currentPageNumber.ToString() + "/" + maxPageNumber.ToString();
		ButtonControl();        //翻页按钮控制
		ShowItems();            //显示商城道具
		shopLoadingInfo.gameObject.SetActive(false);
	}

	//翻页按钮控制
	void ButtonControl()
	{
		if (currentPageNumber == 1)
			previousButton.SetActive(false);
		else
			previousButton.SetActive(true);
		if (currentPageNumber == maxPageNumber)
			nextButton.SetActive(false);
		else
			nextButton.SetActive(true);
	}

	//显示商城道具
	public void ShowItems()
	{
		int start, end, i, j;
		start = (currentPageNumber - 1) * itemsPerPage;
		if (currentPageNumber == maxPageNumber)
			end = itemsLength;
		else
			end = start + itemsPerPage;
		Text[] texts;
		//Image[] images;
		Button button;
		for (i = start, j = 0; i < end; i++, j++)
		{
			int itemNum = i;
			texts = itemMessages[j].GetComponentsInChildren<Text>();
			//images = itemMessage[j].GetComponentsInChildren<Image>();
			button = itemMessages[j].GetComponentInChildren<Button>();
			texts[0].text = shopItems[i].DisplayName;
			//images[1].sprite = GameInfo.guns[shopItems[i].ItemClass];
			//道具是金币购买还是钻石购买
			if (shopItems[i].VirtualCurrencyPrices.ContainsKey("JB"))
			{
				texts[1].text = "金币:"+shopItems[i].VirtualCurrencyPrices["JB"].ToString();
				//images[2].sprite = goldCurrencySprite;
			}
			else if (shopItems[i].VirtualCurrencyPrices.ContainsKey("DC"))
			{
				texts[1].text = shopItems[i].VirtualCurrencyPrices["DC"].ToString();
				//images[2].sprite = diamondCurrencySprite;
			}
			button.onClick.RemoveAllListeners();

			//根据道具的id（ItemId），判断玩家是否已经拥有该物品
			bool hasItems = false;
			foreach (ItemInstance ii in userItems)
			{
				if (ii.ItemId == shopItems[i].ItemId)
				{
					hasItems = true;
					break;
				}
			}
			if (hasItems)
			{
				button.interactable = false;
				button.GetComponentInChildren<Text>().text = "已拥有";
			}
			else
			{
				button.interactable = true;
				button.GetComponentInChildren<Text>().text = "购买";
				button.onClick.AddListener(delegate
					{
						selectedItem = itemNum;
						shopItemDetails.SetActive(true);
					});
			}
			itemMessages[j].SetActive(true);
		}
		for (; j < itemsPerPage; j++)
			itemMessages[j].SetActive(false);
	}

	//PlayFab请求出错时调用，在控制台输出错误信息
	void OnPlayFabError(PlayFabError error)
	{
		Debug.LogError(error.ErrorDetails);
	}

	//上一页按钮
	public void ClickPreviousButton()
	{
		currentPageNumber--;
		pageMessage.text = currentPageNumber.ToString() + "/" + maxPageNumber.ToString();
		ButtonControl();
		ShowItems();
	}

	//下一页按钮
	public void ClickNextButton()
	{
		currentPageNumber++;
		pageMessage.text = currentPageNumber.ToString() + "/" + maxPageNumber.ToString();
		ButtonControl();
		ShowItems();
	}
}
