using System;

public static class RequestIDParser
{
    /// Full quest id format: DDDPPPQQQRRSS (13 chars)
    /// DDD=duration, PPP=pickup, QQQ=dropoff, RR=priority, SS=time segment
    public const int FullLength = 13;
    public const int CoreLength = 11;

    public static bool TryParseFull(
        string raw,
        out int duration,
        out int pickup,
        out int dropoff,
        out int priority,
        out int timeSeg)
    {
        duration = pickup = dropoff = priority = timeSeg = 0;

        if (string.IsNullOrEmpty(raw) || raw.Length < FullLength)
            return false;

        ReadOnlySpan<char> s = raw.AsSpan();
        if (!int.TryParse(s.Slice(0, 3), out duration)) return false;
        if (!int.TryParse(s.Slice(3, 3), out pickup)) return false;
        if (!int.TryParse(s.Slice(6, 3), out dropoff)) return false;
        if (!int.TryParse(s.Slice(9, 2), out priority)) return false;
        if (!int.TryParse(s.Slice(11, 2), out timeSeg)) return false;

        return true;
    }

    // Core id: the id without the priority and time segment (DDDPPPQQQ), used for core sorting/grouping.
    
    public static bool TryGetCoreIDAndTimeSeg(string raw, out int core11, out int timeSeg)
    {
        core11 = 0;
        timeSeg = 0;

        if (string.IsNullOrEmpty(raw) || raw.Length < FullLength)
            return false;

        ReadOnlySpan<char> s = raw.AsSpan();

        if (!int.TryParse(s.Slice(11, 2), out timeSeg)) return false;
        if (!int.TryParse(s.Slice(0, 11), out core11)) return false;

        return true;
    }

   
    public static (int dur, int pick, int drop) ParseCoreID(int core11)
    {
        string padded = core11.ToString("D11");
        ReadOnlySpan<char> s = padded.AsSpan();

        int dur = int.Parse(s.Slice(0, 3));
        int pick = int.Parse(s.Slice(3, 3));
        int drop = int.Parse(s.Slice(6, 3));
        return (dur, pick, drop);
    }

    public static int GetPriorityFromCoreID(int core11)
    {
        string padded = core11.ToString("D11");
        ReadOnlySpan<char> s = padded.AsSpan();
        return int.Parse(s.Slice(9, 2));
    }
}