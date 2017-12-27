using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class MainPanelController : PunBehaviour {
	public GameObject loginPanel;			//游戏登录面板
	public GameObject mainPanel;			//游戏主面板

	public GameObject roomMessagePanel;		//房间信息面板
	public GameObject shopMessagePanel;		//商店信息面板
	public GameObject InventoryMessagePanel;//道具信息面板

	public Text title;
	public GameObject startBtn;
	public GameObject createRoomBtn;
	public GameObject currency;             //玩家游戏货币面板
	public Text goldCurrencyCount;          //玩家金币数量文本

	int requestNum = 4;

	void OnEnable(){
		//设置title
		title.text = "游戏大厅";

		currency.SetActive(false);

		//禁用所有面板
		if (roomMessagePanel != null)
			roomMessagePanel.SetActive (false);
		if (shopMessagePanel != null)
			shopMessagePanel.SetActive (false);
		if (InventoryMessagePanel != null)
			InventoryMessagePanel.SetActive (false);
		if (startBtn != null)
			startBtn.SetActive (false);
		if (createRoomBtn != null)
			createRoomBtn.SetActive (false);

		//玩家登录后，需要同时向PlayFab发起4个请求
		requestNum = 4;

		//获取玩家数据Player Data
		GetUserDataRequest getUserDataRequest = new GetUserDataRequest ();
		PlayFabClientAPI.GetUserData (getUserDataRequest, OnGetUserData, OnPlayFabError);

		//获取游戏道具的信息
		GetCatalogItemsRequest getCatalogItemsrequest = new GetCatalogItemsRequest()
		{
			CatalogVersion = PlayFabUserData.catalogVersion //武器道具：Cannon
		};
		PlayFabClientAPI.GetCatalogItems(getCatalogItemsrequest, OnGetCatalogItems, OnPlayFabError);

		//获取玩家账户信息
		GetAccountInfoRequest getAccountInfoRequest = new GetAccountInfoRequest()
		{
			PlayFabId = PlayFabUserData.playFabId
		};
		PlayFabClientAPI.GetAccountInfo(getAccountInfoRequest, OnGetAccountInfo, OnPlayFabError);

		//获取游戏数据Title Data
		GetTitleDataRequest getTitleDataRequest = new GetTitleDataRequest();
		PlayFabClientAPI.GetTitleData(getTitleDataRequest, OnGetTitleData, OnPlayFabError);
	}

	//玩家数据Player Data获取成功时调用
	void OnGetUserData(GetUserDataResult result){
		Debug.Log ("User Data Loaded");
		//在本地保存玩家数据
		PlayFabUserData.userData = result.Data;

		//成就系统相关数据保存
//		if (result.Data.ContainsKey("AchievementPoints"))
//			PlayFabUserData.achievementPoints = int.Parse(result.Data["AchievementPoints"].Value);
//		else PlayFabUserData.achievementPoints = 0;
//		if (result.Data.ContainsKey("LV"))
//			PlayFabUserData.lv = int.Parse(result.Data["LV"].Value);
//		else
//			PlayFabUserData.lv = 1;
//		lvValue.text = PlayFabUserData.lv.ToString();
//		if (result.Data.ContainsKey("Exp"))
//			PlayFabUserData.exp = int.Parse(result.Data["Exp"].Value);
//		else PlayFabUserData.exp = 0;

		//天赋系统相关数据保存
//		if (result.Data.ContainsKey("ExpAndMoneySkillLV"))
//			PlayFabUserData.expAndMoneySkillLV = int.Parse(result.Data["ExpAndMoneySkillLV"].Value);
//		else PlayFabUserData.expAndMoneySkillLV = 0;
//		if (result.Data.ContainsKey("ShootingRangeSkillLV"))
//			PlayFabUserData.shootingRangeSkillLV = int.Parse(result.Data["ShootingRangeSkillLV"].Value);
//		else PlayFabUserData.shootingRangeSkillLV = 0;
//		if (result.Data.ContainsKey("ShootingIntervalSkillLV"))
//			PlayFabUserData.shootingIntervalSkillLV = int.Parse(result.Data["ShootingIntervalSkillLV"].Value);
//		else PlayFabUserData.shootingIntervalSkillLV = 0;
//		if (result.Data.ContainsKey("ShootingDamageSkillLV"))
//			PlayFabUserData.shootingDamageSkillLV = int.Parse(result.Data["ShootingDamageSkillLV"].Value);
//		else PlayFabUserData.shootingDamageSkillLV = 0;

		//玩家战斗数据保存
		if (result.Data.ContainsKey ("TotalDamage"))
			PlayFabUserData.totalDamage = int.Parse (result.Data ["TotalDamage"].Value);
		else
			PlayFabUserData.totalDamage = 0;
		if (result.Data.ContainsKey ("TotalGame"))
			PlayFabUserData.totalGame = int.Parse (result.Data ["TotalGame"].Value);
		else
			PlayFabUserData.totalGame = 0;
		
		if (PlayFabUserData.totalGame == 0)
			PlayFabUserData.damagPerGame = (float)PlayFabUserData.totalDamage;
		else
			PlayFabUserData.damagPerGame = PlayFabUserData.totalDamage / PlayFabUserData.totalGame;

		if (result.Data.ContainsKey ("GameResult"))
			PlayFabUserData.gameResults = PlayFabSimpleJson.DeserializeObject<List<GameResult>> (result.Data ["GameResult"].Value);
		else
			PlayFabUserData.gameResults = new List<GameResult>();

		//玩家装备的道具
		if (result.Data.ContainsKey ("EquipedWeapon"))
			PlayFabUserData.equipedWeapon = result.Data["EquipedWeapon"].Value;
		else
			PlayFabUserData.equipedWeapon = "NormalCannon";

		//获取玩家的仓库数据
		GetUserInventoryRequest getUserInventoryRequest = new GetUserInventoryRequest();
		PlayFabClientAPI.GetUserInventory(getUserInventoryRequest, OnGetUserInventory, OnPlayFabError);
	}

	//玩家仓库数据获取成功时调用
	void OnGetUserInventory(GetUserInventoryResult result){
		Debug.Log("User Inventory Loaded");
		//显示玩家的金币、钻石数量
		goldCurrencyCount.text = "金币 : "+result.VirtualCurrency ["JB"].ToString();
		PlayFabUserData.coinNum = int.Parse (result.VirtualCurrency ["JB"].ToString ());
		//检测玩家是否拥有装备道具
		bool hasEquipedWeapon = false;
		foreach (ItemInstance i in result.Inventory) {
			if (i.ItemId == PlayFabUserData.equipedWeapon) {
				hasEquipedWeapon = true;
				break;
			}
		}
		//如果玩家未拥有装备的道具（超出使用期限）
		if (!hasEquipedWeapon)
		{
			PlayFabUserData.equipedWeapon = "NornalCannon";

			//更新玩家属性Player Data“EquipedWeapon”
			UpdateUserDataRequest request = new UpdateUserDataRequest();
			request.Data = new Dictionary<string, string>();
			request.Data.Add("EquipedWeapon", PlayFabUserData.equipedWeapon);
			PlayFabClientAPI.UpdateUserData(request, OnUpdateUserData, OnPlayFabError);
		}
		else
		{
			OnMessageResponse();
			Debug.Log("User Data Saved");
		}

		OnMessageResponse();    //PlayFab的数据是否接收完毕

	}

	//游戏道具数据接收成功后调用
	void OnGetCatalogItems(GetCatalogItemsResult result){
		//在GameInfo中保存游戏道具信息
		GameInfo.catalogItems = result.Catalog;
		OnMessageResponse();    //PlayFab的数据是否接收完毕
	}

	//玩家账号信息接收成功后调用
	void OnGetAccountInfo(GetAccountInfoResult result)
	{
		//在PlayFabUserData中保存玩家邮箱信息
		PlayFabUserData.email = result.AccountInfo.PrivateInfo.Email;
		OnMessageResponse();    //PlayFab的数据是否接收完毕
	}

	//游戏数据接收成功后调用
	void OnGetTitleData(GetTitleDataResult result)
	{
		//在GameInfo中保存游戏数据
		GameInfo.titleData = result.Data;
		OnMessageResponse();    //PlayFab的数据是否接收完毕
	}

	//玩家属性更新成功后调用
	void OnUpdateUserData(UpdateUserDataResult result)
	{
		Debug.Log("User Data Saved");
	}

	//PlayFab的数据是否接收完毕
	void OnMessageResponse()
	{
		requestNum--;
		if (requestNum == 0)        //PlayFab的数据已接收完毕，在游戏主面板显示游戏大厅以及其他信息
		{
			currency.SetActive(true);
			roomMessagePanel.SetActive(true);
			startBtn.SetActive (true);
			createRoomBtn.SetActive (true);
		}
	}

	//PlayFab请求出错时调用，在控制台输出错误信息
	void OnPlayFabError(PlayFabError error)
	{
		Debug.LogError("Get an error:" + error.Error);
	}
	public void ClickLobbyButton(){
		title.text = "游戏大厅";
		disableAllPanelAndButton ();
		startBtn.SetActive (true);
		createRoomBtn.SetActive (true);
		roomMessagePanel.SetActive (true);
		PhotonNetwork.JoinLobby ();
	}

	public void ClickShopButton (){
		title.text = "道具商城";
		disableAllPanelAndButton ();
		shopMessagePanel.SetActive (true);
	}

	public void ClickInventoryButton (){
		title.text = "道具仓库";
		disableAllPanelAndButton();
		InventoryMessagePanel.SetActive (true);
	}

	public void ClickQuitButton() {
		PhotonNetwork.Disconnect();					//断开客户端与Photon服务器的连接
		loginPanel.SetActive(true);					//启用游戏登录面板
		mainPanel.SetActive(false);					//禁用游戏大厅面板
	}

	void disableAllPanelAndButton(){
		startBtn.SetActive (false);
		createRoomBtn.SetActive (false);
		roomMessagePanel.SetActive (false);
		shopMessagePanel.SetActive (false);
		InventoryMessagePanel.SetActive (false);
	}
}
