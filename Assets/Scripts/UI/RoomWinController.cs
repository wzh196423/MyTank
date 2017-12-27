using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon;

public class RoomWinController : PunBehaviour {
	
	public GameObject lobbyWindow;
    public GameObject loadingWindow;
	//public GameObject Content;
	public GameObject MsgBox;
	//[HideInInspector]
	//public Text[] RoomInfo;

	public int teamCount = 4;
	public int playerPerTeam = 2;
	public GameObject[] Team1;			//队伍1面板（显示队伍1信息）
	public GameObject[] Team2;			//队伍2面板（显示队伍2信息）
	public GameObject[] Team3;			//队伍3面板（显示队伍3信息）
	public GameObject[] Team4;			//队伍4面板（显示队伍4信息）

	//List<GameObject> list = new List<GameObject>();

	public Text roomName;				//房间名称文本


	PhotonView pView;
	Text[] texts;
	ExitGames.Client.Photon.Hashtable costomProperties;


	void OnEnable () {
		pView = GetComponent<PhotonView>();					//获取PhotonView组件
		if(!PhotonNetwork.connected)return;
		roomName.text = PhotonNetwork.room.Name;	//显示房间名称

		teamCount = PhotonNetwork.room.MaxPlayers / 2;
		DisableTeamPanel ();
		UpdateTeamPanel (false);

		if (PhotonNetwork.isMasterClient)
			transform.Find ("BeginBtn").gameObject.SetActive (true);
		else
			transform.Find ("BeginBtn").gameObject.SetActive (false);

		//交替寻找两队空余位置，将玩家信息放置在对应空位置中
		for (int i = 0; i < playerPerTeam; i++) {	
			string Character = (i ==0 ? "driver":"shooter");
			if (Team1 [i].GetComponent<Text>().text == "") {		//在队伍1找到空余位置
				//Team1 [i].SetActive (true);		//激活对应的队伍信息UI
				Text text = Team1 [i].GetComponent<Text> ();
				text.text = PhotonNetwork.playerName;				//显示玩家昵称
				//if(PhotonNetwork.isMasterClient)texts[1].text="房主";	//如果玩家是MasterClient，玩家状态显示"房主"
				//else texts [1].text = "未准备";							//如果玩家不是MasterClient，玩家状态显示"未准备"


				costomProperties = new ExitGames.Client.Photon.Hashtable () {	//初始化玩家自定义属性
					{ "Team","Team1" },		//玩家队伍
					{ "ID", 0},
					{ "Character", Character},
					{ "TeamNum",i },		//玩家队伍序号
				};
				PhotonNetwork.player.SetCustomProperties (costomProperties);	//将玩家自定义属性赋予玩家
				break;
			} else if (Team2 [i].GetComponent<Text>().text == "") {	//在队伍2找到空余位置
				//Team2 [i].SetActive (true);		//激活对应的队伍信息UI
				Text text = Team2 [i].GetComponent<Text> ();		//显示玩家昵称
				//text.text = PhotonNetwork.playerName;	
				//if(PhotonNetwork.isMasterClient)texts[1].text="房主";	//如果玩家是MasterClient，玩家状态显示"房主"
				//else texts [1].text = "未准备";							//如果玩家不是MasterClient，玩家状态显示"未准备"
				costomProperties = new ExitGames.Client.Photon.Hashtable () {	//初始化玩家自定义属性
					{ "Team","Team2" },		//玩家队伍
					{ "ID", 0},
					{ "Character", Character},
					{ "TeamNum",i },		//玩家队伍序号
				};
				PhotonNetwork.player.SetCustomProperties (costomProperties);	//将玩家自定义属性赋予玩家
				break;
			}
			else if (Team3 [i].GetComponent<Text>().text == "") {	//在队伍2找到空余位置
				//Team3 [i].SetActive (true);		//激活对应的队伍信息UI
				Text text = Team3 [i].GetComponent<Text> ();		//显示玩家昵称
				//text.text = PhotonNetwork.playerName;	
				//if(PhotonNetwork.isMasterClient)texts[1].text="房主";	//如果玩家是MasterClient，玩家状态显示"房主"
				//else texts [1].text = "未准备";							//如果玩家不是MasterClient，玩家状态显示"未准备"
				costomProperties = new ExitGames.Client.Photon.Hashtable () {	//初始化玩家自定义属性
					{ "Team","Team3" },		//玩家队伍
					{ "ID", 0},
					{ "Character", Character},
					{ "TeamNum",i },		//玩家队伍序号
				};
				PhotonNetwork.player.SetCustomProperties (costomProperties);	//将玩家自定义属性赋予玩家
				break;
			}
			else if (Team4 [i].GetComponent<Text>().text == "") {	//在队伍2找到空余位置
				//Team4 [i].SetActive (true);		//激活对应的队伍信息UI
				Text text = Team4 [i].GetComponent<Text> ();		//显示玩家昵称
				//text.text = PhotonNetwork.playerName;	
				//if(PhotonNetwork.isMasterClient)texts[1].text="房主";	//如果玩家是MasterClient，玩家状态显示"房主"
				//else texts [1].text = "未准备";							//如果玩家不是MasterClient，玩家状态显示"未准备"
				costomProperties = new ExitGames.Client.Photon.Hashtable () {	//初始化玩家自定义属性
					{ "Team","Team4" },		//玩家队伍
					{ "ID", 0},
					{ "Character", Character},
					{ "TeamNum",i },		//玩家队伍序号
				};
				PhotonNetwork.player.SetCustomProperties (costomProperties);	//将玩家自定义属性赋予玩家
				break;
			}
		}
	}

