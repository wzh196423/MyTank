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
	public Sprite[] ballSprites;     //炮弹图片
	public static List<CatalogItem> catalogItems;                   //游戏道具数据列表
	public static Dictionary<string, Sprite> weaponName2Img;                  //枪械名称与图片的映射
	public static Dictionary<string, string> titleData;     //PlayFab GameManager中存储的游戏数据

	// Use this for initialization
	void Start () {
		weaponName2Img = new Dictionary<string, Sprite> ();
		for (int i = 0; i < ballSprites.Length; i++) {
			weaponName2Img.Add (ballSprites [i].name, ballSprites [i]);
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
