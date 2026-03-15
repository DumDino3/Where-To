using System;
using System.Collections.Generic;

[Serializable]
public struct ConditionEntry
{
    public string conditionId;
    public List<string> includeTags;
    public List<string> excludeTags;
}