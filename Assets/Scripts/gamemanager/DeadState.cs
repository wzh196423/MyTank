using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadState : BaseState {

    private GameObject curCamera;

    public DeadState(GameManager gm) : base(gm) {
    }


    override public void Init()
    {
        base.Init();
    }

    override public void Enter()
    {
        curCamera = gm.SwitchCamera();
        curCamera.SetActive(true);
        gm.UICtrl.EnableLeaveRoomBtn();
        gm.UICtrl.EnableSwitchBtn();
        gm.UICtrl.switchBtn.GetComponent<Button>().onClick.AddListener(delegate {
            gm.SwitchCamera();
        });
        gm.UICtrl.DisableDriverCanvas();
        gm.UICtrl.DisableShooterCanvas();
        gm.UICtrl.DisableHPSlider();

    }

    override public void Update()
    {
        base.Update();
        gm.countDown = gm.endTimer - PhotonNetwork.time;
        if (gm.countDown >= GameManager.photonCircleTime)
            gm.countDown -= GameManager.photonCircleTime;
        if (gm.countDown <= 0.0)
            gm.countDown = 0.0;
        gm.UICtrl.SetTime(gm.countDown);
        if (curCamera == null)
        {
            curCamera = gm.SwitchCamera();
            curCamera.SetActive(true);
        }
        
    }

    override public void Exit()
    {
        if(curCamera != null)
        {
            curCamera.SetActive(false);
        }
    }

    override public bool checkEndCondition()
    {
        return gm.countDown == 0.0 || gm.livedTankCount == 1;
    }

    override public void onCountDownFinish()
    {
    }
}
