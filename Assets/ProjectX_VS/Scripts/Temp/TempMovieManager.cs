using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TempMovieManager : MonoBehaviour {

	public MovieController 		Movie;
	public RawImage 			RawImage;

	void Start () 
	{
	
	}

	void Update () 
	{
		RawImage.texture = Movie.ControllerMaterial.mainTexture;
	}
}
