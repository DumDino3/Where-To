using System;
using NUnit.Framework.Constraints;
using UnityEditor.Rendering;
using UnityEngine;

public class CabinStateMachine: StateMachine<CabinStateMachine.CabinStates>
{
    
    public static event Action<CabinStates> OnCabinStateChanged;
    
    public enum CabinStates
    {
        Picked,
        Idling,
        Dropped,
    }

    void Awake()
    {
        States.Add(CabinStates.Picked, new PickUpState());
        States.Add(CabinStates.Idling, new IdleState());
        States.Add(CabinStates.Dropped, new DropOffState());
        CurrentState = States[CabinStates.Idling];
    }

    public void SetPickUp()
    {
        QueueNextState(CabinStates.Picked);
        OnCabinStateChanged?.Invoke(CabinStates.Picked);
    }

    public void SetDropOff()
    {
        QueueNextState(CabinStates.Dropped);
        OnCabinStateChanged?.Invoke(CabinStates.Dropped);
    }
    
    public void SetIdle()
    {
        QueueNextState(CabinStates.Idling);
        OnCabinStateChanged?.Invoke(CabinStates.Idling);
    }
}
