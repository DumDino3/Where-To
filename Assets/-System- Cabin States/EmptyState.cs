using System.Collections;
using UnityEngine;

public class EmptyState: BaseState<CabinStateMachine.CabinStates>
{
    public EmptyState() : base(CabinStateMachine.CabinStates.Empty)
    {
    }

    public override void EnterState()
    {
        
    }
    public override void ExitState()
    {
        
    }
    public override void UpdateState()
    {

    }
}
