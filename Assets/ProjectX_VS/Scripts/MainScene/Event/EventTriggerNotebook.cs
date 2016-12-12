using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EventTriggerNotebook : EventTrigger {

	public Scrollbar scrollBar;
	
	void Start () 
	{
		scrollBar.value = 1;
	}

	void Update () 
	{
		if( scrollBar.value < 0.1f && !isEventTriggered)
		{
			NotebookFinishReading();
		}
	}

	void NotebookFinishReading()
	{
		isEventTriggered = true;
		EventManager.TriggerEvent ("notebookFinishReading");
	}
}
