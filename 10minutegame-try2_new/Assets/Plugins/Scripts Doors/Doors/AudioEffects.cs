using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEffects : MonoBehaviour
{
    [Header("Settings")]
    public AudioSource audioSource;

    [Header("Music")]
    public AudioClip music;

    [Header("End Sound")]
    public AudioClip endSound;
    
    [Header("Win")]
    public AudioClip win;

    [Header("Beat")]
    public AudioClip beat;

    [Header("Pop")]
    public AudioClip pop;

    [Header("Door")]
    public AudioClip doorOpen;

    [Header("Coin")]
    public AudioClip[] coin;

    [Header("Key")]
    public AudioClip key;

    [Header("Closet")]
    public AudioClip closetOpen;
    public AudioClip closetClose;

    [Header("Drawer")]
    public AudioClip[] drawerOpen;
    public AudioClip[] drawerClose;

    [Header("Kill")]
    public AudioClip kill;

    float pitchValue;
    float lastTime;

    public void PlaySound(AudioClip clip)
    {
        audioSource.pitch = 1;
        audioSource.PlayOneShot(clip);
    }

    public void PlaySound(AudioClip[] clip)
    {
        audioSource.pitch = 1;
        audioSource.PlayOneShot(clip[Random.Range(0, clip.Length)]);
    }

    public void PlaySound(AudioSource source, AudioClip clip)
    {
        audioSource.pitch = 1;
        source.PlayOneShot(clip);
    }

    public void PlaySound(AudioSource source, AudioClip[] clip)
    {
        audioSource.pitch = 1;
        source.PlayOneShot(clip[Random.Range(0, clip.Length)]);
    }

    public void PlaySoundPitch(AudioClip clip, bool resetPitch)
    {
        if (Time.time > lastTime + 0.05f)
            lastTime = Time.time;
        else
            return;

        if (resetPitch || pitchValue > 1.8f || pitchValue < 1)
            pitchValue = 1;
        else
            pitchValue += 0.1f;
        audioSource.pitch = pitchValue;
        audioSource.PlayOneShot(clip);
    }
}
