using System.Collections.Generic;
using System.Linq;

public sealed class RequestScheduler
{
    private readonly Dictionary<int, List<int>> _requestsBySeg = new();

    public void EnsureSegment(int timeSeg)
    {
        if (!_requestsBySeg.ContainsKey(timeSeg))
            _requestsBySeg.Add(timeSeg, new List<int>());
    }
    
    public bool TryAddRawQuestId(string rawQuestId)
    {
        if (!RequestIDParser.TryGetCoreIDAndTimeSeg(rawQuestId, out int coreID, out int timeSeg))
            return false;

        EnsureSegment(timeSeg);
        _requestsBySeg[timeSeg].Add(coreID);
        return true;
    }
    
    

    public IEnumerable<int> GetSortedCoreIDForSegment(int timeSeg)
    {
        if (!_requestsBySeg.TryGetValue(timeSeg, out var list))
            return Enumerable.Empty<int>();

        return list.OrderByDescending(RequestIDParser.GetPriorityFromCoreID);
    }
    
}