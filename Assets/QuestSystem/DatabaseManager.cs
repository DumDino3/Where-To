using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    private Dictionary<int, RideRequestData> rideRequests = new Dictionary<int, RideRequestData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadRideRequests();
    }

    private void LoadRideRequests()
    {
        TextAsset csv = Resources.Load<TextAsset>("RIDE_REQUEST");
        if (csv == null)
        {
            Debug.LogWarning("DatabaseManager: RIDE_REQUEST.csv not found in Resources.");
            return;
        }

        string[] lines = csv.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] fields = line.Split(',');
            if (fields.Length < 5) continue;

            if (!int.TryParse(fields[0].Trim(), out int id))
            {
                Debug.LogWarning($"DatabaseManager: Could not parse ID on line {i}: '{fields[0]}'");
                continue;
            }
            if (!int.TryParse(fields[2].Trim(), out int dialogueId))
            {
                Debug.LogWarning($"DatabaseManager: Could not parse DIALOGUE on line {i}: '{fields[2]}'");
                continue;
            }

            RideRequestData data = new RideRequestData
            {
                ID = id,
                NPC = fields[1].Trim(),
                DIALOGUE = dialogueId,
                SPAWN = fields[3].Trim(),
                DESTINATION = fields[4].Trim()
            };

            rideRequests[id] = data;
        }

        Debug.Log($"DatabaseManager: Loaded {rideRequests.Count} ride requests.");
    }

    public RideRequestData GetRideRequest(int id)
    {
        rideRequests.TryGetValue(id, out RideRequestData data);
        return data;
    }
}
