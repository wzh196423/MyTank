using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;

public class GameManager0 : PunBehaviour {
	//public static GameManager gm;
	const float photonCircleTime = 4294967.295f;
    public static GameState state = GameState.PreStart;
    public enum GameState{PreStart,Playing,End};
	
	//public Camera mainCamera;

	public GameObject driverCanvas;
	public GameObject shooterCanvas;
	public GameObject displayCanvas;
	public Text timeLabel;							//倒计时时间显示
	public GameObject gameResultWindow;						//游戏结束信息
	public Slider hpSlider;						//玩家血

	public double startTimer = 0;
	public double endTimer = 0;
	public double countDown = 0;
	public double gamePlayingTime = 600;
	public float gameOverTime = 10.0f;				//游戏结束时间
	public double checkPlayerTime = 5;
	public int loadedPlayerNum;
	public int bindedPlayerNum;


	public static GameObject localPlayer;
	public static List<string> dead_tank_list = new List<string>();
	public static List<GameObject> tank_list = new List<GameObject>();

    ExitGames.Client.Photon.Hashtable playerCustomProperties;
	
	private Vector3[] locations = {
		new Vector3 (871,1.09f, 560.59f),
		new Vector3 (777.1f, -17f, 759.9f),
		new Vector3 (1198, -11.5f, 1274),
		new Vector3 (846, -21, 1361)
	};
	// Use this for initialization
	void Start () {
        //gm = GetComponent<GameManager> ();
        //mainCamera = Camera.main
        photonView.RPC("ConfirmLoad", PhotonTargets.MasterClient, null);
		if (PhotonNetwork.isMasterClient) {
            //photonView.RPC("ConfirmLoad", PhotonTargets.All, null);
            photonView.RPC ("SetTime", PhotonTargets.All, PhotonNetwork.time, checkPlayerTime);
		}
        
    }

