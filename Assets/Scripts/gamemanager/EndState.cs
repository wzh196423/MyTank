using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndState : BaseState {

    public EndState(GameManager gm) : base(gm) {
    }


    override public void Init()
    {
        base.Init();
    }

    override public void Enter()
    {
        gm.UICtrl.EnableCanvasCamera();
        gm.UICtrl.DisableLeaveRoomBtn();
        gm.UICtrl.DisableSwitchBtn();
        gm.UICtrl.DisableDriverCanvas();
        gm.UICtrl.DisableShooterCanvas();
        gm.UICtrl.DisableHPSlider();
        gm.UICtrl.DisableTimePanel();
        gm.UICtrl.gameResultWindow.SetActive(true);

    }

    override public void Update()
    {
        base.Update();
    }

    override public void Exit()
    {
        
    }

    override public bool checkEndCondition()
    {
        return false;
    }

    override public void onCountDownFinish()
    {
    }
}
