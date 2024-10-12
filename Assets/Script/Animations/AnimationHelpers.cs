using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHelpers : MonoBehaviour
{
    public void StopEating()
    {
       PlayerController.main._playerAnimator.SetBool("IsEating", false);
    }
}
