using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon;

//[RequireHasChild(typeof(Text))]
public class DragText : PunBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

	public bool dragOnSurfaces = true;
	public Color DragColor;
	public Color highlightColor = Color.yellow;

	//public GameObject DraggingText;
	private GameObject m_DraggingText;
	private RectTransform m_DraggingPlane;

	void Start() {
		//isHost = GameObject.Find ("UserInfo").GetComponent<UserInfo> ().isHost;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
//		isHost = GameObject.Find ("UserInfo").GetComponent<UserInfo> ().isHost;
		if (!PhotonNetwork.isMasterClient) {
			return;
		}

		string str = GetComponent<Text> ().text;
		if (str == "")
			return;
		var canvas = FindInParents<Canvas>(gameObject);
		if (canvas == null)
			return;

		// We have clicked something that can be dragged.
		// What we want to do is create an icon for this.
		m_DraggingText = Instantiate(gameObject) as GameObject;
		//m_DraggingText = new GameObject("TempText");

		m_DraggingText.transform.SetParent (canvas.transform, false);
		m_DraggingText.transform.SetAsLastSibling();


		// The icon will be under the cursor.
		// We want it to be ignored by the event system.
		m_DraggingText.AddComponent<IgnoreRaycast>();

		var myText = m_DraggingText.GetComponent<Text>();
		myText.text = GetComponent<Text> ().text;
		myText.color = DragColor;
		//myText.font = Font("Arial");
		//image.sprite = GetComponent<Image>().sprite;
		//myText.SetNativeSize();


		if (dragOnSurfaces)
			m_DraggingPlane = transform as RectTransform;
		else
			m_DraggingPlane = canvas.transform as RectTransform;
		SetDraggedPosition(eventData);
	}

	public void OnDrag(PointerEventData data)
	{
//		isHost = GameObject.Find ("UserInfo").GetComponent<UserInfo> ().isHost;
		if (!PhotonNetwork.isMasterClient) {
			return;
		}

		string str = GetComponent<Text> ().text;
		if (str == "")
			return;
		if (m_DraggingText != null)
			SetDraggedPosition(data);
	}

	private void SetDraggedPosition(PointerEventData data)
	{
		if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
			m_DraggingPlane = data.pointerEnter.transform as RectTransform;

		var rt = m_DraggingText.GetComponent<RectTransform>();
		Vector3 globalMousePos;
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
		{
			rt.position = globalMousePos;
			rt.rotation = m_DraggingPlane.rotation;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (m_DraggingText != null) {
			Destroy (m_DraggingText);
			//GetComponent<Text> ().text = "";
			//Destroy (gameObject);
		}
	}

	static public T FindInParents<T>(GameObject go) where T : Component
	{
		if (go == null) return null;
		var comp = go.GetComponent<T>();

		if (comp != null)
			return comp;

		Transform t = go.transform.parent;
		while (t != null && comp == null)
		{
			comp = t.gameObject.GetComponent<T>();
			t = t.parent;
		}
		return comp;
	}
}
