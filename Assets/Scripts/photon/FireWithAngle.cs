using UnityEngine;
using System.Collections;

public class FireWithAngle : MonoBehaviour
{

	//public GameObject cannonMouth;
	public const float g = 9.8f;
	public GameObject target;
	public float speed = 10;
	//初速度大小

	private float time;
	private float verticalSpeed;
	private Vector3 moveDirection;
	private float angleSpeed;
	private float angle;
	private bool on = false;

	void Start ()
	{  
		float tmepDistance = Vector3.Distance (transform.position, target.transform.position);  
		float tempTime = tmepDistance / speed;  
		float riseTime, downTime;  
		riseTime = downTime = tempTime / 2;  
		verticalSpeed = g * riseTime;  
		transform.LookAt (target.transform.position);  

		float tempTan = verticalSpeed / speed;  
		double hu = Mathf.Atan (tempTan);  
		angle = (float)(180 / Mathf.PI * hu);  
		transform.eulerAngles = new Vector3 (-angle, transform.eulerAngles.y, transform.eulerAngles.z);  
		angleSpeed = angle / riseTime;  

		moveDirection = target.transform.position - transform.position;  
	}

	void Update ()
	{  
		if (on) {  
			//finish  
			return;  
		}  
		time += Time.deltaTime;  
		float test = verticalSpeed - g * time;  
		transform.Translate (moveDirection.normalized * speed * Time.deltaTime, Space.World);  
		transform.Translate (Vector3.up * test * Time.deltaTime, Space.World);  
		float testAngle = -angle + angleSpeed * time;  
		transform.eulerAngles = new Vector3 (testAngle, transform.eulerAngles.y, transform.eulerAngles.z);  
	}

	void OnCollisionEnter (Collision collision)
	{
		if (collision.gameObject.tag.Equals ("Player"))	//根据碰撞物体的标签来判断该物体是否为坦克
			Debug.Log ("On!");			//获取多米诺骨牌撞击音效的AudioSource组件并播放
		else
			Debug.Log ("Pity!");
		Destroy (gameObject);
		on = true;
	}

	//每隔固定时间执行一次，用于物理模拟
	/*void start () {
		//gameObject.GetComponent<Rigidbody>().velocity = -GameObject.Find("CannonMouth").transform.forward * 100;
		//gameObject.GetComponent<Rigidbody> ().velocity = Vector3.up * 100;
		if (v0 == null) {
			v0 = 10;
		}
		timer = 0;
	}

	void update(){
		timer += Time.deltaTime;
		gameObject.transform.Translate (-cannonMouth.transform.forward * X (timer)-cannonMouth.transform.up * Y (timer));
	}
	//当有物体与该物体即将发生碰撞时，调用OnCollisionEnter()函数
	void OnCollisionEnter(Collision collision)	
	{
		if (collision.gameObject.tag.Equals ("Player"))	//根据碰撞物体的标签来判断该物体是否为坦克
			Debug.Log ("On!");			//获取多米诺骨牌撞击音效的AudioSource组件并播放
		else
			Debug.Log ("Pity!");
		Destroy (gameObject);
	}
	float X(float time){
		return time * v0;
	}
	float Y(float time){
		return 0.5f * 9.8f * time * time;
	}
	*/
}
