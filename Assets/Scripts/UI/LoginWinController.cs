using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using PlayFab;
using PlayFab.ClientModels;

public class LoginWinController : PunBehaviour
{
	
	public GameObject mainWindow;
	//public GameObject Handler;
	public InputField Username;
	public InputField Password;
	public GameObject MessageBox;

	//UnitySocket socket;

	void Start() {
		//thread = GameObject.Find ("UserHandler").GetComponent<UserThread> ().thread;
		/*
		if (!PhotonNetwork.connected) {
			SetLoginPanelActive ();								//启用游戏登录面板
			username.text = PlayerPrefs.GetString ("Username");	//在本地保存玩家昵称
		} 
		//如果已连接Photon服务器
		else
			SetLobbyPanelActive ();	//启用游戏大厅面板
		connectionState.text = "";	//初始化网络连接状态文本信息
		*/
	}

	void Update(){		
		//在游戏画面左下角显示当前的网络连接状态
		//Debug.Log(PhotonNetwork.connectionStateDetailed.ToString ());
	}

    //“登录”按钮响应事件
    public void ClickLoginButton()
    {
        
        LoginWithPlayFabRequest request = new LoginWithPlayFabRequest()
        {
            Username = Username.text,
            Password = Password.text,
        };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginResult, OnLoginError);
        

//        //连接Photon服务器
//        PhotonNetwork.ConnectUsingSettings("1.0");
//        //PhotonNetwork.automaticallySyncScene = false;
//        PhotonNetwork.autoCleanUpPlayerObjects = false;   //玩家离开时不清除游戏对象
//        PhotonNetwork.player.NickName = Username.text;          //设置玩家昵称
//        Debug.Log(PhotonNetwork.connectionStateDetailed.ToString());
//        LobbyWindow.SetActive(true);
//        gameObject.SetActive(false);

        //loginingWindow.GetComponentInChildren<Text>().text = "登陆中。。。";
        //loginingWindow.SetActive(true);
    }

    //账号登录成功后，保存玩家信息，连接Photon服务器
    void OnLoginResult(LoginResult result)
    {
        PlayerPrefs.SetString("Username", Username.text);   //将玩家昵称保存在本地
        //在PlayFab保存玩家信息
        PlayFabUserData.playFabId = result.PlayFabId;
        PlayFabUserData.username = Username.text;
        //loginingWindow.SetActive(false);

        //连接Photon服务器
        PhotonNetwork.ConnectUsingSettings("1.0");
        //PhotonNetwork.automaticallySyncScene = false;
        PhotonNetwork.autoCleanUpPlayerObjects = false;   //玩家离开时不清除游戏对象
        PhotonNetwork.player.NickName = Username.text;          //设置玩家昵称
        Debug.Log(PhotonNetwork.connectionStateDetailed.ToString());      
		mainWindow.SetActive(true);
        gameObject.SetActive(false);
       
        
    }

    //登录请求失败时调用，根据错误类型提示玩家登录失败原因
	void OnLoginError(PlayFabError error)
    {
        //loginingWindow.SetActive(false);
        GameObject messageBox = Instantiate(MessageBox);
        
        Debug.Log("Get an error:" + error.Error);
        if (error.Error == PlayFabErrorCode.InvalidParams && (error.ErrorDetails.ContainsKey("Username") || (error.ErrorDetails.ContainsKey("Email"))))
        {
            messageBox.GetComponent<MessageBox>().Show("账号/邮箱输入不符合规范！");           
        }
        else if (error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Password"))
        {
            messageBox.GetComponent<MessageBox>().Show("登录密码长度应为6~24位！");          
        }
        else if (error.Error == PlayFabErrorCode.AccountNotFound)
        {
            messageBox.GetComponent<MessageBox>().Show("账号/邮箱不存在！");            
        }
        else if (error.Error == PlayFabErrorCode.InvalidUsernameOrPassword || error.Error == PlayFabErrorCode.InvalidEmailOrPassword)
        {
            messageBox.GetComponent<MessageBox>().Show("登录密码错误！");          
        }
        else if (error.Error == PlayFabErrorCode.AccountBanned)
        {
            messageBox.GetComponent<MessageBox>().Show("账号/邮箱已被锁定！");  
        }
        else
        {
            messageBox.GetComponent<MessageBox>().Show("未知错误！");
        }
    }

    //“注册”按钮的响应函数
    public void ClickRegisterButton()
    {
            //向PlayFab发起账号注册请求
            RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest()
            {
                Username = Username.text,
                Password = Password.text,
                RequireBothUsernameAndEmail = false  //注册信息需要包含注册账号的账号名和绑定邮箱
            };
            PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterResult, OnRegisterError);
            //loginingWindow.GetComponentInChildren<Text>().text = "账 号 注 册 中...";
            //loginingWindow.SetActive(true);
        
    }

    //注册成功后
    void OnRegisterResult(RegisterPlayFabUserResult result)
    {
        PlayerPrefs.SetString("Username", Username.text);   //将玩家昵称保存在本地
        PlayFabUserData.playFabId = result.PlayFabId;
        PlayFabUserData.username = Username.text;

        //连接Photon服务器
        PhotonNetwork.autoCleanUpPlayerObjects = false;   //玩家离开时不清除游戏对象
        PhotonNetwork.ConnectUsingSettings("1.0");
        PhotonNetwork.player.NickName = Username.text;          //设置玩家昵称
        Debug.Log(PhotonNetwork.connectionStateDetailed.ToString());

		PurchaseItemRequest request = new PurchaseItemRequest () {
			CatalogVersion = PlayFabUserData.catalogVersion,
			VirtualCurrency = "JB",
			Price = 0,
			ItemId = "NormalCannon"
		};
		PlayFabClientAPI.PurchaseItem (request, OnPurchaseItemResult, OnPlayFabError);
        
        


    }

	//道具购买成功后调用
	void OnPurchaseItemResult(PurchaseItemResult result){
		mainWindow.SetActive(true);
		gameObject.SetActive(false);
	}

	void OnPlayFabError(PlayFabError error){
		Debug.Log("初始化新玩家道具出错：" + error.ErrorDetails);
	}


    //账号注册失败时调用，根据返回的错误类型提示玩家出错原因
    void OnRegisterError(PlayFabError error)
    {
        //loginingWindow.SetActive(false);
        string errorMessage;
        Debug.Log("Get an error：" + error.Error);
        if ((error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Username"))
            || error.Error == PlayFabErrorCode.InvalidUsername)
        {
            errorMessage = "游戏账号输入不符合规范";
        }
      
        else if ((error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Password"))
           || error.Error == PlayFabErrorCode.InvalidPassword)
        {
            errorMessage = "游戏密码输入不符合规范";
        }
        else if (error.Error == PlayFabErrorCode.UsernameNotAvailable)
        {
            errorMessage = "该游戏账号已被注册";
        }
        else
        {
            errorMessage = "未知错误";
        }
        GameObject messageBox = Instantiate(MessageBox);
        messageBox.GetComponent<MessageBox>().Show(errorMessage);

    }

	public override void OnConnectionFail(DisconnectCause cause){
		gameObject.SetActive (true);
		mainWindow.SetActive(false);
	}


}
