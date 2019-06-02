using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeAnimationOffset : StateMachineBehaviour
{


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);//could replace 0 by any other animation layer index
        animator.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
    }
  
    
        }