using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHelpers : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _eatingSFX;
    public void StopEating()
    {
       PlayerController.main._playerAnimator.SetBool("IsEating", false);
    }
    public void PlayEatingSFX()
    {
            Debug.Log("K pasa!!");
        if (!_audioSource.isPlaying)
        {
            _audioSource.PlayOneShot(_eatingSFX);
        }
    }
}
