using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

    [SerializeField] Slider slider;
    [SerializeField] AudioMixer mixer;

    void Start()
    {
        SetVolume();
    }

    public void SetVolume()
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(slider.value) * 20);
    }


}
