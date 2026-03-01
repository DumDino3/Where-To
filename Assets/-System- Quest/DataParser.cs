using System.Collections.Generic;
using UnityEngine;

public class DataParser : MonoBehaviour
{
    public static DataParser Instance { get; private set; }

    private Dictionary<int, RideRequest> rideRequestDictionary = new Dictionary<int, RideRequest>();
    private Dictionary<int, DialoguePool> dialoguePoolDictionary = new Dictionary<int, DialoguePool>();
    private Dictionary<int, LocationPool> locationPoolDictionary = new Dictionary<int, LocationPool>();
    private Dictionary<int, NpcId> npcIdDictionary = new Dictionary<int, NpcId>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //Parse data from database into instances of RideRequest classes
        LoadRideRequests();
        LoadDialoguePools();
        LoadLocationPools();
        LoadNpcIds();
    }

    private void LoadRideRequests()
    {
        // Assign the CSV
        TextAsset rideRequestCSV = Resources.Load<TextAsset>("CsvDatabase/RIDE_REQUEST");
        if (rideRequestCSV == null)
        {
            Debug.LogWarning("DataParser: Make sure RIDE_REQUEST.csv is in CsvDatabase/");
            return;
        }

        //Split the CSV into single lines
        string[] rideRequestLines = rideRequestCSV.text.Split('\n');

        //Further split each line into fields
        for (int i = 0; i < rideRequestLines.Length; i++)
        {
            //Trim the blank spaces at the end and beginning of the line, then check if it there is still any text left
            string line = rideRequestLines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            //Split by "," to separate datas on each line into fields, check if the fields excceeds the dictionary-TValue-class's amount of fields
            string[] fields = line.Split(',');
            if (fields.Length < 9) continue;

            //Manually, for each field, by order: trim blank space start and end, then assign each field to a variable
            //If the assigning fails (field is not an int or is blank) return an error log
            string name = fields[0].Trim();
            if (!int.TryParse(fields[1].Trim(), out int id))
            {
                Debug.LogWarning($"rideRequestDictionary: Could not parse ID on line {i}: '{fields[1]}'");
                continue;
            }
            if (!int.TryParse(fields[2].Trim(), out int duration))
            {
                Debug.LogWarning($"rideRequestDictionary: Could not parse DURATION on line {i}: '{fields[2]}'");
                continue;
            }
            if (!int.TryParse(fields[3].Trim(), out int npc))
            {
                Debug.LogWarning($"rideRequestDictionary: Could not parse NPC on line {i}: '{fields[3]}'");
                continue;
            }
            if (!int.TryParse(fields[4].Trim(), out int dialogueId))
            {
                Debug.LogWarning($"rideRequestDictionary: Could not parse DIALOGUE on line {i}: '{fields[4]}'");
                continue;
            }
            if (!int.TryParse(fields[5].Trim(), out int spawn))
            {
                Debug.LogWarning($"rideRequestDictionary: Could not parse SPAWN on line {i}: '{fields[5]}'");
                continue;
            }
            if (!int.TryParse(fields[6].Trim(), out int destination))
            {
                Debug.LogWarning($"rideRequestDictionary: Could not parse DESTINATION on line {i}: '{fields[6]}'");
                continue;
            }
            if (!int.TryParse(fields[7].Trim(), out int priority))
            {
                Debug.LogWarning($"rideRequestDictionary: Could not parse PRIORITY on line {i}: '{fields[7]}'");
                continue;
            }
            if (!int.TryParse(fields[8].Trim(), out int tag))
            {
                Debug.LogWarning($"rideRequestDictionary: Could not parse TAG on line {i}: '{fields[8]}'");
                continue;
            }
            
            //Create a RideRequest instance, then apply the parsed variables above to its fields
            RideRequest rideRequest = new RideRequest
            {
                NAME = name,
                ID = id,
                DURATION = duration,
                NPC = npc,
                DIALOGUE = dialogueId,
                SPAWN = spawn,
                DESTINATION = destination,
                PRIORITY = priority,
                TAG = tag
            };

            rideRequestDictionary[id] = rideRequest;
        }

        Debug.Log($"DataParser: Loaded {rideRequestDictionary.Count} ride requests.");
    }

    public RideRequest GetRideRequest(int id)
    {
        rideRequestDictionary.TryGetValue(id, out RideRequest rideRequest);
        return rideRequest;
    }

    private void LoadDialoguePools()
    {
        // Assign the CSV
        TextAsset dialoguePoolCSV = Resources.Load<TextAsset>("CsvDatabase/DIALOGUE_POOL");
        if (dialoguePoolCSV == null)
        {
            Debug.LogWarning("DataParser: Make sure DIALOGUE_POOL.csv is in CsvDatabase/");
            return;
        }

        //Split the CSV into single lines
        string[] dialoguePoolLines = dialoguePoolCSV.text.Split('\n');

        //Further split each line into fields
        for (int i = 0; i < dialoguePoolLines.Length; i++)
        {
            //Trim the blank spaces at the end and beginning of the line, then check if it there is still any text left
            string line = dialoguePoolLines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            //Split by "," to separate datas on each line into fields, check if the fields excceeds the dictionary-TValue-class's amount of fields
            string[] fields = line.Split(',');
            if (fields.Length < 6) continue;

            //Manually, for each field, by order: trim blank space start and end, then assign each field to a variable
            //If the assigning fails (field is not an int or is blank) return an error log
            string name = fields[0].Trim();
            if (!int.TryParse(fields[1].Trim(), out int id))
            {
                Debug.LogWarning($"dialoguePoolDictionary: Could not parse ID on line {i}: '{fields[1]}'");
                continue;
            }
            string trans = fields[2].Trim();
            string geton = fields[3].Trim();
            string ride = fields[4].Trim();
            string end = fields[5].Trim();

            //Create a DialoguePool instance, then apply the parsed variables above to its fields
            DialoguePool dialoguePool = new DialoguePool
            {
                NAME = name,
                ID = id,
                TRANS = trans,
                GETON = geton,
                RIDE = ride,
                END = end
            };

            dialoguePoolDictionary[id] = dialoguePool;
        }

        Debug.Log($"DataParser: Loaded {dialoguePoolDictionary.Count} dialogue pools.");
    }

    public DialoguePool GetDialoguePool(int id)
    {
        dialoguePoolDictionary.TryGetValue(id, out DialoguePool dialoguePool);
        return dialoguePool;
    }

    private void LoadLocationPools()
    {
        // Assign the CSV
        TextAsset locationPoolCSV = Resources.Load<TextAsset>("CsvDatabase/LOCATION_POOL");
        if (locationPoolCSV == null)
        {
            Debug.LogWarning("DataParser: Make sure LOCATION_POOL.csv is in CsvDatabase/");
            return;
        }

        //Split the CSV into single lines
        string[] locationPoolLines = locationPoolCSV.text.Split('\n');

        //Further split each line into fields
        for (int i = 0; i < locationPoolLines.Length; i++)
        {
            //Trim the blank spaces at the end and beginning of the line, then check if it there is still any text left
            string line = locationPoolLines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            //Split by "," to separate datas on each line into fields, check if the fields excceeds the dictionary-TValue-class's amount of fields
            string[] fields = line.Split(',');
            if (fields.Length < 2) continue;

            //Manually, for each field, by order: trim blank space start and end, then assign each field to a variable
            //If the assigning fails (field is not an int or is blank) return an error log
            string name = fields[0].Trim();
            if (!int.TryParse(fields[1].Trim(), out int id))
            {
                Debug.LogWarning($"locationPoolDictionary: Could not parse ID on line {i}: '{fields[1]}'");
                continue;
            }

            //Create a LocationPool instance, then apply the parsed variables above to its fields
            LocationPool locationPool = new LocationPool
            {
                NAME = name,
                ID = id
            };

            locationPoolDictionary[id] = locationPool;
        }

        Debug.Log($"DataParser: Loaded {locationPoolDictionary.Count} location pools.");
    }

    public LocationPool GetLocationPool(int id)
    {
        locationPoolDictionary.TryGetValue(id, out LocationPool locationPool);
        return locationPool;
    }

    private void LoadNpcIds()
    {
        // Assign the CSV
        TextAsset npcIdCSV = Resources.Load<TextAsset>("CsvDatabase/NPC_ID");
        if (npcIdCSV == null)
        {
            Debug.LogWarning("DataParser: Make sure NPC_ID.csv is in CsvDatabase/");
            return;
        }

        //Split the CSV into single lines
        string[] npcIdLines = npcIdCSV.text.Split('\n');

        //Further split each line into fields
        for (int i = 0; i < npcIdLines.Length; i++)
        {
            //Trim the blank spaces at the end and beginning of the line, then check if it there is still any text left
            string line = npcIdLines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            //Split by "," to separate datas on each line into fields, check if the fields excceeds the dictionary-TValue-class's amount of fields
            string[] fields = line.Split(',');
            if (fields.Length < 4) continue;

            //Manually, for each field, by order: trim blank space start and end, then assign each field to a variable
            //If the assigning fails (field is not an int or is blank) return an error log
            string name = fields[0].Trim();
            if (!int.TryParse(fields[1].Trim(), out int id))
            {
                Debug.LogWarning($"npcIdDictionary: Could not parse ID on line {i}: '{fields[1]}'");
                continue;
            }
            string displayname = fields[2].Trim();
            string tag = fields[3].Trim();

            //Create a NpcId instance, then apply the parsed variables above to its fields
            NpcId npcId = new NpcId
            {
                NAME = name,
                ID = id,
                DISPLAYNAME = displayname,
                TAG = tag
            };

            npcIdDictionary[id] = npcId;
        }

        Debug.Log($"DataParser: Loaded {npcIdDictionary.Count} npc ids.");
    }

    public NpcId GetNpcId(int id)
    {
        npcIdDictionary.TryGetValue(id, out NpcId npcId);
        return npcId;
    }
}
