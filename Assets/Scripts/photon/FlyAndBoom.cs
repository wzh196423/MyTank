using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class FlyAndBoom : PunBehaviour
{
	public GameObject boomEffectPrefab;
	public AudioClip boomAudio;
	private float damageRadius;
	private int damage;
	private float range;
	private float v0;
	private float distance = 0.0f;
	private PhotonPlayer owner;
	// Use this for initialization
	void Start ()
	{
//        damageRadius = 50;
//        damage = 20;
//        range = 500;
        v0 = 200;
		string name = gameObject.name.Split('(')[0];
		Debug.Log (gameObject.name);
		foreach (CatalogItem i in GameInfo.catalogItems) {
			//Debug.Log (i.ItemId.Split('-')[0]);
			//  把临时道具的时间信息去掉，比如'-1day'
			if (i.ItemId.Split('-')[0] == name) {
				Dictionary<string,string> data = PlayFabSimpleJson.DeserializeObject<Dictionary<string,string>> (i.CustomData);
				string rangeStr, damageStr, radiusStr;
				data.TryGetValue ("炮弹射程", out rangeStr);
				data.TryGetValue ("炮弹威力", out damageStr);
				data.TryGetValue ("爆炸范围", out radiusStr);
				range = float.Parse (rangeStr);
				damage = int.Parse (damageStr);
				damageRadius = float.Parse (radiusStr);

				break;
			}
		}
		Debug.Log("炮弹射程"+range+" 炮弹威力"+damage+" 爆炸范围"+ damageRadius +"。");
    }

	public override void OnPhotonInstantiate (PhotonMessageInfo info)
	{
		base.OnPhotonInstantiate (info);
		Debug.Log (info.sender);
		owner = info.sender;
	}
	
	// Update is called once per frame
	void Update ()
	{
		distance += v0 * Time.deltaTime;
        //Debug.Log(gameObject.transform.position);
		gameObject.transform.Translate (transform.forward * v0 * Time.deltaTime, Space.World);
		if (distance >= range) {
			boom ();
			Destroy (gameObject);
			Debug.Log ("Loss");
			return;
		}
	}

	void OnCollisionEnter (Collision collision)
	{
        Debug.Log(collision.collider.gameObject.name);
		boom ();
		Destroy (gameObject);
	}

	void boom ()
	{
		boomEffect ();
		if (!PhotonNetwork.isMasterClient)
			return;
		// MasterClient进行爆炸判断
		Collider[] others = Physics.OverlapSphere (gameObject.transform.position,damageRadius);
        List<string> damagedTanks = new List<string>();
		if (others.Length == 0)
			return;
		for (int i = 0; i < others.Length; i++) {
            //Debug.Log (others[i].gameObject.name);
            TankModel tankModel = others[i].gameObject.GetComponentInParent<TankModel>();

            if (others [i].gameObject.tag == "TankPart" && !damagedTanks.Contains(tankModel.gameObject.name)) {
                damagedTanks.Add(tankModel.gameObject.name);
				tankModel.TakeDamage (damage,owner);
			}
		}
		
	}

	void boomEffect ()
	{
		GameObject effect = Instantiate(boomEffectPrefab) as GameObject;
		effect.transform.position = gameObject.transform.position;
		AudioSource.PlayClipAtPoint (boomAudio, gameObject.transform.position);
		Destroy (effect, 2);
	}
		
}
