using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectManager {

    static public Animator getAnimator(GameObject _Target)
    {
        Animator TargetAnimator = _Target.GetComponent<Animator>();
        return TargetAnimator;
    }

    static public AnimatorStateInfo getAnimatorStateInfo(GameObject _Target)
    {
        AnimatorStateInfo TargetState = getAnimator(_Target).GetCurrentAnimatorStateInfo(0);
        return TargetState;
    }
}
