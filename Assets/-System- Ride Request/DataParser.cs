using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;

public class DataParser : MonoBehaviour
{
    public static DataParser Instance { get; private set; }

    private static Dictionary<int, long> requestIdDictionary = new Dictionary<int, long>();
    public static Dictionary<int, long> RequestIdDictionary {get{return requestIdDictionary;}}

    private static Dictionary<int, DialoguePool> dialogueIdDictionary = new Dictionary<int, DialoguePool>();
    public static Dictionary<int, DialoguePool> DialogueIdDictionary {get{return dialogueIdDictionary;}}

    private static Dictionary<int, long> locationIdDictionary = new Dictionary<int, long>();
    public static Dictionary<int, long> LocationIdDictionary {get{return locationIdDictionary;}}

    private static Dictionary<int, long> npcIdDictionary = new Dictionary<int, long>();
    public static Dictionary<int, long> NpcIdDictionary {get{return npcIdDictionary;}}


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

            //Split by "," to separate datas on each line into fields
            string[] fields = line.Split(',');
            if (fields.Length < 2) continue;

            //Skip field 0, use field 1 as ID (key)
            if (!int.TryParse(fields[1].Trim(), out int id))
            {
                Debug.LogWarning($"rideRequestDictionary: Could not parse ID on line {i}: '{fields[1]}'");
                continue;
            }

            //Combine remaining integer fields (fields 2+) into one long int
            string combinedValue = "";
            bool validData = true;
            for (int j = 2; j < fields.Length; j++)
            {
                if (!int.TryParse(fields[j].Trim(), out int value))
                {
                    Debug.LogWarning($"rideRequestDictionary: Could not parse field {j} on line {i}: '{fields[j]}'");
                    validData = false;
                    break;
                }
                combinedValue += value.ToString();
            }

            if (!validData || !long.TryParse(combinedValue, out long combinedInt))
            {
                Debug.LogWarning($"rideRequestDictionary: Could not create combined long on line {i}");
                continue;
            }

            requestIdDictionary[id] = combinedInt;
        }

        Debug.Log($"DataParser: Loaded {requestIdDictionary.Count} ride requests.");
    }

    public static long GetRideRequest(int id)
    {
        requestIdDictionary.TryGetValue(id, out long rideRequest);
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

            dialogueIdDictionary[id] = dialoguePool;
        }

        Debug.Log($"DataParser: Loaded {dialogueIdDictionary.Count} dialogue pools.");
    }

    public static DialoguePool GetDialoguePool(int id)
    {
        dialogueIdDictionary.TryGetValue(id, out DialoguePool dialoguePool);
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

            //Split by "," to separate datas on each line into fields
            string[] fields = line.Split(',');
            if (fields.Length < 2) continue;

            //Skip field 0, use field 1 as ID (key)
            if (!int.TryParse(fields[1].Trim(), out int id))
            {
                Debug.LogWarning($"locationPoolDictionary: Could not parse ID on line {i}: '{fields[1]}'");
                continue;
            }

            //Combine remaining integer fields (fields 2+) into one long int
            string combinedValue = "";
            bool validData = true;
            for (int j = 2; j < fields.Length; j++)
            {
                if (!int.TryParse(fields[j].Trim(), out int value))
                {
                    Debug.LogWarning($"locationPoolDictionary: Could not parse field {j} on line {i}: '{fields[j]}'");
                    validData = false;
                    break;
                }
                combinedValue += value.ToString();
            }

            if (!validData || !long.TryParse(combinedValue, out long combinedInt))
            {
                Debug.LogWarning($"locationPoolDictionary: Could not create combined long on line {i}");
                continue;
            }

            locationIdDictionary[id] = combinedInt;
        }

        Debug.Log($"DataParser: Loaded {locationIdDictionary.Count} location pools.");
    }

    public static long GetLocationPool(int id)
    {
        locationIdDictionary.TryGetValue(id, out long locationPool);
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

            //Split by "," to separate datas on each line into fields
            string[] fields = line.Split(',');
            if (fields.Length < 2) continue;

            //Skip field 0, use field 1 as ID (key)
            if (!int.TryParse(fields[1].Trim(), out int id))
            {
                Debug.LogWarning($"npcIdDictionary: Could not parse ID on line {i}: '{fields[1]}'");
                continue;
            }

            //Combine remaining integer fields (fields 2+) into one long int
            string combinedValue = "";
            bool validData = true;
            for (int j = 2; j < fields.Length; j++)
            {
                if (!int.TryParse(fields[j].Trim(), out int value))
                {
                    Debug.LogWarning($"npcIdDictionary: Could not parse field {j} on line {i}: '{fields[j]}'");
                    validData = false;
                    break;
                }
                combinedValue += value.ToString();
            }

            if (!validData || !long.TryParse(combinedValue, out long combinedInt))
            {
                Debug.LogWarning($"npcIdDictionary: Could not create combined long on line {i}");
                continue;
            }

            npcIdDictionary[id] = combinedInt;
        }

        Debug.Log($"DataParser: Loaded {npcIdDictionary.Count} npc ids.");
    }

    public static long GetNpcId(int id)
    {
        npcIdDictionary.TryGetValue(id, out long npcId);
        return npcId;
    }
}
