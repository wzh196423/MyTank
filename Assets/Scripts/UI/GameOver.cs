using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
public class GameOver : MonoBehaviour {
    
    private GameManager gm;
	public int coinPerGame;
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
				// 排序逻辑，血多的在前，血一样的时候，分高的在前
				if (gm.healthRecord[tank1] != gm.healthRecord[tank2])
					return gm.healthRecord[tank1] - gm.healthRecord[tank2];
				else{
					return gm.damageRecord[tank1] - gm.damageRecord[tank2];
				}
            });
            foreach (string tank in gm.tank_list)
            {
                if(!gm.dead_tank_list.Contains(tank))
                {
                    tankNames[index] = tank;
                    index++;
                }
            }
        }

        for (int i = 0; i < tankNames.Length; i++)
        {
			string teamName = tankNames [tankNames.Length - i - 1];
			GameResult gameresult = new GameResult ();
			gameresult.rank = i + 1;
			if (gm.damageRecord.ContainsKey (teamName)) {
				gameresult.damage = gm.damageRecord [teamName];
			} else {
				gameresult.damage = 0;
			}
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

				PlayFabUserData.coinNum += (coinPerGame + gameresult.damage);
				AddUserVirtualCurrencyRequest req = new AddUserVirtualCurrencyRequest () {
					Amount = coinPerGame + gameresult.damage,
					VirtualCurrency = "JB"
				};
				PlayFabClientAPI.AddUserVirtualCurrency (req, (result) => {
				}, (error) => {
				});
				// 更新游戏货币

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
