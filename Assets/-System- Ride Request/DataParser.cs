using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;

public class DataParser : MonoBehaviour
{
    public static DataParser Instance { get; private set; }

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
        // LoadDialoguePools();
        // LoadDialogueIds();
        // LoadLocationPools();
        // LoadNpcIds();
    }






//--------------------- RIDE REQUEST -----------------------







    private static Dictionary<string, string> requestIdDictionary = new Dictionary<string, string>();
    public static Dictionary<string, string> RequestIdDictionary { get { return requestIdDictionary; } }

    private void LoadRideRequests()
    {
        requestIdDictionary.Clear();

        // Assign the CSV
        TextAsset rideRequestCSV = Resources.Load<TextAsset>("CsvDatabase/RIDE_REQUEST");

        //Split the CSV into single lines
        string[] rideRequestLines = rideRequestCSV.text.Split('\n');

        //Further split each line into fields
        for (int i = 0; i < rideRequestLines.Length; i++)
        {
            //Trim the blank spaces at the end and beginning of the line, then check if it there is still any text left
            string line = rideRequestLines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            //Split by "," to separate data on each line into fields
            string[] fields = line.Split(',');
            if (fields.Length != 9)
            {
                Debug.LogWarning($"rideRequestDictionary: Field amount mismatch on row {i}");
                continue;
            }

            //Check and assign field [1] as ID on its own
            string idKey = fields[1].Trim();
            if (string.IsNullOrEmpty(idKey) && idKey.Length != 3){
                Debug.LogWarning($"Invalid ID on row {i}");
            }
            string id = idKey;

            for(int e = 0; e > 1 && e < 8; e++)
            {
                if (string.IsNullOrEmpty(fields[e]) && fields[e].Length != 3){
                    Debug.LogWarning($"Invalid ID on row {i}");
                }
            }

            for(int e = 0; e > 7 && e < 10; e++)
            {
                if (string.IsNullOrEmpty(fields[e]) && fields[e].Length != 2){
                    Debug.LogWarning($"Invalid ID on row {i}");
                }
            }

            //Check the rest of the field

            requestIdDictionary[id] = String.Concat(fields[2].Trim(),fields[3].Trim(),fields[4].Trim(),fields[5].Trim(),fields[6].Trim(),fields[7].Trim(),fields[8].Trim(),fields[9].Trim());
        }

        Debug.Log($"DataParser: Loaded {requestIdDictionary.Count} ride requests.");
    }

    public static string GetRideRequest(string id)
    {
        requestIdDictionary.TryGetValue(id, out string rideRequest);
        return rideRequest;
    }
}




//--------------------- DIALOGUE POOL -----------------------






//     private void LoadDialoguePools()
//     {
//         // Assign the CSV
//         TextAsset dialoguePoolCSV = Resources.Load<TextAsset>("CsvDatabase/DIALOGUE_POOL");
//         if (dialoguePoolCSV == null)
//         {
//             Debug.LogWarning("DataParser: Make sure DIALOGUE_POOL.csv is in CsvDatabase/");
//             return;
//         }

//         //Split the CSV into single lines
//         string[] dialoguePoolLines = dialoguePoolCSV.text.Split('\n');

//         //Further split each line into fields
//         for (int i = 0; i < dialoguePoolLines.Length; i++)
//         {
//             //Trim the blank spaces at the end and beginning of the line, then check if it there is still any text left
//             string line = dialoguePoolLines[i].Trim();
//             if (string.IsNullOrEmpty(line)) continue;

//             // Format per line: <name>,<id values>.,<trans values>.,<geton values>.,<ride values>.,<getoff values>
//             // Ignore field[0] which is name, then split remaining fields into groups using "." markers
//             string[] fields = line.Split(',');
//             if (fields.Length < 2) continue;

//             List<List<string>> groupedFields = new List<List<string>>();
//             List<string> currentGroup = new List<string>();

//             for (int j = 1; j < fields.Length; j++)
//             {
//                 string token = fields[j].Trim();

//                 // Ignore empty CSV fields
//                 if (string.IsNullOrEmpty(token))
//                 {
//                     continue;
//                 }

//                 if (token == ".")
//                 {
//                     groupedFields.Add(currentGroup);
//                     currentGroup = new List<string>();
//                     continue;
//                 }

//                 currentGroup.Add(token);
//             }

//             groupedFields.Add(currentGroup);

//             if (groupedFields.Count < 5)
//             {
//                 Debug.LogWarning($"dialoguePoolDictionary: Expected at least 5 grouped fields on line {i}, found {groupedFields.Count}");
//                 continue;
//             }

