using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackSMB : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.transform.GetComponent<PlayerController>().ShowWeapon();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.transform.GetComponent<PlayerController>().HideWeapon();
    }
}
