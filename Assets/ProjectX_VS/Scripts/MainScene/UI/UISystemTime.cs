using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UISystemTime : MonoBehaviour {

	public Text timeText;
	public Text timeText2;

	void Start () 
	{

	}

	void Update () 
	{
		int day = System.DateTime.Now.Day;
		int month = System.DateTime.Now.Month;
		int year = System.DateTime.Now.Year;
		int hour = System.DateTime.Now.Hour;
		int minute = System.DateTime.Now.Minute;

		string localDate2 = year.ToString() + "/" + month.ToString() + "/" + day.ToString();
		string localDate = hour.ToString() + ":" + minute.ToString();

		timeText.text = localDate.ToString();
		timeText2.text = localDate2.ToString();
	}
}
