﻿using UnityEngine;
using System.Collections;
using System;
public class showFPS_OnGUI : MonoBehaviour
{

	public float fpsMeasuringDelta = 2.0f;

	private float timePassed;
	private int m_FrameCount = 0;
	private float m_FPS = 0.0f;

	private void Start()
	{
		timePassed = 0.0f;
	}

	private void Update()
	{
		m_FrameCount = m_FrameCount + 1;
		timePassed = timePassed + Time.deltaTime;

		if (timePassed > fpsMeasuringDelta)
		{
			m_FPS = m_FrameCount / timePassed;

			timePassed = 0.0f;
			m_FrameCount = 0;
		}
	}

	private void OnGUI()
	{
		GUIStyle bb = new GUIStyle();
		bb.normal.background = null;    //这是设置背景填充的
		bb.normal.textColor = new Color(1.0f, 0.5f, 0.0f);   //设置字体颜色的
		bb.fontSize = 18;       //当然，这是字体大小

		//在左上角显示FPS
		GUI.Label(new Rect(0, 0, 200, 200), "FPS: " + Math.Ceiling(m_FPS), bb);
	}
}
