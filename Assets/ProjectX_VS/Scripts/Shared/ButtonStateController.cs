using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonStateController : MonoBehaviour {

	public Image  ButtonImage;
	public Button Button;
	public Text   Text;

	public Sprite Highlight;
	public Sprite Standard;
	public Sprite Pressed;
	public Sprite Disabled;

	public Color HighlightColor;
	public Color StandardColor;
	public Color PressedColor;
	public Color DisabledColor;

	private bool m_enabled;

	void Start ()
	{
		EnableButton();
	}

	void Update ()
	{
	
	}

	public void EnableButton()
	{
		Button.interactable = true;
		m_enabled = true;
		ButtonImage.sprite = Standard;
		if( Text!=null ) Text.color = StandardColor;
	}

	public void DisableButton()
	{
		Button.interactable = false;
		m_enabled = false;
		ButtonImage.sprite = Disabled;
		if( Text!=null ) Text.color = DisabledColor;
	}

	public void OnHighlight( Image image )
	{
		if( !m_enabled ) return;
		image.sprite = Highlight;
		if( Text!=null ) Text.color = HighlightColor;
	}

	public void OnPressed( Image image )
	{
		if( !m_enabled ) return;
		image.sprite = Pressed;
		if( Text!=null ) Text.color = PressedColor;
	}

	public void OnReleased( Image image )
	{
		if( !m_enabled ) return;
		image.sprite = Standard;
		if( Text!=null ) Text.color = StandardColor;
	}

	public void OnDisabled( Image image )
	{
		image.sprite = Disabled;
		if( Text!=null ) Text.color = DisabledColor;
	}
}
