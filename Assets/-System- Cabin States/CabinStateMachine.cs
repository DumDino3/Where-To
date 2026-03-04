using NUnit.Framework.Constraints;
using UnityEditor.Rendering;
using UnityEngine;

public class CabinStateMachine: StateMachine<CabinStateMachine.CabinStates>
{
    public enum CabinStates
    {
        Empty,
        Picked,
        Driving,
        Dropped,
    }

    void Awake()
    {
        States.Add(CabinStates.Empty, new EmptyState());
        CurrentState = States[CabinStates.Empty];
    }

    protected override void Update()
    {
        base.Update();
        //CabinSta
    }
}