//             if (groupedFields[0].Count == 0 || !int.TryParse(groupedFields[0][0], out int id))
//             {
//                 string rawId = groupedFields[0].Count > 0 ? groupedFields[0][0] : "<empty>";
//                 Debug.LogWarning($"dialoguePoolDictionary: Could not parse ID on line {i}: '{rawId}'");
//                 continue;
//             }

//             if (!TryParseDialogueIntList(groupedFields[1], i, "TRANS", out List<int> trans)) continue;
//             if (!TryParseDialogueIntList(groupedFields[2], i, "GETON", out List<int> geton)) continue;
//             if (!TryParseDialogueIntList(groupedFields[3], i, "RIDE", out List<int> ride)) continue;
//             if (!TryParseDialogueIntList(groupedFields[4], i, "END", out List<int> end)) continue;

//             //Create a DialoguePool instance, then apply the parsed variables above to its fields
//             DialoguePool dialoguePool = new DialoguePool
//             {
//                 ID = id,
//                 TRANS = trans,
//                 GETON = geton,
//                 RIDE = ride,
//                 END = end
//             };

//             dialoguePoolDictionary[id] = dialoguePool;
//         }

//         Debug.Log($"DataParser: Loaded {dialoguePoolDictionary.Count} dialogue pools.");
//     }

//     private static bool TryParseDialogueIntList(List<string> tokens, int lineIndex, string fieldName, out List<int> parsedValues)
//     {
//         parsedValues = new List<int>();

//         for (int i = 0; i < tokens.Count; i++)
//         {
//             if (!int.TryParse(tokens[i], out int value))
//             {
//                 Debug.LogWarning($"dialoguePoolDictionary: Could not parse {fieldName} value on line {lineIndex}: '{tokens[i]}'");
//                 return false;
//             }

//             parsedValues.Add(value);
//         }

//         return true;
//     }

//     public static DialoguePool GetDialoguePool(int id)
//     {
//         dialoguePoolDictionary.TryGetValue(id, out DialoguePool dialoguePool);
//         return dialoguePool;
//     }





// //--------------------- DIALOGUE POOL -----------------------





//     private static Dictionary<string, DialoguePool> dialoguePoolDictionary = new Dictionary<string, DialoguePool>();
//     public static Dictionary<string, DialoguePool> DialoguePoolDictionary {get{return dialoguePoolDictionary;}}

//     private void LoadDialogueIds()
//     {
//         // Assign the CSV
//         TextAsset dialogueIdCSV = Resources.Load<TextAsset>("CsvDatabase/DIAG_TITLE_POOL");
//         if (dialogueIdCSV == null)
//         {
//             Debug.LogWarning("DataParser: Make sure DIAG_TITLE_POOL.csv is in CsvDatabase/");
//             return;
//         }

//         //Split the CSV into single lines
//         string[] dialogueIdLines = dialogueIdCSV.text.Split('\n');

//         //Further split each line into fields
//         for (int i = 0; i < dialogueIdLines.Length; i++)
//         {
//             //Trim the blank spaces at the end and beginning of the line, then check if it there is still any text left
//             string line = dialogueIdLines[i].Trim();
//             if (string.IsNullOrEmpty(line)) continue;

//             // Format: name, id, yarnspinner title string, ...
//             // Ignore field[0] (name)
//             string[] fields = line.Split(',');
//             if (fields.Length < 3)
//             {
//                 Debug.LogWarning($"dialogueIdDictionary: Not enough fields on line {i}");
//                 continue;
//             }

//             if (!int.TryParse(fields[1].Trim(), out int id))
//             {
//                 Debug.LogWarning($"dialogueIdDictionary: Could not parse ID on line {i}: '{fields[1]}'");
//                 continue;
//             }

//             string dialogueTitle = fields[2].Trim();
//             if (string.IsNullOrEmpty(dialogueTitle))
//             {
//                 Debug.LogWarning($"dialogueIdDictionary: Empty dialogue title on line {i}");
//                 continue;
//             }

//             dialogueIdDictionary[id] = dialogueTitle;
//         }

//         Debug.Log($"DataParser: Loaded {dialogueIdDictionary.Count} dialogue IDs.");
//     }

//     public static string GetDialogue(int id)
//     {
//         dialogueIdDictionary.TryGetValue(id, out string dialogueTitle);
//         return dialogueTitle;
//     }





// //--------------------- LOCATION POOL -----------------------





//     private static Dictionary<string, string> locationIdDictionary = new Dictionary<string, string>();
//     public static Dictionary<string, string> LocationIdDictionary {get{return locationIdDictionary;}}

