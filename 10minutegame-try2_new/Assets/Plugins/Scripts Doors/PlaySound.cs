using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{

    public AudioSource source;
    public AudioClip[] sound;

    public void PlayClip()
    {
        source.PlayOneShot(sound[Random.Range(0, sound.Length)], 0.5F);
    }

}
