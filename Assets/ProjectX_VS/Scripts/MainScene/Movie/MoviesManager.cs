using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MoviesManager : MonoBehaviour
{

	public List<MovieLogicManager> 		MovieLogicManagerList;
	public List<ButtonStateController>	MovieButtonImagesList;
	public List<AudioClip> 				AudioClipList;
	public MovieLogicManager			CurrentMovieLogicManager;
	public AudioSource					AudioSource;
	public RawImage						MovieDisplayUI;
	public Sprite						EnableSprite;
	public int							CurrentMovieLogicManagerIndex	= 0;

	void Start ()
	{
		Init();
	}

	void Update ()
	{
		UpdateMovie();
	}

	//Pay attention here for two audio clips playing
	private void Init()
	{
		CurrentMovieLogicManager = MovieLogicManagerList[CurrentMovieLogicManagerIndex];
		AudioClipList = this.GetComponent<AudioFilesReader>().AudioClipList;
		AudioSource.PlayOneShot( AudioClipList[0] );


		StartCoroutine(InitCameraButtons(0.1f));
		//Jim:Disable for VS test
		StartCoroutine(PlayVoiceAudio( 1, 15.0f ));
	}

	private void UpdateMovie()
	{
		if( CurrentMovieLogicManager.GetCurrentMovieMainTexture() != null )
		{
			MovieDisplayUI.texture = CurrentMovieLogicManager.GetCurrentMovieMainTexture();
		}
	}

	public void OnCam1()
	{
		CurrentMovieLogicManagerIndex = 0;
		MovieLogicManagerList[CurrentMovieLogicManagerIndex].gameObject.SetActive(true);

		MovieLogicManagerList[1].gameObject.SetActive(false);
		MovieLogicManagerList[2].gameObject.SetActive(false);
		CurrentMovieLogicManager = MovieLogicManagerList[CurrentMovieLogicManagerIndex];
	}

	public void OnCam2()
	{
		CurrentMovieLogicManagerIndex = 1;
		MovieLogicManagerList[CurrentMovieLogicManagerIndex].gameObject.SetActive(true);

		MovieLogicManagerList[0].gameObject.SetActive(false);
		MovieLogicManagerList[2].gameObject.SetActive(false);
		CurrentMovieLogicManager = MovieLogicManagerList[CurrentMovieLogicManagerIndex];
	}

	public void OnCam3()
	{
		CurrentMovieLogicManagerIndex = 2;
		MovieLogicManagerList[CurrentMovieLogicManagerIndex].gameObject.SetActive(true);

		MovieLogicManagerList[0].gameObject.SetActive(false);
		MovieLogicManagerList[1].gameObject.SetActive(false);
		CurrentMovieLogicManager = MovieLogicManagerList[CurrentMovieLogicManagerIndex];
	}

	public void OnCam4()
	{
		CurrentMovieLogicManagerIndex = 3;

		CurrentMovieLogicManager = MovieLogicManagerList[CurrentMovieLogicManagerIndex];
	}

	public void EnableMovie( int index )
	{
		MovieLogicManagerList[index].gameObject.SetActive(true);
		MovieLogicManagerList[index].EnableMovieUpdate();
		MovieButtonImagesList[index].EnableButton();
	}

	IEnumerator PlayVoiceAudio( int index, float time )
	{
		yield return new WaitForSeconds( time );
		if( index >= 0 && index < AudioClipList.Count )
		{
			AudioSource.clip = AudioClipList[index];
			AudioSource.Play();

			EventManager.instance.PhoneTrigger.AudioSource = AudioSource;
			EventManager.instance.PhoneTrigger.TrackingPhone = true;
		}
	}

	IEnumerator InitCameraButtons( float time )
	{
		yield return new WaitForSeconds( time );
		for( int i = 0; i < 2; i++ )
		{
			MovieButtonImagesList[i].EnableButton();
		}

		for( int i = 2; i < 4; i++ )
		{
			MovieButtonImagesList[i].DisableButton();
		}
	}
}
