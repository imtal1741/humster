using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepTrigger : MonoBehaviour
{

    public AudioSource audioSource;

    [Range(0f, 1f)]
    public float audioClipVolume = 0.1f;
    public float relativeRandomizedVolumeRange = 0.2f;
    public AudioClip[] footStepClips;

    public List<string> ignoreTags = new List<string>();

    void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < ignoreTags.Count; i++)
        {
            if (other.CompareTag(ignoreTags[i]))
                return;
        }

        PlayFootstepSound();
    }

    void PlayFootstepSound()
    {
        int _footStepClipIndex = Random.Range(0, footStepClips.Length);
        audioSource.PlayOneShot(footStepClips[_footStepClipIndex], audioClipVolume + audioClipVolume * Random.Range(-relativeRandomizedVolumeRange, relativeRandomizedVolumeRange));
    }

}
