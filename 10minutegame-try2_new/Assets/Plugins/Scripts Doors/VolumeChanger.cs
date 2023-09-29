using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeChanger : MonoBehaviour
{
    
	public AudioMixer mixer;

    public bool isButtons;
    public Button ButtonOff;
    public Button ButtonOn;

    void Start()
    {
        float v = PlayerPrefs.GetFloat("MasterVolume");
        SetVolume(v);

        if (isButtons)
        {
            if (v == 0)
                ButtonOn.onClick.Invoke();
            else
                ButtonOff.onClick.Invoke();
        }

    }
	
	public void SetVolume(float value)
	{
		
		mixer.SetFloat("MasterVolume", value);

        PlayerPrefs.SetFloat("MasterVolume", value);

    }
	
	
}
