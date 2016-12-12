using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LoginLoadingVFX : MonoBehaviour
{
	public List<GameObject> loadingCubeList;
	public Text 	LoadingDescText;
	public float 	LoadingTime = 2.0f;
	public bool  	IsLoadingFinished = false;

	private float m_timer;
	private float m_loadingGapTime = 0.0f;
	private int m_index = 0;
	void Start ()
	{
		IsLoadingFinished = false;
		if( loadingCubeList.Count == 0 )
		{
			return;
		}
		m_loadingGapTime = LoadingTime / loadingCubeList.Count;
		m_index = 0;
	}

	void Update ()
	{
		if( m_index >= loadingCubeList.Count )
		{
			IsLoadingFinished = true;
			Close();

			return;
		}

		m_timer += Time.deltaTime;
		if( m_timer > m_loadingGapTime )
		{
			m_timer = 0.0f;
			loadingCubeList[m_index++].SetActive(true);
		}
	}

	void Close()
	{
		LoadingDescText.gameObject.SetActive(false);
		this.GetComponent<RawImage>().enabled = false;
		foreach( GameObject loadingCube in loadingCubeList )
		{
			loadingCube.SetActive(false);
		}
	}

	public void Reset()
	{
		IsLoadingFinished = false;
		foreach( GameObject loadingCube in loadingCubeList )
		{
			loadingCube.SetActive(false);
		}
		LoadingDescText.gameObject.SetActive(true);
		this.GetComponent<RawImage>().enabled = true;
		m_index = 0;
		m_timer = 0.0f;
		m_loadingGapTime = LoadingTime / loadingCubeList.Count;
	}
}
