using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MotionValues;

namespace MotionValues
{
    [System.Serializable]
    public class MotionTiming
    {
        public float[] MotionPeriods = new float[10];

        public MotionTiming()
        {
            if (MotionPeriods != null) return;
            //WARNING: i<10 可変
            for (int i = 0; i < 10; i++)
            {
                MotionPeriods[i] = 0;
            }
        }

        public void SetMotionTiming(MotionTiming _Target, MotionTiming _Source)
        {
            for (int i = 0; i < 10; i++)
            {
                _Target.MotionPeriods[i] = _Source.MotionPeriods[i];
            }
        }
    }

}

public class MotionManager : SingleTon<MotionManager>
{

    [SerializeField]
    MotionTiming PlayerAttack;
    //      Default Setting
    //  Period[0] = 0.2f
    //  Period[1] = 0.3f
    //  Period[2] = 0.3f
    //  Period[3] = 0.2f
    //  Period[4] = 0.3f
    //  Period[5] = 0.1f
    //  Period[6] = 0.4f

    [SerializeField]
    MotionTiming PlayerCounterAttack;
    //      Default Setting
    //  Period[0] = 0.3f
    //  Period[1] = 0.35f
    //  Period[2] = 0.22f
    //  Period[3] = 0.18f
    //  Period[4] = 0.35f

    [SerializeField]
    MotionTiming EnemyAttack;
    //      Default Setting
    //  Period[0] = 0.35f
    //  Period[1] = 0.02f
    //  Period[2] = 0.73f

    [SerializeField]
    MotionTiming EnemyAttack2;
    //      Default Setting
    //  Period[0] = 0.40f
    //  Period[1] = 0.05f
    //  Period[2] = 0.75f
    //  Period[3] = 0.85f

    [SerializeField]
    MotionTiming BossAttack;
    //      Default Setting
    //  Period[0] = 0.3f
    //  Period[1] = 0.2f
    //  Period[2] = 0.5f
    //  Period[3] = 1f

    [SerializeField]
    MotionTiming BossAttack2;
    //      Default Setting
    //  Period[0] = 1f
    //  Period[1] = 0.02f
    //  Period[2] = 1f
    //  Period[3] = 1f
    //  Period[4] = 0.02f
    //  Period[5] = 1f

    [SerializeField]
    MotionTiming BossDamageReact;
    //      Default Setting
    //  Period[0] = 1f
    //  Period[1] = 2f

    Dictionary<string, MotionTiming> dictionary = new Dictionary<string, MotionTiming>();

    private void Awake()
    {
        dictionary.Add("PlayerAttack", PlayerAttack);
        dictionary.Add("PlayerCounterAttack", PlayerCounterAttack);
        dictionary.Add("EnemyAttack", EnemyAttack);
        dictionary.Add("EnemyAttack2", EnemyAttack2);
        dictionary.Add("BossAttack", BossAttack);
        dictionary.Add("BossAttack2", BossAttack2);
        dictionary.Add("BossDamageReact", BossDamageReact);
    }


    public MotionTiming GetMotionValue(string _TargetTag)
    {
        // TODO タグが存在しない！ => エラーにする
        return dictionary[_TargetTag];
    }
}