//     private void LoadLocationPools()
//     {
//         // Assign the CSV
//         TextAsset locationPoolCSV = Resources.Load<TextAsset>("CsvDatabase/LOCATION_POOL");
//         if (locationPoolCSV == null)
//         {
//             Debug.LogWarning("DataParser: Make sure LOCATION_POOL.csv is in CsvDatabase/");
//             return;
//         }

//         //Split the CSV into single lines
//         string[] locationPoolLines = locationPoolCSV.text.Split('\n');

//         //Further split each line into fields
//         for (int i = 0; i < locationPoolLines.Length; i++)
//         {
//             //Trim the blank spaces at the end and beginning of the line, then check if it there is still any text left
//             string line = locationPoolLines[i].Trim();
//             if (string.IsNullOrEmpty(line)) continue;

//             //Split by "," to separate datas on each line into fields
//             string[] fields = line.Split(',');
//             if (fields.Length < 2) continue;

//             //Skip field 0, use field 1 as ID (key)
//             if (!int.TryParse(fields[1].Trim(), out int id))
//             {
//                 Debug.LogWarning($"locationPoolDictionary: Could not parse ID on line {i}: '{fields[1]}'");
//                 continue;
//             }

//             //Combine remaining integer fields (fields 2+) into one long int
//             string combinedValue = "";
//             bool validData = true;
//             for (int j = 2; j < fields.Length; j++)
//             {
//                 if (!int.TryParse(fields[j].Trim(), out int value))
//                 {
//                     Debug.LogWarning($"locationPoolDictionary: Could not parse field {j} on line {i}: '{fields[j]}'");
//                     validData = false;
//                     break;
//                 }
//                 combinedValue += value.ToString();
//             }

//             if (!validData || !long.TryParse(combinedValue, out long combinedInt))
//             {
//                 Debug.LogWarning($"locationPoolDictionary: Could not create combined long on line {i}");
//                 continue;
//             }

//             locationIdDictionary[id] = combinedInt;
//         }

//         Debug.Log($"DataParser: Loaded {locationIdDictionary.Count} location pools.");
//     }

//     public static long GetLocationPool(int id)
//     {
//         locationIdDictionary.TryGetValue(id, out long locationPool);
//         return locationPool;
//     }





// //--------------------- NPCID POOL -----------------------




//     private static Dictionary<string, string> npcIdDictionary = new Dictionary<string, string>();
//     public static Dictionary<string, string> NpcIdDictionary {get{return npcIdDictionary;}}


//     private void LoadNpcIds()
//     {
//         // Assign the CSV
//         TextAsset npcIdCSV = Resources.Load<TextAsset>("CsvDatabase/NPC_ID");
//         if (npcIdCSV == null)
//         {
//             Debug.LogWarning("DataParser: Make sure NPC_ID.csv is in CsvDatabase/");
//             return;
//         }

//         //Split the CSV into single lines
//         string[] npcIdLines = npcIdCSV.text.Split('\n');

//         //Further split each line into fields
//         for (int i = 0; i < npcIdLines.Length; i++)
//         {
//             //Trim the blank spaces at the end and beginning of the line, then check if it there is still any text left
//             string line = npcIdLines[i].Trim();
//             if (string.IsNullOrEmpty(line)) continue;

//             //Split by "," to separate datas on each line into fields
//             string[] fields = line.Split(',');
//             if (fields.Length < 2) continue;

//             //Skip field 0, use field 1 as ID (key)
//             if (!int.TryParse(fields[1].Trim(), out int id))
//             {
//                 Debug.LogWarning($"npcIdDictionary: Could not parse ID on line {i}: '{fields[1]}'");
//                 continue;
//             }

//             //Combine remaining integer fields (fields 2+) into one long int
//             string combinedValue = "";
//             bool validData = true;
//             for (int j = 2; j < fields.Length; j++)
//             {
//                 if (!int.TryParse(fields[j].Trim(), out int value))
//                 {
//                     Debug.LogWarning($"npcIdDictionary: Could not parse field {j} on line {i}: '{fields[j]}'");
//                     validData = false;
//                     break;
//                 }
//                 combinedValue += value.ToString();
//             }

//             if (!validData || !long.TryParse(combinedValue, out long combinedInt))
//             {
//                 Debug.LogWarning($"npcIdDictionary: Could not create combined long on line {i}");
//                 continue;
//             }

//             npcIdDictionary[id] = combinedInt;
//         }

//         Debug.Log($"DataParser: Loaded {npcIdDictionary.Count} npc ids.");
//     }

//     public static long GetNpcId(int id)
//     {
//         npcIdDictionary.TryGetValue(id, out long npcId);
//         return npcId;
//     }
// }
