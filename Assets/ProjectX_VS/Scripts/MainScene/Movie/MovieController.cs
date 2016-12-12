using UnityEngine;
using System.Collections;

public class MovieController : MonoBehaviour {

	public MediaPlayerCtrl 			MovieTexture;
	public Material    				ControllerMaterial;
	public string      				MoviePath;
	public bool						TestPlayMovie = false;

	private float	   m_timer;
	private bool	   m_isFirstFrameReady;
	private bool 	   m_flag;

	void Awake () 
	{
		if( MovieTexture == null )
		{
			Debug.LogError( "GameObject: " + this.gameObject.name + "'s MovieTexture has not been assigned. ");
			return;
		}

		if( MoviePath == null || MoviePath == string.Empty )
		{
			Debug.LogError( "GameObject: " + this.gameObject.name + "'s MoviePath is empty. ");
			return;
		}

		ControllerMaterial = MovieTexture.GetComponent<Renderer>().material;
		//MovieTexture.Load(MoviePath);
		m_flag = false;
		m_timer = 0.0f;

		m_isFirstFrameReady = false;
		MovieTexture.OnVideoFirstFramePlaying += OnVideoFirstFramePlay;
	}

	void Update () 
	{
		if(TestPlayMovie)
		{
			MovieTexture.Play();
			TestPlayMovie = false;
		}
	}

	void OnVideoFirstFramePlay()
	{
		MovieTexture.GetComponent<MeshRenderer> ().enabled = true;
		m_isFirstFrameReady = true;
	}

	public bool IsReady( )
	{
		return m_isFirstFrameReady && !(MovieTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY);
	}

	public void SetMoviePath( string moviePath )
	{
		MoviePath = moviePath;
	}

	public void MovieSeekTo( int startTime = 0 )
	{
		MovieTexture.SeekTo(startTime);
	}

	public void PlayMovie( )
	{
		MovieTexture.Play();
	}

	public void PauseMoive( )
	{
		MovieTexture.Pause();
	}

	public void StopMoive( )
	{
		MovieTexture.Stop();
	}


}
