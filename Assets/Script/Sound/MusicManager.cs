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
    Coroutine musicC;
    public void StopMusic()
    {

        if (musicC != null)
        {
            StopCoroutine(musicC);
        }
    }
    public void ChangeMusic(AudioClip clip, float dur = 1)
    {
        StopMusic();
        musicC = StartCoroutine(FadeMusic(clip, dur));
    }
    IEnumerator FadeMusic(AudioClip nMusic, float dur = 1)
    {
        float volume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime * .5f / dur;
            yield return null;
        }
        audioSource.clip = nMusic;
        audioSource.Play();
        while (audioSource.volume < volume)
        {
            audioSource.volume += Time.deltaTime * .5f / dur;
            yield return null;
        }
        audioSource.volume = volume;
    }
}
