using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CabinEventManager : MonoBehaviour
{
    public static CabinEventManager Instance { get; private set; }

    public static string currentRide { get; private set; }
    public int requestID;

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
            TryPickup();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Dropoff();
        }
    }

    public void TryPickup()
    {
        string data = DataParser.GetRideRequest("40");
        if (data == null)
        {
            Debug.LogWarning($"CabinEventManager: No ride request found with ID {requestID}.");
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
