using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CallAPPManager : MonoBehaviour {

	public DropDownList ContactDropDownObject;
	public DropDownList IdentityDropDownObject;
	public GameObject CallIcon;
	public AudioDataManager AudioHandler;
	public Button 	CallButton;

	public Text   	CallTargetName;
	public Text   	CallButtonText;
	public Text 	DisplayContactInfoText;
	public bool 	isCallingNow;
	public bool 	allowedUse;

	void Start () 
	{
		CallButton.interactable = false;
		isCallingNow = false;
		allowedUse = false;
		CallButtonText.color = Color.gray;

		EventManager.TriggerEvent("EnableSherryPhoneCall");
	}

	void Update ()
	{
		if( ContactDropDownObject.isSetUp && IdentityDropDownObject.isSetUp && allowedUse)
		{
			CallButton.interactable = true;
			CallButtonText.color = Color.black;
		}
	}

	public void OnCallClicked()
	{
		ContactDropDownObject.SetEnable(false);
		IdentityDropDownObject.SetEnable(false);
		CallButton.gameObject.SetActive(false);
		CallIcon.SetActive(true);
		StartCoroutine( ClosePhoneCallAPP( 0.0f ) ) ;
		isCallingNow = true;
	}

	public void Reset()
	{
		ContactDropDownObject.SetEnable(true);
		IdentityDropDownObject.SetEnable(true);
		CallButton.gameObject.SetActive(true);
		isCallingNow = false;
	}

	public void SetCallTargetName( string name )
	{
		CallTargetName.text = name;
	}

	IEnumerator ClosePhoneCallAPP( float closeTime )
	{
		yield return new WaitForSeconds( closeTime );

		string displayContactInfoText = "Call " + ContactDropDownObject.DisplayText.text + " As " + IdentityDropDownObject.DisplayText.text;
		DisplayContactInfoText.text = displayContactInfoText;

		this.gameObject.SetActive(false);
		AudioHandler.gameObject.SetActive(true);
		AudioHandler.isReady = true;
		UIMananger.instance.UpdateAppUIDepth(AudioHandler.gameObject.transform);
	}
}
