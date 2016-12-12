using UnityEngine;
using System.Collections;

public class LoadingVFX : MonoBehaviour {

	public RectTransform Black;
	public float TimeLength;

	private Vector3 m_containerLocalScale;

	void Start () 
	{
		TimeLength = 1.0f;
	}

	public void Reset()
	{
		Black.localScale = new Vector3( 0.0f, 1.0f, 1.0f );
	}

	void Update ()
	{
		if(Black.localScale.x  >= 1.0f ) 
		{
			return;
		}

		Black.localScale += new Vector3( TimeLength, 0.0f, 0.0f ) * Time.deltaTime;
	}
}
