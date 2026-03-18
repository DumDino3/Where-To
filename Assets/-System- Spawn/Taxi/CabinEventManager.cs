using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CabinEventManager : MonoBehaviour
{
    public static CabinEventManager Instance { get; private set; }

    public RideRequest currentRide { get; private set; }

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
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TryPickup(currentRide.ID);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Dropoff();
        }
    }

    public void TryPickup(int rideRequestID)
    {
        RideRequest data = DataParser.Instance.GetRideRequest(rideRequestID);
        if (data == null)
        {
            Debug.LogWarning($"CabinEventManager: No ride request found with ID {rideRequestID}.");
            return;
        }

        currentRide = data;
        OnRideStarted?.Invoke();
    }

    public void Dropoff()
    {
        OnRideEnded?.Invoke();
        currentRide = null;
    }
}
