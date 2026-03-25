using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DatabaseImporter
{
    #region Import All

    [MenuItem("Tools/ImportCsvToDatabase/All")]
    public static void ImportAll()
    {
        ImportLocations();
        ImportNPCs();
        ImportDialoguePools();
        ImportTags();
        ImportRideRequests();
        Debug.Log("DatabaseImporter: Worked well :)");
    }
    #endregion



    #region Ride Request
    private const string RIDE_REQUEST_CSV = "CsvDatabase/RIDE_REQUEST";
    private const string RIDE_REQUEST_ASSET = "Assets/Resources/SO/Asset/RideRequestDatabaseSO.asset";

    [MenuItem("Tools/ImportCsvToDatabase/Ride Requests")]
    public static void ImportRideRequests()
    {
        // Load CSV file into script
        TextAsset csv = Resources.Load<TextAsset>(RIDE_REQUEST_CSV);
        if (csv == null)
        {
            Debug.LogError("CSV not found at Resources/" + RIDE_REQUEST_CSV);
            return;
        }

        // Check for SO asset, if none create a new one. Regardless of SO asset presence, clear the list before write.
        RideRequestDatabaseSO databaseSO = AssetDatabase.LoadAssetAtPath<RideRequestDatabaseSO>(RIDE_REQUEST_ASSET);
        if (databaseSO == null)
        {
            databaseSO = ScriptableObject.CreateInstance<RideRequestDatabaseSO>();
            AssetDatabase.CreateAsset(databaseSO, RIDE_REQUEST_ASSET);
        }
        databaseSO.entries.Clear();

        // Split CSV file into lines
        string[] lines = csv.text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim().TrimEnd(',');
            if (string.IsNullOrEmpty(line)) continue;

            string[] fields = line.Split(',');
            if (fields.Length < 10) continue; // Ensure all columns are present

            List<string> conditionList = new List<string>();
            if (fields.Length >= 10 && !string.IsNullOrWhiteSpace(fields[5]))
            {
                string[] duringParts = fields[10].Split('/');
                foreach (var part in duringParts)
                {
                    string trimmed = part.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                        conditionList.Add(trimmed);
                }
            }


            // Example: parse fields
            string day = fields[0].Trim();
            string name = fields[1].Trim();
            string id = fields[2].Trim();
            string segment = fields[3].Trim();
            string priority = fields[4].Trim();
            string duration = fields[5].Trim();
            string npc = fields[6].Trim();
            string dialogue = fields[7].Trim();
            string spawn = fields[8].Trim();
            string destination = fields[9].Trim();

            // Add to database
            databaseSO.entries.Add(new RideRequestEntry
            {
                day = day,
                requestName = name,
                requestId = id,
                timeSegment = segment,
                priority = priority,
                duration = duration,
                npcId = npc,
                dialoguePoolId = dialogue,
                spawnId = spawn,
                destinationId = destination,
                condition = conditionList
            });
        }

        EditorUtility.SetDirty(databaseSO);
        AssetDatabase.SaveAssets();
        Debug.Log($"RideRequestImporter: Imported {databaseSO.entries.Count} ride requests.");
    }
    #endregion



    #region Locations
    private const string LOCATION_CSV = "CsvDatabase/LOCATION_ID";
    private const string LOCATION_ASSET = "Assets/Resources/SO/Asset/LocationDatabaseSO.asset";

    [MenuItem("Tools/ImportCsvToDatabase/Locations")]
    public static void ImportLocations()
    {
        //Load CSV file into script
        TextAsset csv = Resources.Load<TextAsset>(LOCATION_CSV);
        if (csv == null)
        {
            Debug.LogError("CSV not found at Resources/" + LOCATION_CSV);
            return;
        }

        //Check for SO asset, if none create a new one. Regardless of SO asset presence, clear the list before write.
        LocationDatabaseSO databaseSO = AssetDatabase.LoadAssetAtPath<LocationDatabaseSO>(LOCATION_ASSET);
        if (databaseSO == null)
        {
            databaseSO = ScriptableObject.CreateInstance<LocationDatabaseSO>();
            AssetDatabase.CreateAsset(databaseSO, LOCATION_ASSET);
        }
        databaseSO.entries.Clear();

        //Split CSV file into lines
        string[] lines = csv.text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            //Trim blank spaces at start and end. Check if there is data left.
            string line = lines[i].Trim().TrimEnd(',');
            if (string.IsNullOrEmpty(line)) continue;

            //Further split each line into fields
            string[] fields = line.Split(',');
            if (fields.Length < 2) continue;

            //Trim fields, check if valid id, add valid entry to DataBase list
            string name = fields[0].Trim();
            string id = fields[1].Trim();
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"LocationImporter: No Id found on row {i}");
                continue;
            }
            databaseSO.entries.Add(new LocationEntry
            {
                name = name,
                id = id
            });
        }

        //Set edited SO assets as dirty (edited), trigger save assets so unity writes dirty files
        EditorUtility.SetDirty(databaseSO);
        AssetDatabase.SaveAssets();
        Debug.Log($"LocationImporter: Imported {databaseSO.entries.Count} entries.");
    }
    #endregion




    #region NPCs
    private const string NPC_CSV = "CsvDatabase/NPC_ID";
    private const string NPC_FOLDER = "Assets/Resources/SO/NPC";

    [MenuItem("Tools/ImportCsvToDatabase/NPCs")]
    public static void ImportNPCs()
    {
        //Load CSV file into script
        TextAsset csv = Resources.Load<TextAsset>(NPC_CSV);
        if (csv == null)
        {
            Debug.LogError("CSV not found at Resources/" + NPC_CSV);
            return;
        }

        //Ensure output folder exists
        if (!AssetDatabase.IsValidFolder(NPC_FOLDER))
        {
            Debug.LogWarning($"NpcImporter: Missing folder '{NPC_FOLDER}'. Create it first, then re-run import.");
            return;
        }

        //Split CSV file into lines
        string[] lines = csv.text.Split('\n');
        int created = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            //Trim blank spaces at start and end. Check if there is data left.
            string line = lines[i].Trim().TrimEnd(',');
            if (string.IsNullOrEmpty(line)) continue;

            //Further split each line into fields
            string[] fields = line.Split(',');
            if (fields.Length < 4) continue;

            //Trim fields, check if valid id
            string npcName = fields[0].Trim();
            string id = fields[1].Trim();
            string displayName = fields[2].Trim();
            string modelId = fields[3].Trim();

            //Create or update individual NpcSO asset using npcName as file name
            string assetPath = $"{NPC_FOLDER}/{npcName}.asset";
            NpcSO npc = AssetDatabase.LoadAssetAtPath<NpcSO>(assetPath);
            if (npc == null)
            {
                npc = ScriptableObject.CreateInstance<NpcSO>();
                AssetDatabase.CreateAsset(npc, assetPath);
            }

            npc.npcName = npcName;
            npc.id = id;
            npc.displayName = displayName;
            npc.modelId = modelId;

            EditorUtility.SetDirty(npc);
            created++;
        }

        //Set edited SO assets as dirty (edited), trigger save assets so unity writes dirty files
        AssetDatabase.SaveAssets();
        Debug.Log($"NpcImporter: Created/updated {created} NPC assets.");
    }
    #endregion




    #region Dialogue Pool
    private const string DIALOGUE_POOL_ASSET = "Assets/Resources/SO/Asset/DialoguePoolDatabaseSO.asset";
    private const string DIALOGUE_POOL_CSV = "CsvDatabase/DIALOGUE_POOL";

    [MenuItem("Tools/ImportCsvToDatabase/Dialogue Pools")]
    public static void ImportDialoguePools()
    {
        //Load CSV file into script
        TextAsset csv = Resources.Load<TextAsset>(DIALOGUE_POOL_CSV);
        if (csv == null)
        {
            Debug.LogError("CSV not found at Resources/" + DIALOGUE_POOL_CSV);
            return;
        }

        //Check for SO asset, if none create a new one. Regardless of SO asset presence, clear the list before write.
        DialoguePoolDatabaseSO databaseSO = AssetDatabase.LoadAssetAtPath<DialoguePoolDatabaseSO>(DIALOGUE_POOL_ASSET);
        if (databaseSO == null)
        {
            databaseSO = ScriptableObject.CreateInstance<DialoguePoolDatabaseSO>();
            AssetDatabase.CreateAsset(databaseSO, DIALOGUE_POOL_ASSET);
        }
        databaseSO.entries.Clear();

        //Split CSV file into lines
        string[] lines = csv.text.Split('\n');
        int poolCounter = 1;

        for (int i = 0; i < lines.Length; i++)
        {
            //Trim blank spaces at start and end. Check if there is data left.
            string line = lines[i].Trim().TrimEnd(',');
            if (string.IsNullOrEmpty(line)) continue;

            //Further split each line into fields
            string[] fields = line.Split(',');
            if (fields.Length < 5) continue;

            //During is a bunch of string separated by "/" in field 5, split them and add to a list
            List<string> duringList = new List<string>();
            if (fields.Length >= 5 && !string.IsNullOrWhiteSpace(fields[5]))
            {
                string[] duringParts = fields[5].Split('/');
                foreach (var part in duringParts)
                {
                    string trimmed = part.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                        duringList.Add(trimmed);
                }
            }

            //Assign into SO
            databaseSO.entries.Add(new DialoguePoolEntry
            {
                poolName = fields[0].Trim(),
                poolId = fields[1].Trim(),
                transition = fields[2].Trim(),
                getOn = fields[3].Trim(),
                end = fields[4].Trim(),
                during = duringList
            });

            poolCounter++;
        }

        //Set edited SO assets as dirty (edited), trigger save assets so unity writes dirty files
        EditorUtility.SetDirty(databaseSO);
        AssetDatabase.SaveAssets();
        Debug.Log($"DialoguePoolImporter: imported {databaseSO.entries.Count} pools.");
    }
    #endregion




    #region Tags
    private const string TAGS_CSV = "CsvDatabase/TAGS";
    private const string TAGS_ASSET = "Assets/Resources/SO/Asset/TagDatabaseSO.asset";

    [MenuItem("Tools/ImportCsvToDatabase/Tags")]
    public static void ImportTags()
    {
        //Load CSV file into script
        TextAsset csv = Resources.Load<TextAsset>(TAGS_CSV);
        if (csv == null)
        {
            Debug.LogError("CSV not found at Resources/" + TAGS_CSV);
            return;
        }

        //Check for SO asset, if none create a new one. Regardless of SO asset presence, clear the list before write.
        TagDatabaseSO catalogSO = AssetDatabase.LoadAssetAtPath<TagDatabaseSO>(TAGS_ASSET);
        if (catalogSO == null)
        {
            catalogSO = ScriptableObject.CreateInstance<TagDatabaseSO>();
            AssetDatabase.CreateAsset(catalogSO, TAGS_ASSET);
        }
        catalogSO.tags.Clear();

        //Split CSV file into lines
        string[] lines = csv.text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            //Trim blank spaces at start and end. Check if there is data left.
            string line = lines[i].Trim().TrimEnd(',');
            if (string.IsNullOrEmpty(line)) continue;

            //Further split each line into fields, tag is always column B
            string[] fields = line.Split(',');
            string tag = fields.Length > 1 ? fields[1].Trim() : fields[0].Trim();

            //Skip empty tags
            if (string.IsNullOrEmpty(tag)) continue;

            //Avoid duplicates
            if (!catalogSO.tags.Contains(tag))
                catalogSO.tags.Add(tag);
        }

        //Set edited SO assets as dirty (edited), trigger save assets so unity writes dirty files
        EditorUtility.SetDirty(catalogSO);
        AssetDatabase.SaveAssets();
        Debug.Log($"TagImporter: Imported {catalogSO.tags.Count} tags.");
    }
    #endregion
}