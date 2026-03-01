using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Android;

public class RequestPool : MonoBehaviour
{
    public static RequestPool Instance {get; private set;}
    public List<int> requestID = new List<int>();
    [SerializeField] private List<RideRequest> activeRequests = new List<RideRequest>();
    public List<RideRequest> ActiveRequests
    {
        get { return activeRequests; }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        foreach(int request in requestID)
        {
            AddRequest(request);
        }
    }
    
    public void AddRequest(int rideRequestID)
    {
        RideRequest data = DataParser.Instance.GetRideRequest(rideRequestID);
        if (data == null)
        {
            Debug.LogWarning($"RequestPool: No ride request found with ID {rideRequestID}.");
            return;
        }

        ActiveRequests.Add(data);
        Debug.Log($"RequestPool: Added ride request {rideRequestID} to pool.");
    }

    public void RemoveRequest(int rideRequestID)
    {
        activeRequests.RemoveAll(r => r.ID == rideRequestID);
    }
}
