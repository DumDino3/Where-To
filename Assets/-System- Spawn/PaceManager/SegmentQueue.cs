using System.Collections.Generic;
using UnityEngine;

public sealed class SegmentQueue 
{
    private Queue<int> coreID = new Queue<int>();
    
    
    
    public void PushCoreIDintoQueue(int id)
    {
        coreID.Enqueue(id);
        Debug.Log(id);
        
    }
    
    public int PopCoreIDFromQueue()
    {
        if (coreID.Count == 0)
        {
            return 0;
        }
        return coreID.Dequeue();
        
    }
    
    public int FlushQueue()
    {
        int count = coreID.Count;
        coreID.Clear();
        return count;
    }
    
}
