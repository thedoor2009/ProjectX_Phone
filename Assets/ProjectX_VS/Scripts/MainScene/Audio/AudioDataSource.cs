using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioDataSource : MonoBehaviour {

	public static AudioDataSource instance = null;      
	public List<AudioClip> AudiosList;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);    
		}
		AudiosList = this.GetComponent<AudioFilesReader>().AudioClipList;
		DontDestroyOnLoad(gameObject);
	}

	void Start () 
	{
		
	}


	void Update () 
	{

	}

	public AudioClip FindAudio( string audioName )
	{
		foreach( AudioClip audio in AudiosList )
		{
			if( audio.name == audioName )
			{
				return audio;
			}
		}
		return null;
	}
}
