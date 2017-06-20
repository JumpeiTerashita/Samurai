using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// アニメーション終了感知
/// Observerパターン
/// </summary>
public class StateMachineObserver : StateMachineBehaviour
{

    public Action<Animator, AnimatorStateInfo,int> onStateExit;
    public Action onIdleExit;
    public Action onDamageExit;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        //{
        //    onIdleExit();
        //}

        if (onStateExit != null)
        {
            onStateExit(animator, stateInfo,layerIndex);
        }
    }
}
