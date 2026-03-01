using UnityEngine;

public class DialogueRouter : MonoBehaviour
{
    private void OnEnable()
    {
        if (CabinManager.Instance == null)
        {
            Debug.LogWarning("DialogueRouter: CabinManager instance not found.");
            return;
        }
        CabinManager.Instance.OnRideStarted += HandleRideStarted;
        CabinManager.Instance.OnRideEnded += HandleRideEnded;
    }

    private void OnDisable()
    {
        if (CabinManager.Instance == null) return;
        CabinManager.Instance.OnRideStarted -= HandleRideStarted;
        CabinManager.Instance.OnRideEnded -= HandleRideEnded;
    }

    private void HandleRideStarted()
    {
        RideRequestData ride = CabinManager.Instance.currentRide;
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
