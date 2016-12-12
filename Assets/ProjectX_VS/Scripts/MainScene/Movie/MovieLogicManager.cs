//This is the base class of movies logic manager. In general, you could find all helper functions
//here which would help you define a particular logic for different camera.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MovieLogicManager : MonoBehaviour {
	//This dictionary is the map for audio choice and movie state
	//eg, when player chooses choice 2, of which audio name is VSTest_A02
	//then we want to play movie state "02" or "movie_state_2" ( any name you prefer ).
	//Therefore, you need to define this map in AudioMovieMatchDictionary dictionary.

	//The following is the example from VS_TestFakeMovieLogicManager
	/*AudioMovieMatchDictionary = new Dictionary<string, string>()
	{
		{"VSTest_A02", "02"},
		{"VSTest_A03", "03"},
		{"VSTest_A04", "04"},
		{"VSTest_A07", "05"},
		{"VSTest_A05", "06"},
		{"VSTest_A06", "07"}
	};
	*/
	public Dictionary<string, string> AudioMovieMatchDictionary;

	//This dictionary is the map for movie state and movie index.Usually we would set the state number
	//as the same value of movie index. But sometimes in speical cases, we might do something different.
	//So this dictionary would help us define this map.

	//The following is the example from VS_TestFakeMovieLogicManager
	/*MovieStateMatchDictionary = new Dictionary<string, int>()
	{
		{"02", 1},
		{"03", 2},
		{"04", 3},
		{"05", 4},
		{"06", 5},
		{"07", 6}
	};
	*/
	public Dictionary<string, int> MovieStateMatchDictionary;

	//Only when enabled is true,then the movie would be updated. Or it would just stay in black.
	//REMEMEBER: Set this value not in code, check or uncheck from game object attached script!!
	public bool Enabled;

	//This variable is for phone app, only when isReadyToCall is true, then phone app would be enabled.
	public bool isReadyToCall;

	//This is how moves logic manager access to audio choice handler data manager.
	public AudioDataManager AudioDataManagerObject;

	public AudioSource audio;
	public List<AudioClip> AudipClipList;
	public List<MovieController> MovieControllerList;

	public float AnnoyTime;
	//Indicate which movie is the loop movie, the index you could check from MovieControllerList
	public int LoopMovieIndex;

	protected class AnnoyMovieData
	{
		public string AnnoyStateName;
		public int    AnnoyMovieIndex;
		public int    AnnoyAudioIndex;
	};

	//This list of audios is the one when you want to update the phone app converstation choices

	/* The following example shows a basic process if you want to update conversation choices
	m_insertAudios.Clear();
	m_insertAudios.Add("VSTest_A02");
	m_insertAudios.Add("VSTest_A03");
	RefreshAudioDataManager();
	*/
	protected List<string> m_insertAudios = new List<string>();

	protected string m_currentMovieMode;
	protected string m_previousMovieMode;
	protected float m_annoyTimer;
	protected int m_totalDuration;
	protected int m_currentDuration;
	protected int m_currentIndex;
	protected int m_nextIndex;
	protected bool m_initFlag = false;
	public bool m_isLoop;

	protected void Start ()
	{
		isReadyToCall = false;
		m_isLoop = true;

		m_currentMovieMode = "";
		m_currentIndex = 0;
		m_nextIndex = 0;
		m_annoyTimer = 0.0f;

		AudipClipList = this.GetComponent<AudioFilesReader>().AudioClipList;

		if( Enabled )
		{
			if(MovieControllerList.Count > 0 )
			{
				MovieControllerList[m_currentIndex].PlayMovie();
			}
		}
	}

	protected void Update ()
	{
		if( Enabled )
		{
			UpdateMovieLogic();
		}
	}

	//This is the most important function you need to override in your own movies logic manager
	protected virtual void UpdateMovieLogic()
	{

	}

	//Set whehther you want current movie to be looped or not
	public void SetMovieLoop( bool loopValue )
	{
		m_isLoop = loopValue;
	}

	//Enable the functionaliy of updating (playing) the movies
	public void EnableMovieUpdate()
	{
		if(MovieControllerList.Count > 0 )
		{
			MovieControllerList[m_currentIndex].PlayMovie();
		}
		Enabled = true;
	}

	//Be prepared for playing next  movie, index here is the index of next movie
	protected void PlayNextMovie( int index )
	{
		if( index < 0 || index > MovieControllerList.Count - 1 )
		{
			return;
		}

		m_nextIndex = index;
		MovieControllerList[m_currentIndex].StopMoive();
		MovieControllerList[index].PlayMovie();

		if( index + 1 < MovieControllerList.Count )
		{
			MovieControllerList[index+1].gameObject.SetActive(true);
		}
	}

	//So far this function has not been used
	protected bool SetUpMovie( int index )
	{
		if(!MovieControllerList[index].IsReady() )
		{
			return false;
		}

		if(!m_initFlag)
		{
			m_totalDuration = MovieControllerList[index].MovieTexture.GetDuration();
			m_initFlag = true;
		}
		return true;
	}

	//Get current playing movie's main texture
	public Texture GetCurrentMovieMainTexture()
	{
		return MovieControllerList[m_currentIndex].ControllerMaterial.mainTexture;
	}

	//Check whether player has made the choice and then play the next movie
	public bool CheckAudioChoice()
	{
		if( AudioDataManagerObject.AudioChoiceName != string.Empty )
		{	
			m_currentMovieMode = AudioMovieMatchDictionary[AudioDataManagerObject.AudioChoiceName];
			AudioDataManagerObject.AudioChoiceName = string.Empty;

			int next_movie_index = MovieStateMatchDictionary[m_currentMovieMode];
			PlayNextMovie(next_movie_index);

			return true;
		}
		return false;
	}

	//Enter the state of playing loop movie ( not make current movie into loop mode )
	public void EnterLoopState()
	{
		m_annoyTimer = 0.0f;
		m_currentMovieMode = "loop";
		PlayNextMovie(LoopMovieIndex);
	}

	//Refresh the choices of conversation
	public void RefreshAudioDataManager()
	{
		AudioDataManagerObject.InsertAvailableAudio(m_insertAudios);
		AudioDataManagerObject.RefreshAvailableAudio(false);
	}

	//Check whether target is into annoyed state
	protected void CheckAnnoyState( AnnoyMovieData annoyMovieData )
	{
		m_annoyTimer += Time.deltaTime;
		if( m_annoyTimer > AnnoyTime )
		{
			PlayAnnoyState( annoyMovieData );
		}
	}

	//Play annoy state movie
	protected void PlayAnnoyState( AnnoyMovieData annoyMovieData )
	{
		m_annoyTimer = 0.0f;
		m_previousMovieMode = m_currentMovieMode;

		m_currentMovieMode = annoyMovieData.AnnoyStateName;
		PlayNextMovie( annoyMovieData.AnnoyMovieIndex );
		PlayAudioClip( annoyMovieData.AnnoyAudioIndex );
	}

	protected void PlayAudioClip( int index )
	{
		if( index < 0 || index > AudipClipList.Count - 1 )
		{
			return;
		}

		audio.clip = AudipClipList[index];
		audio.Play();
	}

	protected void DisablePreviousAndPlayNext()
	{
		if( m_currentIndex == m_nextIndex )
		{
			return;
		}

		if( m_currentIndex != LoopMovieIndex )
		{
			MovieControllerList[m_currentIndex].gameObject.SetActive(false);
		}
		m_currentIndex = m_nextIndex;
	}
}
