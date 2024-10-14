using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimationRandomiser : MonoBehaviour
{
    [SerializeField] private Animator _npcAnimator;
    [SerializeField] private float _minimumWaitTime = 0.3f;
    [SerializeField] private float _maximumWaitTime = 1f;

    private int[] _animationHashes;
    private void Start()
    {
        _animationHashes = new int[]
        {
            Animator.StringToHash("Base Layer.NPC_HeadScratch_0"),
            Animator.StringToHash("Base Layer.NPC_Tired_0"),
            // Add other states here
        };
        StartCoroutine(PlayRandomAnimation());
    }
    IEnumerator PlayRandomAnimation()
    {
        while (true)
        {
            // Randomly choose an animation state
            int randomIndex = Random.Range(0, _animationHashes.Length);
            int selectedAnimationHash = _animationHashes[randomIndex];

            // Play the randomly selected animation
            _npcAnimator.Play(selectedAnimationHash);

            // Wait for a random time before playing the next animation
            float waitTime = Random.Range(_minimumWaitTime, _maximumWaitTime);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
