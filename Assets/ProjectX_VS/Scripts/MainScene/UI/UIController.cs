using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {

	public Canvas myCanvas;

	private Vector3 	dragLastPos 			= Vector3.zero;

	private bool 		onFirstClick 			= false;
	private bool 		onSecondClick 			= false;
	private float 		onFirstClickTime 		= 0.0f;
	private float 		onClickGapTime 			= 0.3f;

	void Start ()
	{
	
	}

	void Update ()
	{
	
	}

	public void OnDoubleClicked()
	{
		OnClickEvent();

		if( !onFirstClick )
		{
			onFirstClick = true;
			onFirstClickTime = Time.time;
		}
		else
		{
			float currentClickTime = Time.time;
			if( currentClickTime - onFirstClickTime < onClickGapTime )
			{
				Debug.Log("Double Click");
				onFirstClick = false;
				DoubleClickEvent();
			}
			else
			{
				Debug.Log("One Click");
				SingleClickEvent();
			}
			onFirstClickTime = Time.time;
		}
	}

	public virtual void OnClickEvent()
	{
		
	}

	public virtual void DoubleClickEvent()
	{
		
	}

	public virtual void SingleClickEvent()
	{
		
	}

	public void OnDragBegin(Transform button)
	{
		Vector2 pos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);
		dragLastPos = myCanvas.transform.TransformPoint(pos);
	}

	public void OnDrag(Transform button)
	{
		Vector2 pos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);

		if(dragLastPos != Vector3.zero)
		{
			Vector3 offset = myCanvas.transform.TransformPoint(pos) - dragLastPos;
			button.localPosition += offset;
		}
		dragLastPos = myCanvas.transform.TransformPoint(pos);
	}
}
