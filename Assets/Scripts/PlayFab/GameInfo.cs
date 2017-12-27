using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

public class GameResult{
	public int rank { get; set;}
	public int damage{ get; set;}
}

public class GameInfo : MonoBehaviour {
	
	public static List<CatalogItem> catalogItems;                   //游戏道具数据列表
	public static Dictionary<string, string> titleData;     //PlayFab GameManager中存储的游戏数据

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
