using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NotificationController : UIController
{
	public AudioSource	AudioSource;
	public List<AudioClip> AudioClipList;
	public int MaxIndex = 3;

	private string m_notification_state = "";
	private float  m_timer = 0.0f;
	private float  m_time_gap = 20.0f;

	void Start ()
	{
		m_notification_state = "init";
		if( AudioClipList.Count == 0 )
		{
			AudioClipList = this.GetComponent<AudioFilesReader>().AudioClipList;
		}
	}

	void Update ()
	{
		m_timer += Time.deltaTime;
		switch(m_notification_state)
		{
		case "init":
			if( m_timer > m_time_gap )
			{
				m_timer = 0.0f;
				PlayUrgeSound(1);

				m_notification_state = "first_urge";
			}
			break;
		
		case "first_urge":
			if( m_timer > m_time_gap )
			{
				m_timer = 0.0f;
				PlayUrgeSound(2);

				m_notification_state = "second_urge";
			}
			break;
		case "second_urge":
			if( m_timer > m_time_gap )
			{
				m_timer = 0.0f;
				PlayUrgeSound(3);

				m_notification_state = "third_urge";
			}
			break;
		case "third_urge":
			if( m_timer > m_time_gap )
			{
				m_timer = 0.0f;
				m_notification_state = "time_out";
			}
			break;
		case "time_out":
			break;
		}
	}

	private void PlayUrgeSound( int index )
	{
		if( index > MaxIndex - 1 )
		{
			return;
		}
		AudioSource.PlayOneShot( AudioClipList[index]);
	}

	public override void OnClickEvent()
	{
		//TODO
		EventManager.TriggerEvent ("notificationFinishClick");
	}
}
