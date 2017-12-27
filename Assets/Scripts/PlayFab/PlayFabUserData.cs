using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
//PlayFab相关的游戏货玩家账号数据的本地存储
public class PlayFabUserData : MonoBehaviour
{

    public static string equipedWeapon = "NornalCannon";              //玩家装备的枪支
    public static string titleId = "8C4D";                  //PlayFab GameManager对应的游戏ID
    public static string catalogVersion = "Cannon";      //游戏道具的目录名

    public static string playFabId = "";        //玩家账号的ID
    public static string username = "wzh666";          //玩家账号的用户名
    public static string email = "";            //玩家账号的绑定邮箱

	public static int coinNum = 0;

    public static Dictionary<string, UserDataRecord> userData;  //PlayFab GameManager中存储的玩家自定义数据Player Data

//    public static int achievementPoints;        //玩家成就点数
//
//    public static int lv = 0;               //玩家等级
//    public static int exp = 0;              //玩家经验值

    //玩家天赋技能等级
//    public static int expAndMoneySkillLV = 0;           //"土豪"技能
//    public static int shootingRangeSkillLV = 0;         //“精通射程”技能
//    public static int shootingIntervalSkillLV = 0;      //“精通射速”技能
//    public static int shootingDamageSkillLV = 0;        //“精通伤害”技能

    //玩家的战斗数据
    public static int totalDamage = 0;            //累计伤害
    public static int totalGame = 0;           //累计游戏数

    public static float damagPerGame = 0.0f;    //平均每场的伤害

	public static List<GameResult> gameResults; // 每局游戏的结果数组
    //public static float winPercentage = 0.0f;   //胜利场次比
}
