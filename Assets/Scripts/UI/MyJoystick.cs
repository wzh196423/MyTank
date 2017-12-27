using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class MyJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public enum AxisOption
        {
            // Options for which axes to use
            Both, // Use both
            OnlyHorizontal, // Only horizontal
            OnlyVertical // Only vertical
        }

        public int MovementRange = 100;
        public AxisOption axesToUse = AxisOption.Both; // The options for the axes that the still will use
        public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input
        public string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input

        public Transform Trans;

        Vector3 m_StartAnthoredPos;
        Vector3 m_Center;

        bool m_UseX; // Toggle for using the x axis
        bool m_UseY; // Toggle for using the Y axis
        CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
        CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input

        
       

        void OnEnable()
        {
            CreateVirtualAxes();
            //m_Center = transform.parent.position;
            //m_StartAnthoredPos = GetComponent<RectTransform>().anchoredPosition;
            // m_Center = transform.position;
           
        }

        void Start()
        {
            //m_Center = transform.parent.position;
            //m_StartAnthoredPos = GetComponent<RectTransform>().anchoredPosition;
            //print(m_StartAnthoredPos);
            //print("p:" + transform.parent.position);

        }

        void UpdateVirtualAxes(Vector3 value)
        {
            var delta = m_Center - value;
            delta.y = -delta.y;
            delta /= MovementRange;
            if (m_UseX)
            {
                m_HorizontalVirtualAxis.Update(-delta.x);
            }

            if (m_UseY)
            {
                m_VerticalVirtualAxis.Update(delta.y);
            }
        }

        void CreateVirtualAxes()
        {
            // set axes to use
            m_UseX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
            m_UseY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

            // create new axes based on axes to use
            if (m_UseX)
            {
                m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
                CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
            }
            if (m_UseY)
            {
                m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
                CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
            }
        }


        public void OnDrag(PointerEventData data)
        {
            //print(m_Center);
            //print("p:"+ transform.parent.position);
            Vector3 newPos = Vector3.zero;

            int deltaX = (int)(data.position.x - m_Center.x);
            int deltaY = (int)(data.position.y - m_Center.y);
            int originDis = (int)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            int dis = Mathf.Clamp(originDis, 0, MovementRange);

            if (m_UseX)
            {
                
                newPos.x = deltaX * dis / originDis;
            }

            if (m_UseY)
            {
                newPos.y = deltaY * dis / originDis;
            }
            transform.position = new Vector3(m_Center.x + newPos.x, m_Center.y + newPos.y, m_Center.z + newPos.z);
            UpdateVirtualAxes(transform.position);
        }


        public void OnPointerUp(PointerEventData data)
        {
            GetComponent<RectTransform>().anchoredPosition = m_StartAnthoredPos;
            //GetComponent<RectTransform>().sizeDelta = new Vector2(1.0f, 1.0f);
            //GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            UpdateVirtualAxes(m_Center);
        }


        public void OnPointerDown(PointerEventData data) {
            m_Center = transform.parent.position;
            m_StartAnthoredPos = GetComponent<RectTransform>().anchoredPosition;
        }

        void OnDisable()
        {
            // remove the joysticks from the cross platform input
            if (m_UseX)
            {
                m_HorizontalVirtualAxis.Remove();
            }
            if (m_UseY)
            {
                m_VerticalVirtualAxis.Remove();
            }
        }
    }
}