using UnityEngine;
using System.Collections;

public class UILayoutAdjuster : MonoBehaviour {

	public RectTransform rect;

	public bool ShapeFixed = false;

	public bool AdjustScale = true;
	public bool AdjustPosition = true;

	private const int width = 1920;
	private const int height = 1080;

	void Start ()
	{
		AdjustLayout();
	}

	void Update ()
	{
	
	}

	private void AdjustLayout()
	{
		float w = (float)Screen.width;
		float h = (float)Screen.height;

		float factor_w = w / (float)width;
		float factor_h = h / (float)height;

		if(AdjustPosition)
		{
			Vector3 original_pos = rect.localPosition;
			rect.localPosition = new Vector3( original_pos.x * factor_w, original_pos.y * factor_h, original_pos.z );
		}

		if(AdjustScale)
		{
			Vector3 original_scale = rect.localScale;

			if(ShapeFixed)
			{
				rect.localScale = new Vector3( original_scale.x * factor_w, original_scale.y * factor_w, original_scale.z );
			}
			else
			{
				rect.localScale = new Vector3( original_scale.x * factor_w, original_scale.y * factor_h, original_scale.z );
			}
		}
	}
}
