using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class PrepareState : BaseState
{
    public PrepareState(GameManager gm) : base(gm) {
    }


    override public void Init()
    {
        base.Init();
    }
    
    override public void Enter()
    {
        gm.photonView.RPC("ConfirmLoaded", PhotonTargets.MasterClient);
        
    }
    
    override public void Update()
    {
        base.Update();
        if(PhotonNetwork.isMasterClient && gm.loadedPlayerNum == PhotonNetwork.playerList.Length && gm.waitingForInit)
        {
            gm.InitGame();
            gm.waitingForInit = false;
        }
    }
    
    override public void Exit()
    {
        gm.UICtrl.DisableLoadingCanvas();
   }
   
    override public bool checkEndCondition()
    {
        return gm.readyPlayerNum == PhotonNetwork.playerList.Length;
    }
    
    override public void onCountDownFinish()
    {
    }


}

