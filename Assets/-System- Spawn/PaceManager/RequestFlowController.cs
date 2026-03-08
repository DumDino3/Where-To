using System;

public sealed class RequestFlowController
{
    public bool HasActiveQuest { get; private set; }
    public int PickupId { get; private set; }
    public int DropoffId { get; private set; }

    
    public bool TryActivate(int pickupId, int dropoffId)
    {
        if (HasActiveQuest) return false;

        PickupId = pickupId;
        DropoffId = dropoffId;
        HasActiveQuest = true;
        return true;
    }
    
    public void Reset()
    {
        HasActiveQuest = false;
        PickupId = 0;
        DropoffId = 0;
    }
    
    public FlowResult HandleCabinState(CabinStateMachine.CabinStates state)
    {
        if (!HasActiveQuest)
            return FlowResult.None;

        switch (state)
        {
            case CabinStateMachine.CabinStates.Idling:
                return FlowResult.Toggle(PickupId);

            case CabinStateMachine.CabinStates.Picked:
                return FlowResult.Toggle(DropoffId);

            case CabinStateMachine.CabinStates.Dropped:
                HasActiveQuest = false;
                return FlowResult.Done;

            default:
                return FlowResult.None;
        }
    }

    public readonly struct FlowResult
    {
        public static readonly FlowResult None = new FlowResult(false, 0, false);
        public static readonly FlowResult Done = new FlowResult(false, 0, true);

        public readonly bool ToggleSpawnPoint;
        public readonly int ToggleId;
        public readonly bool QuestCompleted;

        private FlowResult(bool toggleSpawnPoint, int toggleId, bool questCompleted)
        {
            ToggleSpawnPoint = toggleSpawnPoint;
            ToggleId = toggleId;
            QuestCompleted = questCompleted;
        }

        public static FlowResult Toggle(int id) => new FlowResult(true, id, false);
    }
}