using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EventTriggerSkype : MonoBehaviour {
	
	public GameObject	NormalPanel;
	public GameObject	CallingPanel;
	public GameObject	AnsweringPanel;
	public Button		SkypeIcon;
	public Text			AnsweringDisplayTime;

	public AudioSource	AudioSource;
	public List<AudioClip> AudipClipList;

	private float answeringTimer = 0.0f;

	public enum SkypeState
	{
		Normal,
		Calling,
		Answering
	}

	public SkypeState skyState;

	void Start () 
	{
		Init();
	}

	void Update () 
	{
		switch(skyState)
		{
		case SkypeState.Normal:
			break;
		case SkypeState.Calling:
			if(!AudioSource.isPlaying)
			{
				AudioSource.Play();
			}
			break;
		case SkypeState.Answering:
			UpdateDisplayTime();
			UpdateAnswertingState();
			break;
		default:
			break;
		}
	}

	private void Init()
	{
		NormalPanel.SetActive(false);
		CallingPanel.SetActive(false);
		AnsweringPanel.SetActive(false);

		AudipClipList = this.GetComponent<AudioFilesReader>().AudioClipList;
	}

	private void UpdateDisplayTime()
	{
		answeringTimer += Time.deltaTime;
		string showAnsweringTime = "";
		if( answeringTimer < 60.0f )
		{
			if( answeringTimer < 10.0f )
			{
				showAnsweringTime = "00 : " + "0" + ((int)(answeringTimer)).ToString();
			}
			else
			{
				showAnsweringTime = "00 : " + ((int)(answeringTimer)).ToString();
			}
		}
		else 
		{
			float min = answeringTimer / 60.0f;
			if( min < 10 )
			{
				showAnsweringTime = "0" + ((int)(min)).ToString() + " : ";
			}
			else
			{
				showAnsweringTime = ((int)(min)).ToString() + " : ";
			}
			
			float seconds = answeringTimer % 60.0f;
			if( seconds < 10.0f )
			{
				showAnsweringTime = showAnsweringTime + "0" + ((int)(seconds)).ToString();
			}
			else
			{
				showAnsweringTime = showAnsweringTime + ((int)(seconds)).ToString();
			}
		}
		AnsweringDisplayTime.text = showAnsweringTime;
	}

	private void UpdateAnswertingState()
	{
		if(!AudioSource.isPlaying)
		{
			skyState = SkypeState.Normal;

			AudioSource.PlayOneShot(AudipClipList[2]);
			Reset();

			EventManager.TriggerEvent ("skypeFinishAnswering");
		}
	}

	public void SetPhoneCall()
	{
		StartCoroutine( PhoneCalling());
	}

	public void OnCancelCall()
	{
		skyState = SkypeState.Normal;
		NormalPanel.SetActive(true);
		CallingPanel.SetActive(false);
		AnsweringPanel.SetActive(false);

		StartCoroutine( PhoneCalling());
		AudioSource.Stop();
	}

	public void OnAnswerCall()
	{
		skyState = SkypeState.Answering;

		NormalPanel.SetActive(false);
		CallingPanel.SetActive(false);
		AnsweringPanel.SetActive(true);

		AudioSource.clip = AudipClipList[1];
		AudioSource.Play();
	}

	private void Reset()
	{
		answeringTimer = 0.0f;
		NormalPanel.SetActive(true);
		CallingPanel.SetActive(false);
		AnsweringPanel.SetActive(false);
	}

	IEnumerator PhoneCalling(float waitingTime = 5.0f)
	{
		yield return new WaitForSeconds( waitingTime );

		skyState = SkypeState.Calling;
		
		NormalPanel.SetActive(false);
		CallingPanel.SetActive(true);
		AnsweringPanel.SetActive(false);

		AudioSource.clip = AudipClipList[0];
		AudioSource.Play();

		SkypeIcon.gameObject.SetActive(true);
	}
}
