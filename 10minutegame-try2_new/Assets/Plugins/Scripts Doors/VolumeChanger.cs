using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeChanger : MonoBehaviour
{
    
	public AudioMixer mixer;
	
	
	public void SetVolume(float value)
	{
		
		mixer.SetFloat("MasterVolume", value);
		
	}
	
	
}
