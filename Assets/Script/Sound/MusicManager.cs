using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }
    public void ChangeMusic(AudioClip clip) { 
    StartCoroutine(FadeMusic(clip));
}
    IEnumerator FadeMusic (AudioClip nMusic, float dur = 1)
    {
        float volume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime * .5f * dur;
            yield return null;
        }
        audioSource.clip = nMusic;
        audioSource.Play();
        while (audioSource.volume < volume)
        {
            audioSource.volume += Time.deltaTime * .5f * dur;
            yield return null;
        }
        audioSource.volume = volume;
    }
}
