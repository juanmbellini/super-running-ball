using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{

	static bool m_Play = true;
	static AudioSource m_MyAudioSource;
	bool m_ToggleChange = true;



// Use this for initialization
	void Start () {
		Debug.Log("ENTRA");
		//Fetch the AudioSource from the GameObject
		m_MyAudioSource = GetComponent<AudioSource>();
		//Ensure the toggle is set to true for the music to play at start-up
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log(m_Play);
		Debug.Log(m_ToggleChange);
		//Check to see if you just set the toggle to positive
		if (m_Play == true && m_ToggleChange == true)
		{
			//Play the audio you attach to the AudioSource component
			m_MyAudioSource.Play();
			//Ensure audio doesn’t play more than once
			m_ToggleChange = false;
		}
		//Check if you just set the toggle to false
		if (m_Play == false && m_ToggleChange == true)
		{
			//Stop the audio
			m_MyAudioSource.Stop();
			//Ensure audio doesn’t play more than once
			m_ToggleChange = false;
		}
	}

	public static void stopMusic()
	{
		m_Play = false;
		m_MyAudioSource.Stop();
	}
}
	