	public override void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps){
		DisableTeamPanel ();	//禁用队伍面板
		UpdateTeamPanel (true);	//根据当前玩家信息在队伍面板显示所有玩家信息（true表示显示本地玩家信息）
	}

	/**覆写IPunCallback回调函数，当MasterClient更变时调用
	 * 设置ReadyButton的按钮事件
	 */
	public override void OnMasterClientSwitched (PhotonPlayer newMasterClient) {
		//ReadyButtonControl ();
        if(PhotonNetwork.isMasterClient)
            transform.Find("BeginBtn").gameObject.SetActive(true);
    }

	/**覆写IPunCallback回调函数，当有玩家离开房间时调用
	 * 更新队伍面板中显示的玩家信息
	 */ 
	public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer){
		DisableTeamPanel ();	//禁用队伍面板
		UpdateTeamPanel (true);	//根据当前玩家信息在队伍面板显示所有玩家信息（true表示显示本地玩家信息）
	}


	//禁用队伍面板
	void DisableTeamPanel(){
		for (int i = 0; i < Team1.Length; i++) {
			Team1 [i].GetComponent<Text>().text = "";
		}
		for (int i = 0; i < Team2.Length; i++) {
			Team2 [i].GetComponent<Text>().text = "";
		}
		for (int i = 0; i < Team3.Length; i++) {
			Team3 [i].GetComponent<Text>().text = "";
		}
		for (int i = 0; i < Team4.Length; i++) {
			Team4 [i].GetComponent<Text>().text = "";
		}
	}

	void UpdateTeamPanel(bool isUpdateSelf){
		GameObject go;
		foreach (PhotonPlayer p in PhotonNetwork.playerList) {	//获取房间里所有玩家信息
			if (!isUpdateSelf && p.IsLocal)	continue;			//判断是否更新本地玩家信息
			costomProperties = p.CustomProperties;				//获取玩家自定义属性
			Text text;
			if (costomProperties ["Team"].Equals ("Team1")) {	//判断玩家所属队伍
				go = Team1 [(int)costomProperties ["TeamNum"]];	//查询玩家的队伍序号
				//go.SetActive (true);							//激活显示玩家信息的UI
				text = go.GetComponent<Text> ();	//获取显示玩家信息的Text组件
			} else if(costomProperties ["Team"].Equals ("Team2")){											
				go = Team2 [(int)costomProperties ["TeamNum"]];	
				//go.SetActive (true);
				text = go.GetComponent<Text> ();
			}
			else if(costomProperties ["Team"].Equals ("Team3")){											
				go = Team3 [(int)costomProperties ["TeamNum"]];	
				//go.SetActive (true);
				text = go.GetComponent<Text> ();
			}
			else {											
				go = Team4 [(int)costomProperties ["TeamNum"]];	
				//go.SetActive (true);
				text = go.GetComponent<Text> ();
			}
			text.text = p.NickName;						//显示玩家姓名
		}
	}
		
	public void ClickStartGameButton(){
		Transform tran = transform.Find("Teams").transform;
		foreach(Transform t in tran) {
			if((t.Find("Drop Box1").Find("Text").GetComponent<Text>().text == "" && t.Find("Drop Box2").Find("Text").GetComponent<Text>().text != "") || 
				(t.Find("Drop Box1").Find("Text").GetComponent<Text>().text != "" && t.Find("Drop Box2").Find("Text").GetComponent<Text>().text == "")) {
				GameObject myBox = Instantiate (MsgBox);
				myBox.GetComponent<MessageBox> ().Show ("请重新分配队伍");
				return;
			}
		}

		PhotonNetwork.room.IsOpen = false;								//设置房间的open属性，使游戏大厅的玩家无法加入此房间
        //PhotonNetwork.LoadLevel("GameScene");
        pView.RPC ("LoadScene", PhotonTargets.All, "GameScene");	//调用RPC，让游戏房间内所有玩家加载场景GameScene，开始游戏
	}




	public void LeaveBtnClick() {
		PhotonNetwork.LeaveRoom ();						//客户端离开游戏房间
		lobbyWindow.SetActive (true);					//激活游戏大厅面板
		gameObject.SetActive (false);					//禁用游戏房间面板
	}

	//RPC函数，玩家加载场景
	[PunRPC]
	public void LoadScene(string sceneName){	
		//PhotonNetwork.LoadLevel (sceneName);	//加载场景名为sceneName的场景
		SceneManager.LoadScene(sceneName);

	}


	public void exchangePos(int pos1, int pos2) {
//		string temp = RoomInfo [pos1].text;
//		RoomInfo [pos1].text = RoomInfo [pos2].text;
//		RoomInfo [pos2].text = temp;
	}		
}
