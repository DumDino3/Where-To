using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RequestCreatorWindow : EditorWindow
{
    #region Database references
    private RideRequestDatabaseSO requestDb;
    private ConditionDatabaseSO conditionDb;
    private DialoguePoolDatabaseSO dialoguePoolDb;
    private LocationDatabaseSO locationDb;
    private TagDatabaseSO tagDb;
    #endregion

    #region Request fields
    private string requestName = "";
    private int npcIndex = 0;
    private int spawnIndex = 0;
    private int destinationIndex = 0;
    private string duration = "005";
    private string priority = "01";
    #endregion

    #region Condition fields
    private int includeTagAddIndex = 0;
    private int excludeTagAddIndex = 0;
    private bool createNewCondition = true;
    private int existingConditionIndex = 0;
    private List<string> includeTags = new List<string>();
    private List<string> excludeTags = new List<string>();
    #endregion

    #region Dialogue Pool fields
    private bool createNewPool = true;
    private int existingPoolIndex = 0;
    private string poolName = "";
    private string transition = "";
    private string getOn = "";
    private List<string> during = new List<string>();
    private string end = "";
    #endregion

    #region Yarn validation
    private HashSet<string> knownYarnNodes = new HashSet<string>();
    private bool hasScannedYarn = false;
    #endregion

    #region NPC cache
    private NpcSO[] allNpcs;
    #endregion

    #region Scroll
    private Vector2 scrollPos;
    #endregion

    [MenuItem("Tools/Request Creator")]
    public static void ShowWindow()
    {
        GetWindow<RequestCreatorWindow>("Request Creator");
    }

    private void OnEnable()
    {
        RefreshNpcCache();
    }

    #region Refresh NPC cache from project assets
    private void RefreshNpcCache()
    {
        string[] guids = AssetDatabase.FindAssets("t:NpcSO");
        allNpcs = new NpcSO[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            allNpcs[i] = AssetDatabase.LoadAssetAtPath<NpcSO>(path);
        }
    }
    #endregion

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.LabelField("REQUEST CREATOR", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        #region Database references UI
        EditorGUILayout.LabelField("── DATABASES ──", EditorStyles.boldLabel);
        requestDb = (RideRequestDatabaseSO)EditorGUILayout.ObjectField("Request DB", requestDb, typeof(RideRequestDatabaseSO), false);
        conditionDb = (ConditionDatabaseSO)EditorGUILayout.ObjectField("Condition DB", conditionDb, typeof(ConditionDatabaseSO), false);
        dialoguePoolDb = (DialoguePoolDatabaseSO)EditorGUILayout.ObjectField("Dialogue Pool DB", dialoguePoolDb, typeof(DialoguePoolDatabaseSO), false);
        locationDb = (LocationDatabaseSO)EditorGUILayout.ObjectField("Location DB", locationDb, typeof(LocationDatabaseSO), false);
        tagDb = (TagDatabaseSO)EditorGUILayout.ObjectField("Tag DB", tagDb, typeof(TagDatabaseSO), false);
        EditorGUILayout.Space(10);

        //Check all databases are assigned before drawing the rest
        if (requestDb == null || conditionDb == null || dialoguePoolDb == null || locationDb == null || tagDb == null)
        {
            EditorGUILayout.HelpBox("Assign all databases above to continue.", MessageType.Warning);
            EditorGUILayout.EndScrollView();
            return;
        }
        #endregion

        DrawRequestSection();
        EditorGUILayout.Space(10);
        DrawConditionSection();
        EditorGUILayout.Space(10);
        DrawDialoguePoolSection();
        EditorGUILayout.Space(20);

        #region Create button
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("CREATE", GUILayout.Height(30)))
        {
            CreateEntries();
        }
        GUI.backgroundColor = Color.white;
        #endregion

        EditorGUILayout.EndScrollView();
    }

    #region Request Section
    private void DrawRequestSection()
    {
        EditorGUILayout.LabelField("── REQUEST ──", EditorStyles.boldLabel);

        //Auto-assigned ID preview
        string nextRequestId = (requestDb.entries.Count + 1).ToString("D3");
        EditorGUILayout.LabelField("ID:", nextRequestId);

        requestName = EditorGUILayout.TextField("Name:", requestName);

        //NPC dropdown from NpcSO assets
        if (allNpcs != null && allNpcs.Length > 0)
        {
            string[] npcLabels = new string[allNpcs.Length];
            for (int i = 0; i < allNpcs.Length; i++)
                npcLabels[i] = $"{allNpcs[i].id} - {allNpcs[i].displayName}";
            npcIndex = EditorGUILayout.Popup("NPC:", npcIndex, npcLabels);
        }
        else
        {
            EditorGUILayout.HelpBox("No NpcSO assets found in project.", MessageType.Warning);
        }

        //Spawn dropdown from LocationDatabase
        List<LocationEntry> locations = locationDb.GetAll();
        if (locations.Count > 0)
        {
            string[] locationLabels = new string[locations.Count];
            for (int i = 0; i < locations.Count; i++)
                locationLabels[i] = $"{locations[i].id} - {locations[i].name}";
            spawnIndex = EditorGUILayout.Popup("Spawn:", spawnIndex, locationLabels);
            destinationIndex = EditorGUILayout.Popup("Destination:", destinationIndex, locationLabels);
        }
        else
        {
            EditorGUILayout.HelpBox("LocationDatabase is empty.", MessageType.Warning);
        }

        duration = EditorGUILayout.TextField("Duration:", duration);
        priority = EditorGUILayout.TextField("Priority:", priority);
    }
    #endregion

    #region Condition Section
    private void DrawConditionSection()
    {
        EditorGUILayout.LabelField("── CONDITION ──", EditorStyles.boldLabel);

        //Toggle between create new or use existing
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Toggle(createNewCondition, "Create New", EditorStyles.miniButtonLeft))
            createNewCondition = true;
        if (GUILayout.Toggle(!createNewCondition, "Use Existing", EditorStyles.miniButtonRight))
            createNewCondition = false;
        EditorGUILayout.EndHorizontal();

        if (!createNewCondition)
        {
            //Dropdown of existing conditions
            List<ConditionEntry> conditions = conditionDb.GetAll();
            if (conditions.Count > 0)
            {
                string[] conditionLabels = new string[conditions.Count];
                for (int i = 0; i < conditions.Count; i++)
                    conditionLabels[i] = $"{conditions[i].conditionId} - Include: {string.Join(", ", conditions[i].includeTags)} | Exclude: {string.Join(", ", conditions[i].excludeTags)}";
                existingConditionIndex = EditorGUILayout.Popup("Condition:", existingConditionIndex, conditionLabels);
            }
            else
            {
                EditorGUILayout.HelpBox("No conditions exist yet. Create one.", MessageType.Info);
                createNewCondition = true;
            }
        }
        else
        {
            //Auto-assigned ID preview
            string nextConditionId = (conditionDb.entries.Count + 1).ToString("D3");
            EditorGUILayout.LabelField("ID:", nextConditionId);

            //Include tags
            EditorGUILayout.LabelField("Include Tags:");
            DrawTagList(includeTags, ref includeTagAddIndex);

            //Exclude tags
            EditorGUILayout.LabelField("Exclude Tags:");
            DrawTagList(excludeTags, ref excludeTagAddIndex);
        }
    }
    #endregion

    #region Tag list drawer
    private void DrawTagList(List<string> tagList, ref int addIndex)
    {
        List<string> allTags = tagDb.tags;
        if (allTags.Count == 0)
        {
            EditorGUILayout.HelpBox("TagDatabase is empty.", MessageType.Warning);
            return;
        }

        //Draw existing tags with remove buttons
        for (int i = tagList.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(tagList[i]);
            if (GUILayout.Button("x", GUILayout.Width(20)))
                tagList.RemoveAt(i);
            EditorGUILayout.EndHorizontal();
        }

        //Add tag dropdown
        EditorGUILayout.BeginHorizontal();

        //Build filtered list (exclude already selected tags)
        List<string> available = new List<string>();
        foreach (string tag in allTags)
        {
            if (!tagList.Contains(tag))
                available.Add(tag);
        }

        if (available.Count > 0)
        {
            string[] availableLabels = available.ToArray();
            // Clamp in case the list shrank
            if (addIndex >= available.Count) addIndex = 0;
            addIndex = EditorGUILayout.Popup(addIndex, availableLabels);
            if (GUILayout.Button("+ Add", GUILayout.Width(50)))
                tagList.Add(available[addIndex]);
        }
        else
        {
            EditorGUILayout.LabelField("All tags used.");
        }

        EditorGUILayout.EndHorizontal();
    }
    #endregion

    #region Dialogue Pool Section
    private void DrawDialoguePoolSection()
    {
        EditorGUILayout.LabelField("── DIALOGUE POOL ──", EditorStyles.boldLabel);

        //Toggle between create new or use existing
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Toggle(createNewPool, "Create New", EditorStyles.miniButtonLeft))
            createNewPool = true;
        if (GUILayout.Toggle(!createNewPool, "Use Existing", EditorStyles.miniButtonRight))
            createNewPool = false;
        EditorGUILayout.EndHorizontal();

        if (!createNewPool)
        {
            //Dropdown of existing pools
            List<DialoguePoolEntry> pools = dialoguePoolDb.GetAll();
            if (pools.Count > 0)
            {
                string[] poolLabels = new string[pools.Count];
                for (int i = 0; i < pools.Count; i++)
                    poolLabels[i] = $"{pools[i].poolId} - {pools[i].poolName}";
                existingPoolIndex = EditorGUILayout.Popup("Pool:", existingPoolIndex, poolLabels);
            }
            else
            {
                EditorGUILayout.HelpBox("No dialogue pools exist yet. Create one.", MessageType.Info);
                createNewPool = true;
            }
        }
        else
        {
            //Auto-assigned ID preview
            string nextPoolId = (dialoguePoolDb.entries.Count + 1).ToString("D3");
            EditorGUILayout.LabelField("ID:", nextPoolId);

            poolName = EditorGUILayout.TextField("Pool Name:", poolName);

            //Yarn node title fields with validation indicators
            DrawYarnField("Transition:", ref transition);
            DrawYarnField("GetOn:", ref getOn);

            //During list (multiple yarn nodes)
            EditorGUILayout.LabelField("During:");
            for (int i = during.Count - 1; i >= 0; i--)
            {
                EditorGUILayout.BeginHorizontal();
                string val = during[i];
                DrawYarnField("", ref val);
                during[i] = val;
                if (GUILayout.Button("x", GUILayout.Width(20)))
                    during.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+ Add During Node"))
                during.Add("");

            DrawYarnField("End:", ref end);

            //Scan button
            EditorGUILayout.Space(5);
            if (GUILayout.Button("Validate Yarn Nodes"))
            {
                knownYarnNodes = YarnNodeScanner.ScanFolder();
                hasScannedYarn = true;
            }
        }
    }
    #endregion

    #region Yarn field drawer
    private void DrawYarnField(string label, ref string value)
    {
        EditorGUILayout.BeginHorizontal();

        if (!string.IsNullOrEmpty(label))
            value = EditorGUILayout.TextField(label, value);
        else
            value = EditorGUILayout.TextField(value);

        //Show validation status if scan has been performed
        if (hasScannedYarn && !string.IsNullOrEmpty(value))
        {
            if (YarnNodeScanner.NodeExists(value, knownYarnNodes))
                EditorGUILayout.LabelField("✅", GUILayout.Width(25));
            else
                EditorGUILayout.LabelField("⚠️", GUILayout.Width(25));
        }

        EditorGUILayout.EndHorizontal();
    }
    #endregion

    #region Create entries into databases
    private void CreateEntries()
    {
        //Validate request name
        if (string.IsNullOrEmpty(requestName))
        {
            Debug.LogWarning("RequestCreator: Request name is empty.");
            return;
        }

        //Resolve or create condition
        string conditionId;
        if (createNewCondition)
        {
            conditionId = (conditionDb.entries.Count + 1).ToString("D3");
            conditionDb.entries.Add(new ConditionEntry
            {
                conditionId = conditionId,
                includeTags = new List<string>(includeTags),
                excludeTags = new List<string>(excludeTags)
            });
            EditorUtility.SetDirty(conditionDb);
        }
        else
        {
            conditionId = conditionDb.entries[existingConditionIndex].conditionId;
        }

        //Resolve or create dialogue pool
        string dialoguePoolId;
        if (createNewPool)
        {
            //Validate pool name
            if (string.IsNullOrEmpty(poolName))
            {
                Debug.LogWarning("RequestCreator: Pool name is empty.");
                return;
            }

            dialoguePoolId = (dialoguePoolDb.entries.Count + 1).ToString("D3");
            dialoguePoolDb.entries.Add(new DialoguePoolEntry
            {
                poolId = dialoguePoolId,
                poolName = poolName,
                transition = transition,
                getOn = getOn,
                during = new List<string>(during),
                end = end
            });
            EditorUtility.SetDirty(dialoguePoolDb);
        }
        else
        {
            dialoguePoolId = dialoguePoolDb.entries[existingPoolIndex].poolId;
        }

        //Create ride request entry
        string requestId = (requestDb.entries.Count + 1).ToString("D3");
        List<LocationEntry> locations = locationDb.GetAll();

        requestDb.entries.Add(new RideRequestEntry
        {
            requestId = requestId,
            requestName = requestName,
            npcId = allNpcs[npcIndex].id,
            spawnId = locations[spawnIndex].id,
            destinationId = locations[destinationIndex].id,
            duration = duration,
            priority = priority,
            conditionId = conditionId,
            dialoguePoolId = dialoguePoolId
        });
        EditorUtility.SetDirty(requestDb);

        //Save all dirty assets
        AssetDatabase.SaveAssets();
        Debug.Log($"RequestCreator: Created request '{requestName}' (ID: {requestId}, Condition: {conditionId}, Pool: {dialoguePoolId})");
    }
    #endregion

    #region Reset fields after creation
    private void ResetFields()
    {
        requestName = "";
        npcIndex = 0;
        spawnIndex = 0;
        destinationIndex = 0;
        duration = "005";
        priority = "01";

        createNewCondition = true;
        existingConditionIndex = 0;
        includeTagAddIndex = 0;
        excludeTagAddIndex = 0;
        includeTags.Clear();
        excludeTags.Clear();

        createNewPool = true;
        existingPoolIndex = 0;
        poolName = "";
        transition = "";
        getOn = "";
        during.Clear();
        end = "";

        hasScannedYarn = false;
    }
    #endregion
}