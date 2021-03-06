﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TempPhoneManager : MonoBehaviour {
	public class AudioUnit
	{
		public AudioClip 		audio;
		public bool 			is_interrupted;
		public List<float> 		interrupt_point_list;
	}

	public class SuspectAudioUnit
	{
		public AudioUnit 	suspect_audio;
		public float 		suspect_time;

		public SuspectAudioUnit()
		{
			suspect_audio = new AudioUnit();
			suspect_time = 0.0f;
		}

		public SuspectAudioUnit(AudioUnit audio, float time )
		{
			if( audio == null )
			{
				suspect_audio = new AudioUnit();
			}
			suspect_time = time;
		}
	}

	public class SuspectAudioSequence
	{
		public List<SuspectAudioUnit> 		suspect_audio_list;//Store all suspect audio unit
		public float 						suspect_timer = 0.0f;
		public int   						cur_suspect_index = 0;//0 1 2 3

		public SuspectAudioSequence( )
		{
			suspect_audio_list = new List<SuspectAudioUnit>();
			suspect_timer = 0.0f;
			cur_suspect_index = 0;
		}

		public SuspectAudioSequence( List<SuspectAudioUnit> audio_list, float timer, int index )
		{
			if( audio_list == null )
			{
				suspect_audio_list = new List<SuspectAudioUnit>();
			}
			suspect_timer = timer;
			cur_suspect_index = index;
		}

		public void Reset()
		{
			suspect_timer = 0.0f;
			cur_suspect_index = 0;
		}
	}

	//All audios that could be interrupted should put into this dictionary
	private Dictionary<string, List<float>> InterruptAudioUnitDictionary = new Dictionary<string, List<float>>
	{
		{ "C_07", new List<float>{ 16.0f, 22.5f, 30.0f, 40.0f, 59.0f } }
	};

	private Dictionary<string, List<float>> WaitAudioUnitDictionary = new Dictionary<string, List<float>>
	{
		{ "C_07", new List<float>{7.0f, 16.5f } }
	};

	private Dictionary<string, List<string>> SuspectSequenceAudioDictionary = new Dictionary<string, List<string>>
	{
		{ "A", new List<string>{ "C_S01","C_S01","C_S02","C_S03" }}
	};

	private Dictionary<string, List<float>> SuspectSequenceTimeDictionary = new Dictionary<string, List<float>>
	{
		{ "A", new List<float>{ 4.5f,4.0f, 3.0f, 2.0f}}
	};

	private Dictionary<string, string> AnswerChoiceDictionary = new Dictionary<string, string>()
	{
		{"Q_01_0", "detailed_explain"},
		{"Q_02_0", "water"},
		{"Q_01_1", "signal"},
		{"Q_02_1", "water"},
		{"Q_01_2", "almost_hangup"},
		{"Q_02_2", "hangup"},
	};

	public ReadJson					ReadJson;
	public AudioDataManager 		AudioDataManagerObject;
	public AudioSource				AudioSource;
	public Text						DebugTimer;
	public Text						SuspectValue;
	public float					NormalWaitTime = 3.0f;
	public float 					SuspectDecreaseTime = 10.0f;
	public float					SuspectRecoverValue = 5.0f;
	public int						RetryRound = 2;

	public string					CurState
	{
		set
		{
			if( value != m_cur_state )
			{
				m_enable_choices = false;
				m_play_once = false;
			}
			m_cur_state = value;
		}
		get
		{
			return m_cur_state;
		}
	}

	private List<AudioUnit> 		m_audio_unit_list;
	private List<string> 			m_insertAudios = new List<string>();
	private SuspectAudioSequence 	m_cur_suspect_seq;

	//TODO:Need to find another way.
	private SuspectAudioSequence    seq_1;

	private string 					m_cur_state = "";
	private string					m_choice = "";
	private float					m_normal_timer = 0.0f;
	private float					m_interrupt_time = 0.0f;
	private float 					m_suspect_decrease_time;
	private float					m_wait_timer = 0.0f;
	private int						m_retry_round = 0;
	private int						m_wait_point = 0;
	private bool					m_enable_choices = false;
	private bool					m_play_once = false;
	private bool					m_wait_flag = false;

	void Start () 
	{
		Init();
	}

	//Q_01 好了，我们开始吧
	//Q_02 稍等一下
	//Q_03 嗯
	//Q_04 好的
	//Q_05 你继续
	//Q_06 明白


	//C_01 准备好了吗 01
	//C_02 要准备好了我们就开始 02
	//C_03 我还以为信号断掉了 03
	//C_04 不急，好了告诉我，我去倒杯水 04
	//C_05 那我先去忙点别的，你准备好了给我打回来 05
	//C_06 我差点挂掉了 06
	//C_07 你们想要的报价大概是这样的 07

	//SequenceA
	//C_S01 喂？
	//C_S02 能听见吗
	//C_S03 咦，听不见了
	 
	void Update ()
	{
		switch(CurState)
		{
		case "first_ask":
			if( PlayAudioFirstTime("C_01") )
			{
				
				return;
			}

			if( AudioDataManagerObject.audioSource.isPlaying || AudioSource.isPlaying ) return;

			if( !m_enable_choices )
			{
				m_enable_choices = true;
				m_insertAudios.Clear();
				m_insertAudios.Add("Q_01");
				m_insertAudios.Add("Q_02");
				RefreshAudioDataManager();
			}

			if( CheckAudioChoice() )
				return;	
			else
				CheckNextState( CurState );
			
			break;

		case "first_ask_2":
			if( PlayAudioFirstTime("C_02") ) return;

			if( AudioDataManagerObject.audioSource.isPlaying ) return;

			if( CheckAudioChoice() )
				return;	
			else
				CheckNextState( CurState );
			break;



		case "signal":
			if( PlayAudioFirstTime("C_03") ) return;

			if( AudioDataManagerObject.audioSource.isPlaying ) return;
				
			if( CheckAudioChoice() ) 
				return;
			else
				CheckNextState( CurState );
			break;



		case "water":
			if( PlayAudioFirstTime("C_04") ) return;

			if( AudioDataManagerObject.audioSource.isPlaying ) return;

			if( CheckAudioChoice() ) 
				return;
			else
				CheckNextState( CurState );
			break;

		

		case "back_check":
			if( PlayAudioFirstTime("C_S01") )
			{
				return;
			}

			if( AudioDataManagerObject.audioSource.isPlaying || AudioSource.isPlaying ) return;

			if( !m_enable_choices )
			{
				m_enable_choices = true;
				m_insertAudios.Clear();
				m_insertAudios.Add("Q_01");
				m_insertAudios.Add("Q_02");
				RefreshAudioDataManager();
			}

			if( CheckAudioChoice() ) 
				return;
			else
				CheckNextState( CurState );
			break;



		case "almost_hangup":
			if( PlayAudioFirstTime("C_03") ) return;

			if( AudioDataManagerObject.audioSource.isPlaying ) return;

			if( CheckAudioChoice() ) 
				return;
			else
				CheckNextState( CurState );
			break;

		case "hangup":
			if( PlayAudioFirstTime("C_05") ) return;

			CurState = "end";
			break;


		case "detailed_explain":
			if( PlayAudioFirstTime("C_07") )
			{
				m_insertAudios.Clear();
				m_insertAudios.Add("Q_03");
				m_insertAudios.Add("Q_04");
				m_insertAudios.Add("Q_05");
				m_insertAudios.Add("Q_06");
				RefreshAudioDataManager();

				InitSuspectValue();

				return;
			}

			CheckSuspectValue();

			break;

		//Special (Reused) Case
		case "Answer_1":
			if( AudioDataManagerObject.audioSource.isPlaying || AudioSource.isPlaying )
			{
				return;
			}
			CurState = AnswerChoiceDictionary[m_choice];
			break;
		case "suepect_sequence_update":
			UpdateSuspectAudioSequence();
			break;
		case "Interrupt":
			if( ReachCloestInterruptPoint() )
			{
				m_suspect_decrease_time = 0.0f;

				if( m_retry_round < RetryRound )
				{
					m_retry_round++;
				}
				m_insertAudios.Clear();
				m_insertAudios.Add("Q_01");
				m_insertAudios.Add("Q_02");
				RefreshAudioDataManager();

				InitSuspectAudioSequence( seq_1 );
				CurState = "suepect_sequence_update";
			}
			break;
		case "end":
			break;
		default:
			break;
		}
	}

	private void Init()
	{
		m_suspect_decrease_time = SuspectDecreaseTime;

		m_audio_unit_list = new List<AudioUnit>();

		List<AudioClip> AudioClipList = this.GetComponent<AudioFilesReader>().AudioClipList;
		int length = AudioClipList.Count;
		for( int index = 0; index < length; index++ )
		{
			AudioUnit audioUnit = new AudioUnit();

			audioUnit.audio = AudioClipList[index];
			audioUnit.is_interrupted = false;

			if( InterruptAudioUnitDictionary.ContainsKey( audioUnit.audio.name ) )
			{
				audioUnit.is_interrupted = true;
				audioUnit.interrupt_point_list = InterruptAudioUnitDictionary[audioUnit.audio.name];//Dictionary[key] = content
			}

			m_audio_unit_list.Add(audioUnit);
		}


		//Create suspect sequence
		seq_1 = CreateSuspectAudioSeqByName("A");

		CurState = "first_ask";
	}

	private bool PlayAudioFirstTime( string audioName )
	{
		if( !m_play_once )
		{
			AudioSource.clip = FindAudioClipByName( audioName );
			AudioSource.Play();
			m_play_once = true;

			return true;
		}	
		return false;
	}

	private void InitSuspectValue()
	{
		m_suspect_decrease_time = SuspectDecreaseTime;
	}

	private bool CheckWaitFlag()
	{
		if(m_wait_flag) return true;

		//Debug.Log(m_wait_timer);
		m_wait_timer += Time.deltaTime;

		List<float> wait_points = WaitAudioUnitDictionary[AudioSource.clip.name];
		if( m_wait_point >= wait_points.Count ) return false;

		for( int index = m_wait_point; index < wait_points.Count; index++ )
		{
			if( m_wait_timer - wait_points[index] < 3.0f && m_wait_timer - wait_points[index] > 0.0f )
			{
				m_wait_point++;
				m_wait_flag = true;
				AudioSource.Pause();
				StartCoroutine(AudioWait(1.0f));
				break;
			}
		}

		return false;
	}

	IEnumerator AudioWait( float time )
	{
		yield return new WaitForSeconds( time );
		m_wait_flag = false;
		AudioSource.UnPause();
	}

	private void CheckSuspectValue()
	{
        print(m_suspect_decrease_time);
        if (CheckWaitFlag())
		{
			//SuspectValue.text = m_suspect_decrease_time.ToString();
			m_suspect_decrease_time -= Time.deltaTime;
			return;
		}

		if( AudioDataManagerObject.AudioChoiceName != string.Empty )
		{
			AudioDataManagerObject.AudioChoiceName = string.Empty;
			m_suspect_decrease_time += SuspectRecoverValue;

			StartCoroutine( RefreshAnswerOption(1.0f) );
		}

		if( m_suspect_decrease_time <= 0.0f )
		{
			List<float> interrupt_points = InterruptAudioUnitDictionary[AudioSource.clip.name];
			float cur_audio_time = AudioSource.time;
			float min = 0.0f;
			int interrupt_point_index = 0;

			for( int index = 0; index < interrupt_points.Count; index++ )
			{
				if( interrupt_points[index] - cur_audio_time <= 0 )
				{
					continue;
				}

				float temp = interrupt_points[index] - cur_audio_time;
				if( temp < min )
				{
					interrupt_point_index = index;
					min = temp;
				}
			}
			m_interrupt_time = interrupt_points[interrupt_point_index];
			CurState = "Interrupt";
		}
	}

	private bool ReachCloestInterruptPoint()
	{
		return AudioSource.time > m_interrupt_time;
	}

	IEnumerator RefreshAnswerOption( float wait_time )
	{
		yield return new WaitForSeconds( wait_time );

		m_insertAudios.Clear();
		m_insertAudios.Add("Q_03");
		m_insertAudios.Add("Q_04");
		m_insertAudios.Add("Q_05");
		m_insertAudios.Add("Q_06");
		RefreshAudioDataManager();
	}

	private void InitSuspectAudioSequence( SuspectAudioSequence seq )
	{
		m_cur_suspect_seq = seq;
		m_cur_suspect_seq.suspect_timer = 0.0f;
		m_cur_suspect_seq.cur_suspect_index = 0;

		AudioSource.clip = m_cur_suspect_seq.suspect_audio_list[m_cur_suspect_seq.cur_suspect_index].suspect_audio.audio;
		AudioSource.Play();
		m_cur_suspect_seq.cur_suspect_index++;
	}

	private void UpdateSuspectAudioSequence()
	{
		m_cur_suspect_seq.suspect_timer += Time.deltaTime;
		int curIndex = m_cur_suspect_seq.cur_suspect_index;
		SuspectAudioUnit curSuspect = m_cur_suspect_seq.suspect_audio_list[curIndex];

		if( CheckAudioChoice() )
		{
			m_cur_suspect_seq.Reset();
			return;
		}
		else if( m_cur_suspect_seq.suspect_timer > curSuspect.suspect_time )
		{
			AudioSource.clip = m_cur_suspect_seq.suspect_audio_list[curIndex].suspect_audio.audio;
			AudioSource.Play();

			if( curIndex + 1 < 	m_cur_suspect_seq.suspect_audio_list.Count )
			{
				m_cur_suspect_seq.cur_suspect_index++;
				m_cur_suspect_seq.suspect_timer = 0.0f;
			}
			else
			{
				CurState = "end";
				//Play call end audio
			}
		}
	}

	private AudioClip FindAudioClipByName( string audio_name )
	{
		AudioClip audio_data = null;
		if( audio_name == null || audio_name == string.Empty )
		{
			return audio_data;
		}

		int length = m_audio_unit_list.Count;
		for( int index = 0; index < length; index++ )
		{
			if( m_audio_unit_list[index].audio.name == audio_name )
			{
				audio_data = m_audio_unit_list[index].audio;
				break;
			}
		}

		return audio_data;
	}

	private SuspectAudioSequence CreateSuspectAudioSeqByName( string seq_name )
	{
		if( seq_name == null || seq_name == string.Empty )
		{
			return null;
		}

		if( !SuspectSequenceAudioDictionary.ContainsKey(seq_name) || !SuspectSequenceTimeDictionary.ContainsKey(seq_name))
		{
			return null;
		}

		SuspectAudioSequence seq = new SuspectAudioSequence();
		List<string> audio_list = SuspectSequenceAudioDictionary[seq_name];
		List<float> audio_time_list = SuspectSequenceTimeDictionary[seq_name];

		if( audio_list.Count != audio_time_list.Count )
		{
			Debug.LogError(" Hey! The number of audio list data doesn't match the one of audio time list. Please Check.");
			return null;
		}

		int length = audio_list.Count;
		for( int index = 0; index < length; index++ )
		{
			float audio_time = audio_time_list[index];
			AudioClip audio_data = FindAudioClipByName( audio_list[index] );

			if( audio_data == null )
			{
				Debug.LogError("Please check audio " + audio_list[index] +" in Dictionary. The data is empty.");
				break;
			}

			SuspectAudioUnit audio_unit = new SuspectAudioUnit();
			audio_unit.suspect_audio.audio = audio_data;
			audio_unit.suspect_time = audio_time;

			seq.suspect_audio_list.Add(audio_unit);
		}

		return seq;
	}

	private void RefreshAudioDataManager()
	{
		AudioDataManagerObject.InsertAvailableAudio(m_insertAudios);
		AudioDataManagerObject.RefreshAvailableAudio(false);
	}

	public bool CheckAudioChoice()
	{
		if( AudioDataManagerObject.AudioChoiceName != string.Empty )
		{	
			string choice_name = AudioDataManagerObject.AudioChoiceName;
			choice_name += ( "_" + m_retry_round.ToString());
			m_choice = choice_name;
			CurState = "Answer_1";

			AudioDataManagerObject.AudioChoiceName = string.Empty;
			m_normal_timer = 0.0f;

			return true;
		}
		return false;
	}

	public void CheckNextState( string stateName )
	{
		string next_state = ReadJson.GetStateDataByFiled(stateName, 0, "next_state").ToString();
		double wait_time = (double)ReadJson.GetStateDataByFiled(stateName, 0, "wait_time");
		int retry_round = (int)ReadJson.GetStateDataByFiled(stateName, 0, "retry_round");
		bool start_suspect = (bool)ReadJson.GetStateDataByFiled(stateName, 0, "start_suspect");

		CheckAudioWaitTimer( next_state, (float)wait_time, retry_round, start_suspect );
	}

	public void CheckAudioWaitTimer( string nextState, float time, int retryRound = 0, bool toSuspectSeq = false )
	{
		NormalWaitTime = time;
		m_normal_timer += Time.deltaTime;
		//DebugTimer.text = ( NormalWaitTime - m_normal_timer ).ToString();

		if( m_normal_timer > NormalWaitTime )
		{
			
			m_normal_timer = 0.0f;

			if( toSuspectSeq )
			{
				m_retry_round = retryRound;
				InitSuspectAudioSequence( seq_1 );

				CurState = "suepect_sequence_update";
			}

			CurState = nextState;
		}
	}
}
