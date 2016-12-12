using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LoginManager : MonoBehaviour 
{
	public MovieController  Movie;
	public Texture2D		CursorOnEnterTexture;

	public float 			FirstPauseTime;
	public float			PhoneRingTime;
	public float			PhoneTalkTime;
	public float			PowerOnTime;
	public float			PowerOnFadeInTime;
	public float			PowerOnFadeOutTime;

	public RawImage			BlackScreen;
	public RawImage			LoginScreen;
	public RawImage			MovieScreen;

	public GameObject		LoginInObj;
	public GameObject		ShutDownTextObj;
	public GameObject       ShutDownButtonObj;

	public string 			LoginPassword;
	public Text		  		PasswordWrongMessage;
	public AudioSource      AudioSource;
	public List<AudioClip>  AudioClipList;

	private string			m_loginState = "";
	private float			m_timer;

	void Start () 
	{
		m_loginState =  "Black_Screen_No_Sound";
		m_timer = 0.0f;

		Cursor.visible = false;

		if( AudioClipList.Count == 0 )
		{
			AudioClipList = this.GetComponent<AudioFilesReader>().AudioClipList;
		}
	}

	void Update () 
	{
		m_timer += Time.deltaTime;
		switch(m_loginState)
		{
		case "Black_Screen_No_Sound":
			if( m_timer > FirstPauseTime )
			{
				m_timer = 0.0f;
				AudioSourceUpdate( true, 0 );
				m_loginState = "Black_Screen_Phone_Ring";
			}
			break;
		case "Black_Screen_Phone_Ring":
			if( m_timer > PhoneRingTime )
			{
				m_timer = 0.0f;
				AudioSourceUpdate( false, 1 );
				m_loginState = "Black_Screen_Phone_Pick_Up";
			}
			break;
		case "Black_Screen_Phone_Pick_Up":
			if( !AudioSource.isPlaying )
			{
				m_timer = 0.0f;
				AudioSourceUpdate( false, 2 );
				m_loginState = "PC_Start_Up";
				Movie.PlayMovie();
			}
			break;
		case "PC_Start_Up":

			if( MovieScreen.IsActive() )
			{
				MovieScreen.texture = Movie.ControllerMaterial.mainTexture;
			}
				
			if( m_timer > PowerOnTime )
			{
				AudioSourceUpdate( false, 3 );
				AudioSource.Play();
				m_timer = 0.0f;
				m_loginState = "PC_Login";

				MovieScreen.gameObject.SetActive(false);
			}

			if( m_timer < PowerOnFadeInTime )
			{
				AudioSource.volume = m_timer / PowerOnFadeInTime;
			}
			else if( m_timer > PowerOnTime - PowerOnFadeOutTime && m_timer < PowerOnTime )
			{
				AudioSource.volume = ( PowerOnTime - m_timer ) / PowerOnFadeOutTime;
			}
			else
			{
				if( !MovieScreen.IsActive() )
				{
					MovieScreen.gameObject.SetActive(true);
				}
			}
			break;
		case "PC_Login":
			AudioSource.volume = 1.0f;
			BlackScreen.CrossFadeAlpha( 0.0f, 0.3f, false);
			Cursor.visible = true;

			m_timer += Time.deltaTime;
			if( m_timer > 0.3f )
			{
				BlackScreen.gameObject.SetActive(false);
			}

			break;
		default:
			break;

		}
	}

	private void AudioSourceUpdate( bool loop, int index )
	{
		AudioSource.Stop();
		AudioSource.loop = loop;
		AudioSource.clip = AudioClipList[index];
		AudioSource.Play();
	}

	public void ShutDownButtonPointerEnter( Text text )
	{
		Cursor.SetCursor( CursorOnEnterTexture, new Vector2( 10, 8 ), CursorMode.Auto);
	}

	public void ShutDownButtonPointerExit( Text text )
	{
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	}

	public void ShutDownSystem()
	{
		ShutDownButtonObj.GetComponent<Button>().interactable = false;
		AudioSource.volume = 1.0f;
		AudioSourceUpdate( false, 4 );
		StartCoroutine( LoginInObjFadeOut() );
	}

	IEnumerator LoginInObjFadeOut()
	{
		CanvasGroup loginIn = LoginInObj.GetComponent<CanvasGroup>(); 
		while(loginIn.alpha > 0)
		{                   
			loginIn.alpha -= Time.deltaTime/0.1f;   
			yield return null;
		}
		ShutDownTextObj.SetActive(true);
		yield return new WaitForSeconds( 5.0f );
		Application.Quit();
	}
}
