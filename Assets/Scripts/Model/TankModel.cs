using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using Photon;

[Serializable]
public class Boundary
{
    public float min, max;
}

public class TankModel : PunBehaviour {

    Rigidbody rb;

    // game parameters
    public float tankMoveSpeed;
    //public float maxSpeed = 10.0f;
    //public float acc = 10.0f;
    public float tankRotateSpeed;
    public float cannonRotateSpeed;
    public float towerRotateSpeed;
    public float driverCameraRotateSpeed;
    public float driverCameraPositionSpeed;
    public int maxHealth;
    public int health;
    public bool isAlive; //玩家是否存活



    public GameObject driverCameraObj;
    public GameObject shooterCameraObj;

    public GameObject cannon;
    public GameObject tower;


    private float cannonRotateAngel = 0.0f;
    public GameObject rotateCenter;

    
    

    private float cameraPositionY;
    private float cameraRotateY;
    private float rotateY;

    private Boundary cannonRotateBoundary = new Boundary();
    private Boundary driverCameraRotateYBoundry = new Boundary();
    private Boundary driverCameraPositionYBoundry = new Boundary();
    //private Boundary tankSpeedBoundry = new Boundary();

    public GameObject cannnonMouth;

    private float timeBetweenShooting = 1.0f;
    private float timeOfLastShooting;

    public Color effect_color;
    [HideInInspector]
    public Image image;
    public GameObject death_prefab;

    private float change_speed = 0.05f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = transform.Find("massCenter").localPosition;
        cameraPositionY = 0.0f;
        cameraRotateY = 0.0f;
        //tankSpeedBoundry.max = 5.0f;
        //tankSpeedBoundry.min = -2.5f;
        driverCameraRotateYBoundry.max = 60;
        driverCameraRotateYBoundry.min = -60;
        driverCameraPositionYBoundry.max = 30.0f;
        driverCameraPositionYBoundry.min = -2.0f;
        rotateY = 0;

        cannonRotateBoundary.max = 30f;
        cannonRotateBoundary.min = -4f;

        timeOfLastShooting = Time.time;


