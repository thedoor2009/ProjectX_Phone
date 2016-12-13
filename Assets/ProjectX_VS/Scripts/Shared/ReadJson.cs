using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;

public class ReadJson : MonoBehaviour {

	public JsonData stateData;

	void Start ()
	{
		string jsonString = File.ReadAllText( Application.dataPath + "/Resources/json/states.json");
		stateData = JsonMapper.ToObject( jsonString );
	}

	void Update ()
	{
	
	}

	public JsonData GetStateDataByFiled( string state, string filed )
	{
		return stateData[state][0][filed];
	}
}
