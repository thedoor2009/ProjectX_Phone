using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HallMovieLogicManager  : MovieLogicManager
{
	protected AnnoyMovieData m_annoyData1 = new AnnoyMovieData();
	protected AnnoyMovieData m_annoyData2 = new AnnoyMovieData();

	protected AnnoyMovieData m_currentAnnoyData = new AnnoyMovieData();

	protected void Start ()
	{
		base.Start();

		m_currentMovieMode = "entry_loop";
		LoopMovieIndex = 10;
		m_currentIndex = LoopMovieIndex;
		m_nextIndex = LoopMovieIndex;

		m_annoyData1.AnnoyStateName = "annoy_1";
		m_annoyData1.AnnoyMovieIndex = 8;
		m_annoyData1.AnnoyAudioIndex = 1;

		m_annoyData2.AnnoyStateName = "annoy_2";
		m_annoyData2.AnnoyMovieIndex = 9;
		m_annoyData2.AnnoyAudioIndex = 2;

		m_currentAnnoyData = m_annoyData1;

		AudioMovieMatchDictionary = new Dictionary<string, string>()
		{
			{"VSTest_A02", "02"},
			{"VSTest_A03", "03"},
			{"VSTest_A04", "04"},
			{"VSTest_A07", "05"},
			{"VSTest_A05", "06"},
			{"VSTest_A06", "07"}
		};

		MovieStateMatchDictionary = new Dictionary<string, int>()
		{
			{"02", 2},
			{"03", 3},
			{"04", 4},
			{"05", 5},
			{"06", 6},
			{"07", 7}
		};
	}

	protected void Update ()
	{
		base.Update();
	}

	//The is the main loop we handle the movices playing logic
	protected override void UpdateMovieLogic()
	{
		switch(m_currentMovieMode)
		{
		case "loop":
			//Until the next movie has been loaded and begun to play, then we'll
			//update current movie index with next movie index

			//eg current index = 1, next movie index = 2
			//when next movie is ready
			//current index = next movie index
			if(!MovieControllerList[m_nextIndex].IsReady())
			{
				return;
			}
			m_currentIndex = m_nextIndex;

			//When movie finishes playing
			if(MovieControllerList[m_currentIndex].MovieTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
			{
				//Replay the current movie again
				MovieControllerList[m_currentIndex].MovieTexture.Play();
			}

			//Check whether player has made the choice in conversation
			//If so, then we'll play the next movie.
			//Please check CheckAudioChoice definition for detailed information.
			if( CheckAudioChoice() )
			{
				return;
			}
			else
			{
				CheckAnnoyState(m_currentAnnoyData);
			}

			break;
		case "entry_loop":
			if(!MovieControllerList[m_nextIndex].IsReady())
			{
				return;
			}
			m_currentIndex = m_nextIndex;

			if(MovieControllerList[m_currentIndex].MovieTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
			{
				if(m_isLoop)
				{
					MovieControllerList[m_currentIndex].MovieTexture.Play();
				}
				else
				{
					m_currentMovieMode = "00";
					PlayNextMovie(0);
				}
			}
			break;
		case "00":
			if(!MovieControllerList[m_nextIndex].IsReady())
			{
				return;
			}
			m_currentIndex = m_nextIndex;

			if(MovieControllerList[m_currentIndex].MovieTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
			{
				if(m_isLoop)
				{
					MovieControllerList[m_currentIndex].MovieTexture.Play();
				}
				else
				{
					m_currentMovieMode = "01";
					PlayAudioClip(0);
					PlayNextMovie(1);
				}
			}
			break;
		case "01":
			//Until the next movie has been loaded and begun to play, then we'll
			//update current movie index with next movie index

			//eg current index = 1, next movie index = 2
			//when next movie is ready
			//current index = next movie index
			if(!MovieControllerList[m_nextIndex].IsReady())
			{
				return;
			}
			DisablePreviousAndPlayNext();

			//When movie finishes playing
			if(MovieControllerList[m_currentIndex].MovieTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
			{
				//We'll update m_insertAudios to display the conversation new choices
				m_insertAudios.Clear();
				m_insertAudios.Add("VSTest_A02");
				m_insertAudios.Add("VSTest_A03");
				RefreshAudioDataManager();

				//Play loop movie 
				EnterLoopState();
			}
			break;
		case "02":
			if(!MovieControllerList[m_nextIndex].IsReady())
			{
				return;
			}
			DisablePreviousAndPlayNext();

			if(MovieControllerList[m_currentIndex].MovieTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
			{
				//Video Stop
			}
			break;
		case "03":
			if(!MovieControllerList[m_nextIndex].IsReady())
			{
				return;
			}
			DisablePreviousAndPlayNext();

			if(MovieControllerList[m_currentIndex].MovieTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
			{
				m_insertAudios.Clear();
				m_insertAudios.Add("VSTest_A04");
				m_insertAudios.Add("VSTest_A07");
				RefreshAudioDataManager();

				EnterLoopState();
			}
			break;
		case "04":
			if(!MovieControllerList[m_nextIndex].IsReady())
			{
				return;
			}
			DisablePreviousAndPlayNext();

			if(MovieControllerList[m_currentIndex].MovieTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
			{
				m_insertAudios.Clear();
				m_insertAudios.Add("VSTest_A05");
				m_insertAudios.Add("VSTest_A06");
				RefreshAudioDataManager();

				EnterLoopState();
			}
			break;
		case "05":
			if(!MovieControllerList[m_nextIndex].IsReady())
			{
				return;
			}
			DisablePreviousAndPlayNext();

			if(MovieControllerList[m_currentIndex].MovieTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
			{
				//Video Stop
			}
			break;
		case "06":
			if(!MovieControllerList[m_nextIndex].IsReady())
			{
				return;
			}
			DisablePreviousAndPlayNext();

			if(MovieControllerList[m_currentIndex].MovieTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
			{
				//Video Stop
			}
			break;
		case "07":
			if(!MovieControllerList[m_nextIndex].IsReady())
			{
				return;
			}
			DisablePreviousAndPlayNext();

			if(MovieControllerList[m_currentIndex].MovieTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
			{
				//Video Stop
			}
			break;
		case "annoy_1":
			if(!MovieControllerList[m_nextIndex].IsReady())
			{
				return;
			}
			DisablePreviousAndPlayNext();

			if(MovieControllerList[m_currentIndex].MovieTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
			{
				EnterLoopState();
				m_currentAnnoyData = m_annoyData2;
			}
			break;
		case "annoy_2":
			if(!MovieControllerList[m_nextIndex].IsReady())
			{
				return;
			}
			DisablePreviousAndPlayNext();

			if(MovieControllerList[m_currentIndex].MovieTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
			{
				//Video Stop
			}
			break;
		default:
			break;
		}
	}
}
