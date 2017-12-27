using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingState : BaseState {

    public PlayingState(GameManager gm) : base(gm) {
    }


    override public void Init()
    {
        base.Init();
    }

    override public void Enter()
    {
        Debug.Log("playing");
        gm.UICtrl.DisableCanvasCamera();
        gm.UICtrl.SetTime(gm.countDown);
        gm.UICtrl.SetHP(gm.tankModel.health);
        if (PhotonNetwork.isMasterClient)
        {
            gm.photonView.RPC("StartCountDown", PhotonTargets.All, PhotonNetwork.time);
        }

    }

    override public void Update()
    {
        if (gm.endTimer != 0)
        {
            base.Update();
            gm.countDown = gm.endTimer - PhotonNetwork.time;
            if (gm.countDown >= GameManager.photonCircleTime)
                gm.countDown -= GameManager.photonCircleTime;
            if (gm.countDown <= 0.0)
                gm.countDown = 0.0;
            gm.UICtrl.SetTime(gm.countDown);
            gm.UICtrl.SetHP(gm.tankModel.health);
        }
           
        
    }

    override public void Exit()
    {
        if (gm.tankModel.health == 0)
        {
            gm.photonView.RPC("UpdateDeadList", PhotonTargets.All, gm.localTank.name);
        }
        
        gm.localTank.GetPhotonView().TransferOwnership(PhotonNetwork.masterClient);
        gm.localTank.transform.Find("tower").gameObject.GetPhotonView().TransferOwnership(PhotonNetwork.masterClient);
        gm.localTank.transform.Find("tower/cannon").gameObject.GetPhotonView().TransferOwnership(PhotonNetwork.masterClient);
        /*
        foreach(PhotonPlayer player in PhotonNetwork.playerList)
        {
            if(player.CustomProperties["TeamNum"].ToString() == PhotonNetwork.player.CustomProperties["TeamNum"].ToString() 
                && player.CustomProperties["Character"].ToString() == "Shooter")
            {
                gm.photonView.RPC("TransformOwner", )
            }
        }
        */
        gm.photonView.RPC("DestroyTank", PhotonTargets.MasterClient, gm.localTank.name);

    }

    override public bool checkEndCondition()
    {
        return gm.countDown == 0.0 || gm.livedTankCount <= 1 || gm.tankModel.health == 0;
    }

    override public void onCountDownFinish()
    {
    }
}
