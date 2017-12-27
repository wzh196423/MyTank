using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIController: MonoBehaviour
{
    public GameObject loadingCanvas;
    public GameObject driverCanvas;
    public GameObject shooterCanvas;
    public Slider HPSlider;
    public Text HPText;
    public GameObject gameResultWindow;
    public Text timeLabel;
    public GameObject leaveRoomBtn;
    public GameObject timePanel;
    public GameObject deathInfo;
    public GameObject switchBtn;
	public Text currentObservee;
    public GameObject canvasCamera;

    public void EnableCanvasCamera()
    {
        canvasCamera.SetActive(true);
    }

    public void DisableCanvasCamera()
    {
        canvasCamera.SetActive(false);
    }

    public void SetMaxHP(int maxHP)
    {
        HPSlider.maxValue = maxHP;
    }

    public void SetMinHP(int minHP)
    {
        HPSlider.minValue = minHP;
    }

    public void SetHP(int hp)
    {
        HPSlider.value = hp;
        HPText.text = hp + "/" + HPSlider.maxValue;
    }

    public void SetTime(double countDown)
    {
        int minute = (int)countDown / 60;
        int second = (int)countDown % 60;
        timeLabel.text = minute.ToString("00") + ":" + second.ToString("00");
    }

    public void EnableLoadingCanvas()
    {
        loadingCanvas.SetActive(true);
    
    }

    public void DisableLoadingCanvas()
    {
        loadingCanvas.SetActive(false);
    }

    public void EnableLeaveRoomBtn()
    {
        leaveRoomBtn.SetActive(true);
    }

    public void DisableLeaveRoomBtn()
    {
        leaveRoomBtn.SetActive(false);
    }

    public void EnableSwitchBtn()
    {
        switchBtn.SetActive(true);
		currentObservee.gameObject.SetActive (true);
    }

    public void DisableSwitchBtn()
    {
        switchBtn.SetActive(false);
		currentObservee.gameObject.SetActive (false);
    }

	public void ChangeCurrentObservee(string info){
		currentObservee.text = "当前视角:" + info;
	}

    public void EnableDriverCanvas()
    {
        driverCanvas.SetActive(true);
    }

    public void DisableDriverCanvas()
    {
        driverCanvas.SetActive(false);
    }

    public void EnableShooterCanvas()
    {
        shooterCanvas.SetActive(true);
    }

    public void DisableShooterCanvas()
    {
        shooterCanvas.SetActive(false);
    }

    public void EnableGameResultWindow()
    {
        gameResultWindow.SetActive(true);
    }

    public void DisableGameResultWindow()
    {
        gameResultWindow.SetActive(false);
    }

    public void EnableHPSlider()
    {
        HPSlider.gameObject.SetActive(true);
    }

    public void DisableHPSlider()
    {
        HPSlider.gameObject.SetActive(false);
    }

    public void EnableTimePanle()
    {
        timePanel.SetActive(true);
    }

    public void DisableTimePanel() {
        timePanel.SetActive(false);
    }

    public void ShowDeathInfo(string tankName)
    {
        GameObject c = Instantiate(deathInfo);
        c.transform.Find("DeathInfo").GetComponent<Text>().text = "Tank <color=red>" + tankName + "</color> is destroyed!";
        Destroy(c, 2);
    }
}