	// Update is called once per frame
	void Update () {
		countDown = endTimer - PhotonNetwork.time;
		if (countDown >= photonCircleTime)
			countDown -= photonCircleTime;
		UpdateTimeLabel();
		switch (state) {
		case GameState.PreStart:
			if (PhotonNetwork.isMasterClient) {
				if (countDown <= 0.0f || loadedPlayerNum == PhotonNetwork.playerList.Length) {
					InstantiatePlayer ();
					photonView.RPC ("StartGame", PhotonTargets.All, startTimer);
				}
			}
			break;
		case GameState.Playing:
			hpSlider.GetComponent<Slider> ().value = localPlayer.GetComponent<TankModel>().health;
			if (PhotonNetwork.isMasterClient) {
				// ********** 判断游戏结束条件，包括只有一个人活着和倒计时结束
				if (dead_tank_list.Count == loadedPlayerNum/2 - 1) {
					photonView.RPC ("EndGame", PhotonTargets.All, PhotonNetwork.time);
				}
				else if (countDown <= 0.0f) {					//游戏倒计时结束，将剩余坦克放入最终名单
					for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
						if (!dead_tank_list.Contains (PhotonNetwork.playerList [i].CustomProperties ["Team"].ToString ()))
							dead_tank_list.Add(PhotonNetwork.playerList [i].CustomProperties ["Team"].ToString ());
					}
					photonView.RPC ("EndGame", PhotonTargets.All, PhotonNetwork.time);
				}
			}
			break;
		case GameState.End:		//游戏胜利状态，倒计时结束，退出游戏房间
			if (countDown <= 0)
				LeaveRoom ();
			break;
		}
	}
	//
	[PunRPC]
	void ConfirmLoad(){
		loadedPlayerNum++;
	}
	//
	[PunRPC]
	void ConfirmBind(){
		bindedPlayerNum++;
	}
	/**IPunCallback回调函数，有玩家断开连接时（离开房间）调用
	 * MasterClient检查双方人数，更新玩家得分榜的显示
	 */
	public override void OnPhotonPlayerDisconnected(PhotonPlayer other){
		if (state != GameState.Playing)		//游戏状态不是游戏进行中，结束函数执行
			return;
		if (PhotonNetwork.isMasterClient) {	//MasterClient检查
			CheckTeamNumber ();				//检查两队人数
		}
	}
	/**检查加载场景的玩家个数
	 * 该函数只由MasterClient调用
	 */
	void CheckTeamNumber(){
		PhotonPlayer[] players = PhotonNetwork.playerList;		//获取房间内玩家列表
		// 判断游戏是否结束
		/*
		int teamOneNum = 0, teamTwoNum = 0;						
		foreach (PhotonPlayer p in players) {					//遍历所有玩家，计算两队人数
			if (p.CustomProperties ["Team"].ToString () == "Team1")
				teamOneNum++;
			else
				teamTwoNum++;
		}
		//如果有某队伍人数为0，另一队获胜
		if (teamOneNum == 0)
			photonView.RPC ("EndGame", PhotonTargets.All, "Team2",PhotonNetwork.time);
		else if (teamTwoNum == 0)
			photonView.RPC ("EndGame", PhotonTargets.All, "Team1",PhotonNetwork.time);*/
	}

	//生成玩家对象
	void InstantiatePlayer(){
		int tankID = 0;
		//playerCustomProperties= PhotonNetwork.player.CustomProperties;	//获取玩家自定义属性
		ExitGames.Client.Photon.Hashtable CustomProperties;
		foreach (PhotonPlayer p in PhotonNetwork.playerList) {		//遍历房间内所有玩家
			if (p.CustomProperties ["Character"].ToString () == "driver") {	//如果有人未准备
				GameObject tankOBJ = PhotonNetwork.Instantiate ("tank", locations[tankID++], Quaternion.identity, 0);
                tankOBJ.GetPhotonView ().TransferOwnership (p);
				CustomProperties = new ExitGames.Client.Photon.Hashtable () {	//初始化玩家自定义属性
					{ "Team",p.CustomProperties["Team"].ToString() },		//玩家队伍
					{"ID",tankOBJ.GetPhotonView().viewID},
					{ "Character", p.CustomProperties["Character"].ToString()},
					{ "TeamNum",(int)p.CustomProperties["TeamNum"] }		//玩家队伍序号
				};
				p.SetCustomProperties (CustomProperties);
				foreach (PhotonPlayer p1 in PhotonNetwork.playerList) {		//遍历房间内所有玩家
					if (p1.CustomProperties ["Team"].ToString () == p.CustomProperties ["Team"].ToString () && p1.CustomProperties ["Character"].ToString () == "shooter") {
						CustomProperties = new ExitGames.Client.Photon.Hashtable () {	//初始化玩家自定义属性
							{ "Team",p1.CustomProperties["Team"].ToString() },		//玩家队伍
							{"ID",tankOBJ.GetPhotonView().viewID},
							{ "Character", p1.CustomProperties["Character"].ToString()},
							{ "TeamNum",(int)p1.CustomProperties["TeamNum"] }		//玩家队伍序号
						};
						p1.SetCustomProperties (CustomProperties);
                        tankOBJ.transform.Find ("tower").gameObject.GetPhotonView ().TransferOwnership (p1);
                        tankOBJ.transform.Find ("tower/cannon").gameObject.GetPhotonView ().TransferOwnership (p1);
					}
				}
			}
		}
			
	}


	[PunRPC]
	void BindPlayer2Tank() {

        
		foreach(GameObject tankOBJ in GameObject.FindGameObjectsWithTag("Player")) {
            //tank_list.Add(tankOBJ);
            foreach (PhotonPlayer p in PhotonNetwork.playerList) {
                print((int)p.CustomProperties["ID"] + " :" + tankOBJ.GetPhotonView().viewID);
                if ((int)p.CustomProperties["ID"] == tankOBJ.GetPhotonView().viewID)
                {
                    tankOBJ.name = p.CustomProperties["Team"].ToString();
                    break;
                }
            }
            if ((int)PhotonNetwork.player.CustomProperties ["ID"] == tankOBJ.GetPhotonView ().viewID) {
				localPlayer = tankOBJ;
				if (PhotonNetwork.player.CustomProperties ["Character"].ToString () == "driver") {

					driverCanvas.SetActive (true);
					localPlayer.transform.Find ("DriverCamera").gameObject.SetActive(true);
                    localPlayer.GetComponent<DriverController>().enabled = true;
                    //localPlayer.GetComponent<TankMove> ().enabled = true;
                    //localPlayer.name = PhotonNetwork.player.CustomProperties["Team"].ToString();
                    //photonView.RPC ("UpdateTankList", PhotonTargets.All, localPlayer);

                } else if (PhotonNetwork.player.CustomProperties ["Character"].ToString () == "shooter") {
					shooterCanvas.SetActive (true);

					//Debug.Log ("find the shooter");

                    localPlayer.GetComponent<ShooterController>().enabled = true;
                    localPlayer.transform.Find ("tower/cannon/ShooterCamera").gameObject.SetActive( true);

					
				}

				displayCanvas.SetActive (true);
				hpSlider.GetComponent<Slider> ().maxValue = localPlayer.GetComponent<TankModel>().maxHealth;
				hpSlider.GetComponent<Slider> ().minValue = 0;
				hpSlider.GetComponent<Slider>().value = localPlayer.GetComponent<TankModel>().health;

				//mainCamera.enabled = false;
				
			}
		}
	}

	//显示倒计时时间
	void UpdateTimeLabel(){
		int minute = (int)countDown / 60;
		int second = (int)countDown % 60;
		timeLabel.text = minute.ToString ("00") + ":" + second.ToString ("00");
	}

	void CheckPlayerConnected (){
		if (countDown <= 0.0f || loadedPlayerNum == PhotonNetwork.playerList.Length) {
			InstantiatePlayer ();
			photonView.RPC ("StartGame", PhotonTargets.All, startTimer);
		}
	}

	[PunRPC]
	void SetTime(double sTime , double dTime){
		startTimer = sTime;
		endTimer = sTime + dTime;
	}

	[PunRPC]
	void StartGame(double timer){
		BindPlayer2Tank ();
		SetTime (timer, gamePlayingTime);
		//InstantiatePlayer ();
		//if (bindedPlayerNum == PhotonNetwork.playerList.Length)
		state = GameState.Playing;
		//AudioSource.PlayClipAtPoint()
	}
	/*[PunRPC]
	void UpdateTankList(GameObject t){
		tank_list.Add (t);
	}*/

	//游戏结束，更改客户端的游戏状态
	[PunRPC]
	void EndGame(double timer){
		// **************游戏结束，打印名次
		for (int i = 0; i < dead_tank_list.Count; i++) {
			string name1 = "";
			string name2 = "";
			for (int j = 0; j < PhotonNetwork.playerList.Length; j++) {
				if (PhotonNetwork.playerList [j].CustomProperties.ContainsValue (dead_tank_list [i])) {
					if (name1 == "")
						name1 = PhotonNetwork.playerList [j].NickName;
					else {
						name2 = PhotonNetwork.playerList [j].NickName;
						break;
					}
				}
			}
			gameResultWindow.transform.Find ("Canvas/Win/Panel").GetChild (i).GetComponentInChildren<Text> ().text = 
				"No." + (i + 1) + " " + name1 + " " + name2;
		}
		state = GameState.End;
		/*
		//如果两队不是平手，游戏结束信息显示获胜队伍胜利
		if (winTeam != "Tie")
			gameResult.text = winTeam + " Wins!";
		if (winTeam == "Tie") 			//如果两队打平
		{	
			gm.state = GameState.Tie;	//游戏状态切换为平手状态
			AudioSource.PlayClipAtPoint (tieAudio, localPlayer.transform.position);	//播放平手音效
			gameResult.text = "Tie!";	//游戏结束信息显示"Tie!"表示平手
		} 
		else if (winTeam == PhotonNetwork.player.CustomProperties ["Team"].ToString ()) 	//如果玩家属于获胜队伍
		{
			gm.state = GameState.GameWin;		//游戏状态切换为游戏胜利状态
			//播放游戏胜利音效
			AudioSource.PlayClipAtPoint (gameWinAudio,localPlayer.transform.position);
		} 
		else //如果玩家属于失败队伍
		{
			gm.state = GameState.GameLose;		//游戏状态切换为游戏失败状态
			//播放游戏失败音效
			AudioSource.PlayClipAtPoint (gameLoseAudio, localPlayer.transform.position);
		}
		*/
		//scorePanel.SetActive(true);		//游戏结束后，显示玩家得分榜
		SetTime (timer, gameOverTime);	//设置游戏结束倒计时时间
	}

	//如果玩家断开与Photon服务器的连接，加载场景GameLobby
	public override void OnConnectionFail(DisconnectCause cause){
		PhotonNetwork.LoadLevel ("LobbyScene");
	}

	//离开房间函数
	public void LeaveRoom(){
		//mainCamera.enabled = true;
		PhotonNetwork.LeaveRoom ();				//玩家离开游戏房间
		PhotonNetwork.LoadLevel ("LobbyScene");	//加载场景GameLobby
	}
	
}
