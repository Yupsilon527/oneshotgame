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

        };
        StartCoroutine(PlayRandomAnimation());
    }
    IEnumerator PlayRandomAnimation()
    {
        while (true)
        {
            int randomIndex = Random.Range(0, _animationHashes.Length);
            int selectedAnimationHash = _animationHashes[randomIndex];

            _npcAnimator.Play(selectedAnimationHash);

            float waitTime = Random.Range(_minimumWaitTime, _maximumWaitTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void NpcCough()
    { 
        StopCoroutine(PlayRandomAnimation());
        if(_npcAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash != Animator.StringToHash("Base Layer.NPC_Cough_0"))
        {
            _npcAnimator.Play(Animator.StringToHash("Base Layer.NPC_Cough_0"));
        }
    }
}
