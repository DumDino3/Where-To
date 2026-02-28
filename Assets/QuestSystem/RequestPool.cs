using System.Collections.Generic;
using UnityEngine;

public class RequestPool : MonoBehaviour
{
    public List<RideRequestData> activeRequests = new List<RideRequestData>();

    public void AddRequest(int rideRequestID)
    {
        RideRequestData data = DatabaseManager.Instance.GetRideRequest(rideRequestID);
        if (data == null)
        {
            Debug.LogWarning($"RequestPool: No ride request found with ID {rideRequestID}.");
            return;
        }

        activeRequests.Add(data);
        Debug.Log($"RequestPool: Added ride request {rideRequestID} to pool.");
    }

    public void RemoveRequest(int rideRequestID)
    {
        activeRequests.RemoveAll(r => r.ID == rideRequestID);
    }
}
