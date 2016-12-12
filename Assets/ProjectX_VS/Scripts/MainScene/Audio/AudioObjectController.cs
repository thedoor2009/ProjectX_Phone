using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AudioObjectController : MonoBehaviour {

	public GameObject DisplayTextObject;
	public Button AudioButton;
	public Text DisplayText;
	public Text Content;
	public Text TimeLength;
	public string AudioName;


	private AudioClip m_audioClip;
	void Start ()
	{
		AudioButton = this.GetComponent<Button>();
	}

	void Update ()
	{

	}

	public void SetContent(string contentText)
	{
		Content.text = contentText;
		DisplayText.text = contentText;
	}

	public void SetAudioName( string audioName )
	{
		AudioName = audioName;
		m_audioClip = AudioDataSource.instance.FindAudio( AudioName );

		float length = m_audioClip ? m_audioClip.length : 0.0f;
		SetTimeLength( length );
	}

	public void SetTimeLength( float timeLength )
	{
		string minS, secondS = "";
		int min = (int)timeLength / 60;
		if( min == 0 )
		{
			minS = "00:";

			if(timeLength < 10.0f )
			{
				secondS = ":0" + ((int)timeLength).ToString();
			}
			else
			{
				secondS = ":" + ((int)timeLength).ToString();
			}
		}
		else
		{
			if( min < 10 )
			{
				minS = "0" + min.ToString();
			}
			else
			{
				minS = min.ToString();
			}
			float second = (timeLength % 60.0f);

			if(second < 10.0f )
			{
				secondS = ":0" + ((int)second).ToString();
			}
			else
			{
				secondS = ":" + ((int)second).ToString();
			}
		}
		TimeLength.text = minS + secondS;
	}

	public void SetButtonState( bool state )
	{
		AudioButton.enabled = state;
		AudioButton.image.color = state ? Color.white : Color.gray;
	}

	public void SetDisplayText(bool state)
	{
		DisplayTextObject.SetActive(state && AudioButton.IsActive());
	}
}
