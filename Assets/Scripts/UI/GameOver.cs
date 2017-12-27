using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
public class GameOver : MonoBehaviour {
    
    private GameManager gm;

    public GameObject[] rank;

    void Start() {
        gm = GameManager.GetGMInstance();
        
        for(int i = gm.tank_list.Count; i <4; i++)
        {
            rank[i].SetActive(false);
        }

        

        string[] tankNames = new string[gm.tank_list.Count];
        int index = 0;
        foreach(string tankName in gm.dead_tank_list)
        {
            tankNames[index] = tankName;
            index++;
        }
        if(gm.dead_tank_list.Count != gm.tank_list.Count)
        {
            gm.tank_list.Sort((tank1, tank2) =>
            {
                return tank1.GetComponent<TankModel>().health - tank2.GetComponent<TankModel>().health;
            });
            foreach (GameObject tank in gm.tank_list)
            {
                if(!gm.dead_tank_list.Contains(tank.name))
                {
                    tankNames[index] = tank.name;
                    index++;
                }
            }
        }

        for (int i = 0; i < tankNames.Length; i++)
        {
			string teamName = tankNames [tankNames.Length - i - 1];
			GameResult gameresult = new GameResult ();
			gameresult.rank = i + 1;
			gameresult.damage = gm.damageRecord [teamName];
			rank[i].GetComponentInChildren<Text>().text = "第"+gameresult.rank+"名:"+ teamName + " 本局伤害:" + gameresult.damage;
			if (teamName == PhotonNetwork.player.CustomProperties ["Team"].ToString()) { // 每个人更新自己的游戏数据
				UpdateUserDataRequest request = new UpdateUserDataRequest();
				request.Data = new Dictionary<string, string> ();

				PlayFabUserData.totalGame ++;
				request.Data.Add ("TotalGame", PlayFabUserData.totalGame.ToString ());
				PlayFabUserData.totalDamage += gameresult.damage;
				request.Data.Add ("TotalDamage", PlayFabUserData.totalDamage.ToString ());
				PlayFabUserData.damagPerGame = PlayFabUserData.totalDamage / PlayFabUserData.totalGame;
				PlayFabUserData.gameResults.Add (gameresult);
				request.Data.Add ("GameResult", PlayFabSimpleJson.SerializeObject(PlayFabUserData.gameResults));

				PlayFabClientAPI.UpdateUserData(request, (result) => { }, (error) => { });
			}
//			if (PhotonNetwork.isMasterClient) {
//				// 跟新每局排名和伤害
//				UpdateUserDataRequest request = new UpdateUserDataRequest();
//				Dictionary<string,string> data = new Dictionary<string, string>();
//				data.Add("Rank","");
//			}
        }

    }



	public void clickBtn() {
        gm.LeaveRoom();
	}
}
