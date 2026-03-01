using System;
using UnityEngine;

public class CabinManager : MonoBehaviour
{
    public static CabinManager Instance { get; private set; }

    public RideRequestData currentRide { get; private set; }

    public event Action OnRideStarted;
    public event Action OnRideEnded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void OnPickup(int rideRequestID)
    {
        RideRequestData data = DatabaseManager.Instance.GetRideRequest(rideRequestID);
        if (data == null)
        {
            Debug.LogWarning($"CabinManager: No ride request found with ID {rideRequestID}.");
            return;
        }

        currentRide = data;
        OnRideStarted?.Invoke();
    }

    public void OnDropoff()
    {
        OnRideEnded?.Invoke();
        currentRide = null;
    }
}
