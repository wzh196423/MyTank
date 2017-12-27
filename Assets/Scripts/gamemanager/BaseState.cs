using System;
using UnityEngine;
using System.Collections;

public class BaseState
{
    protected GameManager gm;
    const float m_photonCircleTime = 4294967.295f;	//Photon服务器循环时间

    private double m_endTime;
    protected double m_countDown;
    private bool m_countDownFlag;
    //GameManager实例
    
    //有参构造函数
    public BaseState(GameManager gm)
    {
        this.gm = gm;
    }

    
    //状态机数据初始化 注意无论什么状态机都会在游戏开始的时候完成初始化
    virtual public void Init()
    {
    }
    //游戏状态机发生切换时 在上一个状态机的Exit函数执行后执行
    virtual public void Enter()
    {
        
    }
    //当前状态机更新 由对应继承自monoBehavior类的update函数调用 如果子类想使用倒计时，需要调用基类update
    virtual public void Update()
    {
        /*
        if (m_countDownFlag)
        {
            m_countDown = m_endTime - PhotonNetwork.time;
            m_countDown = (m_countDown >= m_photonCircleTime) ? m_countDown - m_photonCircleTime : m_countDown;
            if (m_countDown <= 0)
            {
                stopCountDown();
                onCountDownFinish();
            }
        }
        */
    }
    //游戏状态机发生切换时 在下一个状态机的Enter函数之前执行
    virtual public void Exit()
    {
    }
    //检测结束条件 为true则需要进行状态机替换 每帧会调用 注意效率
    virtual public bool checkEndCondition()
    {
        return false;
    }
    //当倒计时结束时触发该函数
    virtual public void onCountDownFinish()
    {
    }
}

