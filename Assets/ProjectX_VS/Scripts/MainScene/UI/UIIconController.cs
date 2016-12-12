using UnityEngine;
using System.Collections;

public class UIIconController : UIController {

	public GameObject ReferUIApp;
	public GameObject UIAppClickHight;

	public bool IsOnHover		 = false;
	public bool ReferUIAppEnable = false;

	void Start ()
	{
		Init();
	}

	void Update ()
	{
		CheckClickOutsidePanel();
	}

	public void CheckClickOutsidePanel()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if(!IsOnHover)
			{
				UIAppClickHight.SetActive(false);
			}
		}
	}

	private void Init()
	{
		if( !ReferUIAppEnable )
		{
			if( ReferUIApp == null )
			{
				Debug.LogError("Please assgine gameobject to " + this.name + "'s ReferUIApp.");
				return;
			}
			ReferUIApp.SetActive(false);
		}
	}

	public void OnHover()
	{
		IsOnHover = true;
	}

	public void OutHover()
	{
		IsOnHover = false;
	}

	public override void DoubleClickEvent()
	{
		if( ReferUIApp == null )
		{
			Debug.LogError("Please assgine gameobject to " + this.name + "'s ReferUIApp.");
			return;
		}
		ReferUIApp.SetActive(true);
		UIMananger.instance.UpdateAppUIDepth( ReferUIApp.gameObject.transform );
	}

	public override void SingleClickEvent()
	{
		UIAppClickHight.SetActive(true);
	}

	public override void OnClickEvent()
	{
		UIAppClickHight.SetActive(true);
	}
}
