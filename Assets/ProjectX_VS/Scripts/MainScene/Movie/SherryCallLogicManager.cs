using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SherryCallLogicManager : MonoBehaviour
{
	public List<AudioClip> 		AudipClipList;
	public AudioDataManager 	AudioDataManagerObject;
	public AudioSource 			AudioSource;
	public CallAPPManager		CallAppAanager;

	public EntranceMovieLogicManager   EntranceMovieManager;
	public ElevatorMovieLogicManager   ElevatorMovieManager;

	public string 				State;
	public bool   				isCallDone = false;

	void Start ()
	{
		State = "-1";
		AudipClipList = this.GetComponent<AudioFilesReader>().AudioClipList;

		AudioSource.clip = AudipClipList[2];
		AudioSource.Play();
	}

	void Update ()
	{
		switch( State )
		{
		case "-1":
			if( !AudioSource.isPlaying )
			{
				State = "00";
				CallAppAanager.allowedUse = true;
			}

			break;
		case "00":
			if( !AudioDataManagerObject.isReady )
			{
				return;
			}
			AudioSource.clip = AudipClipList[0];
			AudioSource.Play();
			State = "01";

			break;
		case "01":
			if( !AudioSource.isPlaying )
			{
				List<string> audios = new List<string>();
				audios.Add("OneCall_C00");
				audios.Add("OneCall_C01");
				StartCoroutine(CallLoadingVFX(audios));
				State = "02";
			}
			break;
		case "02":
			if( AudioDataManagerObject.audioStartPlayingFlag && !AudioDataManagerObject.audioSource.isPlaying )
			{
				State = "03";

				AudioSource.clip = AudipClipList[1];
				AudioSource.Play();
			}
			break;
		case "03":
			if( !AudioSource.isPlaying )
			{
				State = "04";
				AudioSource.clip = AudipClipList[3];
				AudioSource.Play();
				AudioDataManagerObject.gameObject.SetActive(false);

				EntranceMovieManager.SetMovieLoop(false);
				ElevatorMovieManager.SetMovieLoop(false);
				EventManager.TriggerEvent("SteveEntersHallway");
			}
			break;
		case "04":
			if( !AudioSource.isPlaying )
			{
				isCallDone = true;
				Destroy(this.gameObject);
			}
			break;
		default:
			break;
		}
	}

	private IEnumerator CallLoadingVFX(List<string> audios)
	{
		AudioDataManagerObject.InsertAvailableAudio(audios);
		AudioDataManagerObject.EnableLoadingVFX();
		yield return new WaitForSeconds(1.2f);
		AudioDataManagerObject.DisableLoadingVFX();
		AudioDataManagerObject.RefreshAvailableAudio(false);
	}
}
