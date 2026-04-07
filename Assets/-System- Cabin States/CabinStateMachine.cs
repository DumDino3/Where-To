using System;

public class CabinStateMachine: StateMachine<CabinStateMachine.State>
{
    public static event Action<State> OnCabinStateChanged;
    
    public enum State
    {
        Picked,
        Idling,
        Dropped,
    }

    void Awake()
    {
        States.Add(State.Picked, new PickUpState());
        States.Add(State.Idling, new IdleState());
        States.Add(State.Dropped, new DropOffState());
        CurrentState = States[State.Idling];
    }

    public void SetPickUp()
    {
        QueueNextState(State.Picked);
        OnCabinStateChanged?.Invoke(State.Picked);
    }

    public void SetDropOff()
    {
        QueueNextState(State.Dropped);
        OnCabinStateChanged?.Invoke(State.Dropped);
    }

    public void SetIdle()
    {
        QueueNextState(State.Idling);
        OnCabinStateChanged?.Invoke(State.Idling);
    }
}
