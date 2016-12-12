using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DropDownList : MonoBehaviour
{
	public RectTransform Container;
	public Text DisplayText;
	public bool isOpen;
	public bool isSetUp;
	public bool isEnable;

	private Vector3 m_containerLocalScale;

	void Start () 
	{
		if(Container == null )
		{
			Debug.LogError("Please attach recttransform to container.");
		}
		isOpen = false;
		isSetUp = false;
		isEnable = true;
	}

	void Update () 
	{
		if(!isEnable) return;

		m_containerLocalScale = Container.localScale;
		m_containerLocalScale.y = Mathf.Lerp( m_containerLocalScale.y, isOpen ? 1 : 0, Time.deltaTime * 12 );
		Container.localScale = m_containerLocalScale;
	}

	public void SetDisplayText( Text text )
	{
		DisplayText.text = text.text;
		isSetUp = true;
	}

	public void SetEnable( bool state )
	{
		isEnable = state;
	}

	public void OpenDropDownList()
	{
		isOpen = true;
	}

	public void CloseDropDownList()
	{
		isOpen = false;
	}

	public void OnDropDownListClicked () 
	{
		isOpen ^= true;
	}
}
