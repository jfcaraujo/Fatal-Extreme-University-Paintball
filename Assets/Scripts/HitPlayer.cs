﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPlayer : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<Player_Controller>().inputBlocked = true;
    }
}