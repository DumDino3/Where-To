using System.Collections.Generic;
using UnityEngine;

public class RequestManager : MonoBehaviour
{
    public static RequestManager Instance {get; private set;}

    public List<int> requestIDPool = new List<int>();
    public RideRequest currentRide { get; private set; }

    [SerializeField] private List<RideRequest> activeRequestPool = new List<RideRequest>();
    public List<RideRequest> ActiveRequestPool
    {
        get { return activeRequestPool; }
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
        foreach(int request in requestIDPool)
        {
            //AddRequest(request);
        }
    }
    
    // public void AddRequest(int requestID)
    // {
    //     RideRequest data = DataParser.GetRideRequest(requestID);
    //     if (data == null)
    //     {
    //         Debug.LogWarning($"RequestPool: No ride request found with ID {requestID}.");
    //         return;
    //     }

    //     activeRequestPool.Add(data);
    //     Debug.Log($"RequestPool: Added ride request {requestID} to pool.");
    // }

    // public void RemoveRequest(int rideRequestID)
    // {
    //     activeRequestPool.RemoveAll(r => r.ID == rideRequestID);
    // }
}
