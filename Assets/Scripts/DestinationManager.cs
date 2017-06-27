using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationManager : MonoBehaviour
{
    
    Vector3 Destination;

    [SerializeField]
    Vector3 TargetPos;

    // Use this for initialization
    void Start()
    {
        
        Destination = TargetPos;
    }

    
   public void SetDestination()
    {
        Destination = TargetPos;
    }

    public void SetDestination(Vector3 _TargetPos)
    {
        TargetPos = _TargetPos;
        Destination = TargetPos;
    }

    
    public Vector3 GetDestination()
    {
        return Destination;
    }
}
