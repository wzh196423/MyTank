using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class DriverController : MonoBehaviour {
    TankModel tank;

	// Use this for initialization
	void Start () {
        tank = GetComponent<TankModel>();
	}
	
	// Update is called once per frame
	void Update () {
		#if UNITY_IOS || UNITY_ANDROID
        	float moveH = CrossPlatformInputManager.GetAxisRaw("MoveH_Mobile");
        	float moveV = CrossPlatformInputManager.GetAxisRaw("MoveV_Mobile");
        	tank.TankMove(moveV);
        	tank.TankRotate(moveH);        //根据玩家在水平和垂直轴上的输入，调整玩家的位移
        	float cameraH = CrossPlatformInputManager.GetAxisRaw("CameraH_Mobile");
        	float cameraV = CrossPlatformInputManager.GetAxisRaw("CameraV_Mobile");
        	tank.DriverCameraRotate(cameraH, cameraV);       //根据玩家在水平和垂直轴上的输入，调整玩家视角	
		#endif

		#if UNITY_STANDALONE
        	float moveH = Input.GetAxis("Horizontal");
        	float moveV = Input.GetAxis("Vertical");
        	tank.TankMove(moveV);
        	tank.TankRotate(moveH);

        	float cameraH = Input.GetAxis("CameraH");
        	float cameraV = Input.GetAxis("CameraV");
        	tank.DriverCameraRotate(cameraH, cameraV);
		#endif
    }

}
