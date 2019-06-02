using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOffset : MonoBehaviour
{
    private void Start()
    {
        gameObject.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        Animator anim = GetComponent<Animator>();
         AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);//could replace 0 by any other animation layer index
         anim.Play(state.fullPathHash, -1, Random.Range(0.5f, 0.8f));
    }
}
   /* void Update()
    {
        Animator anim = GetComponent<Animator>();
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);//could replace 0 by any other animation layer index
        anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
    }
}*/
