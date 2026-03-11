using System;
using System.Collections.Generic;

[Serializable]
public struct DialoguePoolEntry
{
    public string poolId;
    public string poolName;
    public string transition;
    public string getOn;
    public List<string> during;
    public string end;
}