using UnityEngine;
using System.Collections;

public class EventTriggerPhone : EventTrigger 
{
	public MoviesManager 	MoviesManager;
	public AudioSource 		AudioSource;
	public float 			TrakingPhoneTime = 15.0f;
	public bool 			TrackingPhone = false;

	void Start ()
	{
	
	}

	void Update () 
	{
		if( !TrackingPhone )
		{
			return;
		}

		if( AudioSource.time > TrakingPhoneTime )
		{
			MoviesManager.EnableMovie(2);
		}

		if( AudioSource.time >= 25.0f )
		{
			Debug.Log("Set up phone!!!!");
			EventManager.TriggerEvent("phoneInstallNotificationSetUp");
			this.enabled = false;
		}
	}
}
