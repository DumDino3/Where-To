using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DatabaseImporter
{
    private const string LOCATION_CSV = "CsvDatabase/LOCATION_ID";
    private const string LOCATION_ASSET = "Assets/-System- Ride Request/-Sub- Level Loader/Data/SO/LocationDatabaseSO.asset";

    #region Import All

    [MenuItem("Tools/ImportCsvToDatabase/All")]
    public static void ImportAll()
    {
        ImportLocations();
        ImportNPCs();
        ImportDialoguePools();
        ImportTags();
        Debug.Log("DatabaseImporter: Worked well :)");
    }
    #endregion

    #region Locations

    [MenuItem("Tools/Import/Locations")]
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
    private const string NPC_FOLDER = "Assets/-System- Ride Request/-Sub- Level Loader/Data/SO/NPC";

    [MenuItem("Tools/Import/NPCs")]
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
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"NpcImporter: No Id found on row {i}");
                continue;
            }

            //Create or update individual NpcSO asset using displayName as file name
            string assetPath = $"{NPC_FOLDER}/{displayName}.asset";
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

    #region Dialogue Pools
    private const string DIALOGUE_POOL_ASSET = "Assets/-System- Ride Request/-Sub- Level Loader/Data/SO/DialoguePoolDatabaseSO.asset";
    private const string DIALOGUE_POOL_CSV = "CsvDatabase/DIAG_TITLE_POOL";

    [MenuItem("Tools/Import/Dialogue Pools")]
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

            //Column A is pool name, B-E are yarn node titles
            string poolName = fields[0].Trim();
            if (string.IsNullOrEmpty(poolName))
            {
                Debug.LogWarning($"DialoguePoolImporter: No pool name found on row {i}");
                continue;
            }

            //Auto-assign pool ID as zero-padded string
            string poolId = poolCounter.ToString("D3");

            //During is a list, currently one entry per CSV row. Creator tool will allow multiple later.
            List<string> duringList = new List<string>();
            duringList.Add(fields[3].Trim());

            databaseSO.entries.Add(new DialoguePoolEntry
            {
                poolId = poolId,
                poolName = poolName,
                transition = fields[1].Trim(),
                getOn = fields[2].Trim(),
                during = duringList,
                end = fields[4].Trim()
            });

            poolCounter++;
        }

        //Set edited SO assets as dirty (edited), trigger save assets so unity writes dirty files
        EditorUtility.SetDirty(databaseSO);
        AssetDatabase.SaveAssets();
        Debug.Log($"DialoguePoolImporter: Imported {databaseSO.entries.Count} pools.");
    }
    #endregion

    #region Tags
    private const string TAGS_CSV = "CsvDatabase/TAGS";
    private const string TAGS_ASSET = "Assets/-System- Ride Request/-Sub- Level Loader/Data/SO/TagDatabaseSO.asset";

    [MenuItem("Tools/Import/Tags")]
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