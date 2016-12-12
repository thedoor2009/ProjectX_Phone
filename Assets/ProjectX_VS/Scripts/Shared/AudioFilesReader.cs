using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioFilesReader : MonoBehaviour
{
	public List<string> AudioFileNameList;
	public List<AudioClip> AudioClipList;

	private bool m_isReady = false;

	void Awake()
	{
		InitAudioClipList();
	}

	void Update ()
	{
	
	}

	private void InitAudioClipList()
	{
		if( AudioFileNameList == null || AudioFileNameList.Count == 0 )
		{
			Debug.LogError(" Please check " + this.gameObject.name + " AudioFilesReader script. AudioFileNameList is empty.");
		}

		int count = AudioFileNameList.Count;
		for( int index = 0; index < count; index++ )
		{
			AudioClip audioClip = Resources.Load( "Audios/" + AudioFileNameList[index] ) as AudioClip;
			AudioClipList.Add( audioClip );
		}

		m_isReady = true;
	}

	public bool IsReady()
	{
		return m_isReady;
	}
}
