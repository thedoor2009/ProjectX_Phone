using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;
using System.Collections.Generic;

public class ReadJson : MonoBehaviour {

	private JsonData stateData;
	public int sceneNum;



    void Start ()
	{
        if ( sceneNum == 1 )
		{
			string jsonString = File.ReadAllText( Application.dataPath + "/Resources/json/states.json");
			stateData = JsonMapper.ToObject( jsonString );
            List<float> a = GetAudioUnitDictionaryByType("interrupt_audio_unit", "inturrput_time_sequence")["C_09"];
            List<float> b = GetAudioUnitDictionaryByType("wait_audio_unit", "wait_time_sequence")["C_02"];
            for (int index = 0; index < b.Count; index++)
            {
                print(b[index]);
            }
        }
		else if( sceneNum == 2 )
		{
			string jsonString = File.ReadAllText( Application.dataPath + "/Resources/json/states2.json");
			stateData = JsonMapper.ToObject( jsonString );
		}
	}


	public JsonData GetStateDataByFiled( string state, int index, string filed )
	{
		return stateData[state][index][filed];
	}


    public Dictionary<string, List<float>> GetAudioUnitDictionaryByType(string audioType, string timeSequence)
    {

        Dictionary<string, List<float>> InterrptAudioUnitDictionary = new Dictionary<string, List<float>>();
        JsonData audio_unit_array = stateData[audioType];
        print(audio_unit_array);
        for (int i = 0; i < audio_unit_array.Count; i ++)
        {
            string audio_name = GetStateDataByFiled(audioType, i, "audio_name").ToString();
            JsonData raw_interrupt_time_sequence = GetStateDataByFiled(audioType, i, timeSequence);
            print(raw_interrupt_time_sequence);
            List<float> interrupt_time_sequence = new List<float>();
            for (int j = 0; j < raw_interrupt_time_sequence.Count; j ++)
            {
                double time = (double)(raw_interrupt_time_sequence[j]);
                interrupt_time_sequence.Add((float)time);

            }
            InterrptAudioUnitDictionary.Add(audio_name, interrupt_time_sequence);
        }

        return InterrptAudioUnitDictionary;
    }

}
