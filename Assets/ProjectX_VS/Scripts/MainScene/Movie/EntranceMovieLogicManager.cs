using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EntranceMovieLogicManager : MovieLogicManager
{
	protected void Start ()
	{
		base.Start();
		m_currentMovieMode = "loop";
	}

	protected void Update ()
	{
		base.Update();
	}

	protected override void UpdateMovieLogic()
	{
		switch(m_currentMovieMode)
		{
		case "loop":
			if(!MovieControllerList[m_nextIndex].IsReady())
			{
				return;
			}
			DisablePreviousAndPlayNext();

			if(MovieControllerList[m_currentIndex].MovieTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
			{
				if(m_isLoop)
				{
					MovieControllerList[m_currentIndex].MovieTexture.Play();
				}
				else
				{
					m_currentMovieMode = "01";
					PlayNextMovie(1);
				}
			}
			break;
		case "01":
			if(!MovieControllerList[m_nextIndex].IsReady())
			{
				return;
			}
			DisablePreviousAndPlayNext();

			if(MovieControllerList[m_currentIndex].MovieTexture.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.END)
			{
				m_currentMovieMode = "02";
				PlayNextMovie(2);
			}
			break;
		case "02":
			if(!MovieControllerList[m_nextIndex].IsReady())
			{
				return;
			}
			DisablePreviousAndPlayNext();
			break;
		default:
			break;
		}
	}
}
