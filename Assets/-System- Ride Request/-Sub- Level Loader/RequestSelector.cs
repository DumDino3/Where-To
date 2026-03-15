using System.Collections.Generic;
using UnityEngine;

public static class RequestSelector
{
    public static List<RideRequestEntry> GetEligibleRequests(RideRequestDatabaseSO requestDb, ConditionDatabaseSO conditionDb, List<string> worldTags)
    {
        List<RideRequestEntry> eligibleRequestList = new List<RideRequestEntry>();

        if (requestDb == null || conditionDb == null || worldTags == null)
        {
            Debug.LogWarning("RequestSelector: Missing required input.");
            return eligibleRequestList;
        }

        List<RideRequestEntry> requests = requestDb.GetAll();

        foreach (RideRequestEntry request in requests)
        {
            ConditionEntry? condition = conditionDb.Search(request.conditionId);
            if (condition == null)
            {
                Debug.LogWarning($"RequestSelector: Missing condition '{request.conditionId}' for request '{request.requestId}'");
                continue;
            }

            if (IsConditionMet(condition.Value, worldTags))
                eligibleRequestList.Add(request);
        }

        return eligibleRequestList;
    }

    public static bool IsConditionMet(ConditionEntry condition, List<string> worldTags)
    {
        if (condition.includeTags != null)
        {
            foreach (string includeTag in condition.includeTags)
            {
                if (!worldTags.Contains(includeTag))
                    return false;
            }
        }

        if (condition.excludeTags != null)
        {
            foreach (string excludeTag in condition.excludeTags)
            {
                if (worldTags.Contains(excludeTag))
                    return false;
            }
        }

        return true;
    }
}