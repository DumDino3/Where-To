using System.Collections.Generic;
using System.Linq;

public sealed class RequestScheduler
{
    private readonly Dictionary<int, List<int>> _requestsBySeg = new();

    //this method will use parsed id to establish the time segments in the dictionary
    public void EstablishSegments(int timeSeg)
    {
        if (!_requestsBySeg.ContainsKey(timeSeg))
            _requestsBySeg.Add(timeSeg, new List<int>());
    }
    
    //this method will use parsed id to add the coreID into the corresponding time segment list
    public bool TryAddRawQuestId(string rawQuestId)
    {
        if (!RequestIDParser.TryGetCoreIDAndTimeSeg(rawQuestId, out int coreID, out int timeSeg))
            return false;

        EstablishSegments(timeSeg);
        _requestsBySeg[timeSeg].Add(coreID);
        return true;
    }
    

    //this method will return the sorted coreIDs for a given time segment, sorted by priority (extracted from coreID)
    public IEnumerable<int> GetSortedCoreIDForSegment(int timeSeg)
    {
        if (!_requestsBySeg.TryGetValue(timeSeg, out var list)) return Enumerable.Empty<int>();
        
        return list.OrderByDescending(RequestIDParser.GetPriorityFromCoreID);
    }
    
}