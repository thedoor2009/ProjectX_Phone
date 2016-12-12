using UnityEngine;
using System.Collections;

public class EventTriggerNotification : EventTrigger 
{
	public GameObject NotificationUI;
	public GameObject MovieApp;
	public GameObject PhoneApp;
	public LoginLoadingVFX LoadingVFX;

	private string m_appName;
	void Start ()
	{
		LoadingVFX.Reset();
	}

	void Update ()
	{
		if( LoadingVFX.IsLoadingFinished )
		{
			OpenAppNotification();
			CloseNotification();
		}
	}

	public void CloseNotification()
	{
		NotificationUI.gameObject.SetActive(false);
	}

	public void OpenNotification()
	{
		NotificationUI.gameObject.SetActive(true);
	}

	public void OpenAppNotification()
	{
		if( m_appName == "movie" )
		{
			MovieApp.SetActive(true);
		}
		else if( m_appName == "phone" )
		{
			PhoneApp.SetActive(true);
		}
	}

	public void SetUpAppNotification( string appName )
	{
		LoadingVFX.Reset();
		LoadingVFX.gameObject.SetActive(false);
		m_appName = appName;
	}

	public void NotificationFinishEvent( )
	{
		LoadingVFX.gameObject.SetActive(true);
		LoadingVFX.Reset();
		UIMananger.instance.UpdateAppUIDepth(LoadingVFX.gameObject.transform);
	}
}