        image = GameObject.Find("GameCanvas/DamageEffect").GetComponent<Image>();
        isAlive = true;
        health = maxHealth;
        image.color = Color.clear;
    }
    //每帧执行一次，检测玩家是否存活
    void Update()
    {
        image.color = Color.Lerp(image.color, Color.clear, change_speed);
        if (health <= 0)
            isAlive = false;
        /*if (!isAlive){
			GameObject.Find ("GameManager").GetComponent<GameManager> ().switchCamera ();
			Destroy (gameObject);
		}*/
    }


    public void TankMove(float v)
    {
   
        //float deltaV = 0;
        //if (v != 0 && Vector3.Magnitude(rb.velocity) < maxSpeed)
        //{
            //Debug.Log(v * -Vector3.forward*25);
            //rb.AddRelativeForce(v * -Vector3.forward * 25, ForceMode.Acceleration);
            //rb.AddRelativeForce(v * -Vector3.forward *50000, ForceMode.Force);
        transform.Translate(-Vector3.forward * tankMoveSpeed * v * Time.deltaTime);
        //}
    }

    //玩家的位移函数
    public void TankRotate(float h)
    {
        
        //Debug.Log("v:"+v);
        //transform.Translate(-Vector3.forward * tankMoveSpeed * v); 
        transform.Rotate(new Vector3(0, 1, 0), tankRotateSpeed * h * Time.deltaTime);
    }

    


    public void CannonRotate(float v)
    {
        cannonRotateAngel += v * cannonRotateSpeed * Time.deltaTime;
        if (cannonRotateAngel <= cannonRotateBoundary.max && cannonRotateAngel >= cannonRotateBoundary.min)
        {  
            cannon.transform.RotateAround(rotateCenter.transform.position, rotateCenter.transform.right, v * cannonRotateSpeed * Time.deltaTime);
        }
       
    }

    public void TowerRotate(float h)
    {
        tower.transform.Rotate(Vector3.forward * towerRotateSpeed * h * Time.deltaTime); 
    }

    public void Shoot()
    {
        print("shoot!");
        /*
        if (!photonView.isMine || !isAlive)
            return;
            */
        float now = Time.time;
        if (now - timeOfLastShooting >= timeBetweenShooting)
        {
            timeOfLastShooting = now;
			string cannonBallName = PlayFabUserData.equipedWeapon;
			PhotonNetwork.Instantiate(cannonBallName, cannnonMouth.transform.position, cannnonMouth.transform.rotation, 0);
        }

    }

    public void ShooterViewFieldChange(float value)
    {
		shooterCameraObj.GetComponentInChildren<Camera>().fieldOfView = 100 - 70 * value;
    }

    //玩家的旋转函数
    public void DriverCameraRotate(float rh, float rv)
    {
        float r = driverCameraRotateSpeed * rh * Time.deltaTime;
        cameraRotateY += r;
        if (cameraRotateY < driverCameraRotateYBoundry.min)
        {
            r = r - (cameraRotateY - driverCameraRotateYBoundry.min);
            cameraRotateY = driverCameraRotateYBoundry.min;
            
        }else if(cameraRotateY > driverCameraRotateYBoundry.max)
        {
            r = r - (cameraRotateY - driverCameraRotateYBoundry.max);
            cameraRotateY = driverCameraRotateYBoundry.max;
        }
        driverCameraObj.transform.Rotate(new Vector3(0, r, 0));

        float p = driverCameraPositionSpeed * rv * Time.deltaTime;
        cameraPositionY += p;
        if (cameraPositionY < driverCameraPositionYBoundry.min)
        {
            p = p - (cameraPositionY - driverCameraPositionYBoundry.min);
            cameraPositionY = driverCameraPositionYBoundry.min;

        }
        else if (cameraPositionY > driverCameraPositionYBoundry.max)
        {
            p = p - (cameraPositionY - driverCameraPositionYBoundry.max);
            cameraPositionY = driverCameraPositionYBoundry.max;
        }
		driverCameraObj.transform.Translate(new Vector3(0, p, 0));
    }


    //玩家扣血函数，用于GameManager脚本中调用，只有主客户端可以调用
	public void TakeDamage(int damage,PhotonPlayer attacker)
    {
		Debug.Log("name:" + gameObject.name + ",attacker:"+attacker.CustomProperties["Team"].ToString());
        //healthSlider.GetComponent<Slider> ().value = health;
        if (PhotonNetwork.isMasterClient)
        {       //MasterClient调用
			int newHp = health;
			newHp -= damage;               //玩家扣血
			if (newHp <= 0)
				newHp = 0;
            //UpdateHP(health);
			photonView.RPC("UpdateHP", PhotonTargets.All, newHp, attacker);  //更新所有客户端，该玩家对象的生命值
                //photonView.RPC("removeTank", PhotonTargets.All, gameObject.name);// 更新所有客户端，将该玩家对象消灭，加入死亡名单，并打印死亡信息
		}
    }

    //RPC函数，更新玩家血量
	[PunRPC]
	void UpdateHP(int newHP,PhotonPlayer attacker)
    {
		GameManager gm = GameManager.GetGMInstance ();
		string attackTeam = attacker.CustomProperties ["Team"].ToString ();
		string thisTankTeam = gameObject.name;
		if (/*gameObject == GameManager.GetGMInstance ().localTank && */ health > newHP) {
			Debug.Log("health:"+health+",newHP:"+newHP);
			if (attackTeam != thisTankTeam) {// 自己打自己不加分
				gm.damageRecord [attackTeam] = gm.damageRecord [attackTeam] + (health - newHP);
			}
			playDamageEffect ();
		}
		Debug.Log ("score:" + GameManager.GetGMInstance ().damageRecord [attackTeam]);
		health = newHP;     //更新玩家血量
		//gm.healthRecord[thisTankTeam] = newHP;;
        if (health <= 0)
        {   //如果玩家已死亡
            isAlive = false;
        }
    }


    void playDamageEffect()
    {
        image.color = effect_color;

    }

    public int getHealth()
    {
        return health;
    }

}
