using System;
using System.Collections.Generic;

[Serializable]
public struct DialoguePoolEntry
{
    public string poolName;
    public string poolId;
    public string transition;
    public string getOn;
    public string end;
    public List<string> during;
}