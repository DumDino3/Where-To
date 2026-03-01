using UnityEngine;

public class DialogueRouter : MonoBehaviour
{
    private void Start()
    {
        // Use Start instead of OnEnable to ensure CabinEventManager.Awake() has run
        if (CabinEventManager.Instance == null)
        {
            Debug.LogWarning("DialogueRouter: CabinEventManager instance not found. Make sure CabinEventManager GameObject exists in the scene.");
            return;
        }
        CabinEventManager.Instance.OnRideStarted += HandleRideStarted;
        CabinEventManager.Instance.OnRideEnded += HandleRideEnded;
    }

    private void OnDisable()
    {
        if (CabinEventManager.Instance == null) return;
        CabinEventManager.Instance.OnRideStarted -= HandleRideStarted;
        CabinEventManager.Instance.OnRideEnded -= HandleRideEnded;
    }

    private void HandleRideStarted()
    {
        RideRequest ride = CabinEventManager.Instance.currentRide;
        if (ride == null)
        {
            Debug.LogWarning("DialogueRouter: HandleRideStarted called but currentRide is null.");
            return;
        }
        Debug.Log($"Picked up ride. Dialogue ID: {ride.DIALOGUE}");
    }

    private void HandleRideEnded()
    {
        Debug.Log("Ride ended. Passenger dropped off.");
    }
}
