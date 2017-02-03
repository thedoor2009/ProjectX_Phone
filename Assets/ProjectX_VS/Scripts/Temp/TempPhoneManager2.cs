using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TempPhoneManager2 : MonoBehaviour {

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
		{ "C_07", new List<float>{ 7.0f, 16.5f } }
	};

	private Dictionary<string, List<string>> SuspectSequenceAudioDictionary = new Dictionary<string, List<string>>
	{
		{ "A", new List<string>{ "W_N_01","W_N_01","W_N_02","W_N_03" }},
		{ "B", new List<string>{ "W_N_01","W_N_01","W_N_04","W_N_05" }}
	};

	private Dictionary<string, List<float>> SuspectSequenceTimeDictionary = new Dictionary<string, List<float>>
	{
		{ "A", new List<float>{ 4.5f, 4.0f, 7.0f, 5.0f}},
		{ "B", new List<float>{ 4.5f, 4.0f, 7.0f, 5.0f}}
	};

	private Dictionary<string, string> AnswerChoiceDictionary = new Dictionary<string, string>()
	{
		{"Z_01_0", "ask_not_follow"},
		{"Z_02_0", "ask_30_sec"},
		{"Z_03_0", "ask_not_follow"},
		{"Z_04_0", "ask_not_follow"},
		{"Z_05_0", "no_excuse"},
		{"Z_06_0", "no_excuse"},
		{"Z_07_0", "you_mad"},
		{"Z_08_0", "no_edu"},
		{"Z_09_0", "no_edu"},
		{"Z_10_0", "good"},
		{"Z_11_0", "good"},
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
				m_play_once = false;
				m_enable_choices = false;
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
	private SuspectAudioSequence    seq_2;
	private SuspectAudioSequence    m_seq;

	private string 					m_cur_state = "";
	private string					m_choice = "";
	private float					m_normal_timer = 0.0f;
	private float					m_interrupt_time = 0.0f;
	private float 					m_suspect_decrease_time;
	private float					m_wait_timer = 0.0f;
	private int						m_retry_round = 0;
	private int						m_wait_point = 0;
	private bool					m_play_once = false;
	private bool					m_enable_choices = false;
	private bool					m_wait_flag = false;

	void Start () 
	{
		Init();
	}

	void Update ()
	{
		switch(CurState)
		{
		case "ask_01":
			if( PlayAudioFirstTime("W_01") )
			{
				return;
			}

			if( AudioDataManagerObject.audioSource.isPlaying || AudioSource.isPlaying )
			{
				return;
			}

			if( !m_enable_choices )
			{
				m_enable_choices = true;
				m_insertAudios.Clear();
				m_insertAudios.Add("Z_01");
				m_insertAudios.Add("Z_02");
				RefreshAudioDataManager();
			}

			if( CheckAudioChoice() )
				return;	
			else
				CheckNextState( CurState );

			break;
		case "ask_not_follow":
			if( PlayAudioFirstTime("W_02") )
			{
				return;
			}

			if( AudioDataManagerObject.audioSource.isPlaying || AudioSource.isPlaying ){
				return;
			}

			if( !m_enable_choices )
			{
				m_enable_choices = true;
				m_insertAudios.Clear();
				m_insertAudios.Add("Z_05");
				m_insertAudios.Add("Z_06");
				m_insertAudios.Add("Z_07");
				m_insertAudios.Add("Z_08");
				m_insertAudios.Add("Z_09");
				RefreshAudioDataManager();
			}

			if( CheckAudioChoice() )
				return;	
			else
				CheckNextState( CurState );

			break;
		case "ask_no_wait":
			if( PlayAudioFirstTime("W_04") )
			{
				return;
			}

			if( AudioDataManagerObject.audioSource.isPlaying || AudioSource.isPlaying ) return;

			if( CheckAudioChoice() )
				return;	
			else
				CheckNextState( CurState );
			break;

		case "ask_30_sec":
			if( PlayAudioFirstTime("W_03") )
			{
				return;
			}

			if( AudioDataManagerObject.audioSource.isPlaying || AudioSource.isPlaying ) return;

			if( !m_enable_choices )
			{
				m_enable_choices = true;
				m_insertAudios.Clear();
				m_insertAudios.Add("Z_03");
				m_insertAudios.Add("Z_04");
				RefreshAudioDataManager();
			}

			if( CheckAudioChoice() )
				return;	
			else
				CheckNextState( CurState );

			break;

		case "no_excuse":
			m_seq = seq_2;
			if( PlayAudioFirstTime("W_06") ) return;
			if( AudioDataManagerObject.audioSource.isPlaying || AudioSource.isPlaying ) return;
			if( CheckAudioChoice() )
				return;	
			else
				CheckNextState( CurState );

			break;

		case "you_mad":
			m_seq = seq_2;
			if( PlayAudioFirstTime("W_07") ) return;
			if( AudioDataManagerObject.audioSource.isPlaying || AudioSource.isPlaying ) return;
			if( CheckAudioChoice() )
				return;	
			else
				CheckNextState( CurState );
			break;

			case "no_edu":
			m_seq = seq_2;
			if( PlayAudioFirstTime("W_08") ) return;
			if( AudioDataManagerObject.audioSource.isPlaying || AudioSource.isPlaying ) return;
			if( CheckAudioChoice() )
				return;	
			else
				CheckNextState( CurState );
			break;

		case "make_up":
			if( PlayAudioFirstTime("W_09") )
			{
				return;
			}
			if( AudioDataManagerObject.audioSource.isPlaying || AudioSource.isPlaying )
			{
				return;
			}

			if( !m_enable_choices )
			{
				m_enable_choices = true;
				m_insertAudios.Clear();
				m_insertAudios.Add("Z_10");
				m_insertAudios.Add("Z_11");
				RefreshAudioDataManager();
			}

			if( CheckAudioChoice() )
				return;
			else
				CheckNextState( CurState );
			break;
		case "good":
			if( PlayAudioFirstTime("W_10") ) return;
			break;

		case "suepect_sequence_update":
			UpdateSuspectAudioSequence();
			break;

		case "Answer_1":
			if( AudioDataManagerObject.audioSource.isPlaying || AudioSource.isPlaying )
			{
				return;
			}
			CurState = AnswerChoiceDictionary[m_choice];
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
		seq_2 = CreateSuspectAudioSeqByName("B");
		m_seq = seq_1;

		CurState = "ask_01";
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

		Debug.Log(m_wait_timer);
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
		if(CheckWaitFlag())
		{
			SuspectValue.text = m_suspect_decrease_time.ToString();
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
		string next_state = ReadJson.GetStateDataByFiled(stateName, "next_state").ToString();
		double wait_time = (double)ReadJson.GetStateDataByFiled(stateName, "wait_time");
		int retry_round = (int)ReadJson.GetStateDataByFiled(stateName, "retry_round");
		bool start_suspect = (bool)ReadJson.GetStateDataByFiled(stateName, "start_suspect");

		CheckAudioWaitTimer( next_state, (float)wait_time, retry_round, start_suspect );
	}

	public void CheckAudioWaitTimer( string nextState, float time, int retryRound = 0, bool toSuspectSeq = false )
	{
		NormalWaitTime = time;
		m_normal_timer += Time.deltaTime;
		DebugTimer.text = ( NormalWaitTime - m_normal_timer ).ToString();

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
