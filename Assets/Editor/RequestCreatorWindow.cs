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
    //private const string DefaultPriority = "01";

    private bool createNewRequest = true;
    private int existingRequestIndex = 0;
    private int loadedRequestIndex = -1;

    private string requestName = "";
    private int npcIndex = 0;
    private int spawnIndex = 0;
    private int destinationIndex = 0;
    private string duration = "005";
    #endregion

    #region Condition fields
    private int includeTagAddIndex = 0;
    private int excludeTagAddIndex = 0;
    private bool createNewCondition = true;
    private int existingConditionIndex = 0;
    private int loadedConditionIndex = -1;
    private List<string> includeTags = new List<string>();
    private List<string> excludeTags = new List<string>();
    #endregion

    #region Dialogue Pool fields
    private bool createNewPool = true;
    private int existingPoolIndex = 0;
    private int loadedPoolIndex = -1;
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

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Toggle(createNewRequest, "Create New", EditorStyles.miniButtonLeft) && !createNewRequest)
            SwitchRequestMode(true);
        if (GUILayout.Toggle(!createNewRequest, "Use Existing", EditorStyles.miniButtonRight) && createNewRequest)
            SwitchRequestMode(false);
        EditorGUILayout.EndHorizontal();

        if (createNewRequest)
        {
            string nextRequestId = (requestDb.entries.Count + 1).ToString("D3");
            EditorGUILayout.LabelField("ID:", nextRequestId);
        }
        else
        {
            List<RideRequestEntry> requests = requestDb.GetAll();
            if (requests.Count > 0)
            {
                existingRequestIndex = ClampIndex(existingRequestIndex, requests.Count);
                int newIndex = EditorGUILayout.Popup("Request:", existingRequestIndex, BuildRequestLabels(requests));
                if (newIndex != existingRequestIndex)
                    existingRequestIndex = newIndex;

                if (loadedRequestIndex != existingRequestIndex)
                    LoadRequestSelection(existingRequestIndex);

                EditorGUILayout.LabelField("ID:", requests[existingRequestIndex].requestId);
            }
            else
            {
                EditorGUILayout.HelpBox("No requests exist yet. Create one.", MessageType.Info);
                SwitchRequestMode(true);
            }
        }

        requestName = EditorGUILayout.TextField("Name:", requestName);

        if (allNpcs != null && allNpcs.Length > 0)
        {
            npcIndex = ClampIndex(npcIndex, allNpcs.Length);
            npcIndex = EditorGUILayout.Popup("NPC:", npcIndex, BuildNpcLabels());
        }
        else
        {
            EditorGUILayout.HelpBox("No NpcSO assets found in project.", MessageType.Warning);
        }

        List<LocationEntry> locations = locationDb.GetAll();
        if (locations.Count > 0)
        {
            spawnIndex = ClampIndex(spawnIndex, locations.Count);
            destinationIndex = ClampIndex(destinationIndex, locations.Count);
            string[] locationLabels = BuildLocationLabels(locations);
            spawnIndex = EditorGUILayout.Popup("Spawn:", spawnIndex, locationLabels);
            destinationIndex = EditorGUILayout.Popup("Destination:", destinationIndex, locationLabels);
        }
        else
        {
            EditorGUILayout.HelpBox("LocationDatabase is empty.", MessageType.Warning);
        }

        duration = EditorGUILayout.TextField("Duration:", duration);
    }

    private void SwitchRequestMode(bool toCreateNew)
    {
        createNewRequest = toCreateNew;
        loadedRequestIndex = -1;

        if (toCreateNew)
        {
            requestName = "";
            duration = "005";
            npcIndex = 0;
            spawnIndex = 0;
            destinationIndex = 0;
        }
        else
        {
            LoadRequestSelection(existingRequestIndex);
        }
    }

    private void LoadRequestSelection(int index)
    {
        List<RideRequestEntry> requests = requestDb.GetAll();
        if (requests.Count == 0)
            return;

        existingRequestIndex = ClampIndex(index, requests.Count);
        RideRequestEntry request = requests[existingRequestIndex];
        loadedRequestIndex = existingRequestIndex;

        requestName = request.requestName;
        duration = request.duration;
        npcIndex = FindNpcIndexById(request.npcId);

        List<LocationEntry> locations = locationDb.GetAll();
        spawnIndex = FindLocationIndexById(locations, request.spawnId);
        destinationIndex = FindLocationIndexById(locations, request.destinationId);

        int conditionIndex = FindConditionIndexById(request.conditionId);
        if (conditionIndex >= 0)
        {
            createNewCondition = false;
            existingConditionIndex = conditionIndex;
            loadedConditionIndex = -1;
            LoadConditionSelection(existingConditionIndex);
        }
        else
        {
            createNewCondition = true;
            loadedConditionIndex = -1;
            includeTags.Clear();
            excludeTags.Clear();
        }

        int poolIndex = FindPoolIndexById(request.dialoguePoolId);
        if (poolIndex >= 0)
        {
            createNewPool = false;
            existingPoolIndex = poolIndex;
            loadedPoolIndex = -1;
            LoadPoolSelection(existingPoolIndex);
        }
        else
        {
            createNewPool = true;
            loadedPoolIndex = -1;
            poolName = "";
            transition = "";
            getOn = "";
            during.Clear();
            end = "";
        }
    }
    #endregion

    #region Condition Section
    private void DrawConditionSection()
    {
        EditorGUILayout.LabelField("── CONDITION ──", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Toggle(createNewCondition, "Create New", EditorStyles.miniButtonLeft) && !createNewCondition)
            SwitchConditionMode(true);
        if (GUILayout.Toggle(!createNewCondition, "Use Existing", EditorStyles.miniButtonRight) && createNewCondition)
            SwitchConditionMode(false);
        EditorGUILayout.EndHorizontal();

        if (!createNewCondition)
        {
            List<ConditionEntry> conditions = conditionDb.GetAll();
            if (conditions.Count > 0)
            {
                existingConditionIndex = ClampIndex(existingConditionIndex, conditions.Count);
                int newIndex = EditorGUILayout.Popup("Condition:", existingConditionIndex, BuildConditionLabels(conditions));
                if (newIndex != existingConditionIndex)
                    existingConditionIndex = newIndex;

                if (loadedConditionIndex != existingConditionIndex)
                    LoadConditionSelection(existingConditionIndex);

                EditorGUILayout.LabelField("ID:", conditions[existingConditionIndex].conditionId);

                EditorGUILayout.LabelField("Include Tags:");
                DrawTagList(includeTags, ref includeTagAddIndex);

                EditorGUILayout.LabelField("Exclude Tags:");
                DrawTagList(excludeTags, ref excludeTagAddIndex);
            }
            else
            {
                EditorGUILayout.HelpBox("No conditions exist yet. Create one.", MessageType.Info);
                SwitchConditionMode(true);
            }
        }
        else
        {
            string nextConditionId = (conditionDb.entries.Count + 1).ToString("D3");
            EditorGUILayout.LabelField("ID:", nextConditionId);

            EditorGUILayout.LabelField("Include Tags:");
            DrawTagList(includeTags, ref includeTagAddIndex);

            EditorGUILayout.LabelField("Exclude Tags:");
            DrawTagList(excludeTags, ref excludeTagAddIndex);
        }
    }

    private void SwitchConditionMode(bool toCreateNew)
    {
        createNewCondition = toCreateNew;
        loadedConditionIndex = -1;
        includeTagAddIndex = 0;
        excludeTagAddIndex = 0;

        if (toCreateNew)
        {
            includeTags.Clear();
            excludeTags.Clear();
        }
        else
        {
            LoadConditionSelection(existingConditionIndex);
        }
    }

    private void LoadConditionSelection(int index)
    {
        List<ConditionEntry> conditions = conditionDb.GetAll();
        if (conditions.Count == 0)
            return;

        existingConditionIndex = ClampIndex(index, conditions.Count);
        ConditionEntry condition = conditions[existingConditionIndex];
        loadedConditionIndex = existingConditionIndex;

        includeTags = condition.includeTags != null ? new List<string>(condition.includeTags) : new List<string>();
        excludeTags = condition.excludeTags != null ? new List<string>(condition.excludeTags) : new List<string>();
        includeTagAddIndex = 0;
        excludeTagAddIndex = 0;
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

        for (int i = tagList.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(tagList[i]);
            if (GUILayout.Button("x", GUILayout.Width(20)))
                tagList.RemoveAt(i);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();

        List<string> available = new List<string>();
        foreach (string tag in allTags)
        {
            if (!tagList.Contains(tag))
                available.Add(tag);
        }

        if (available.Count > 0)
        {
            if (addIndex >= available.Count)
                addIndex = 0;

            addIndex = EditorGUILayout.Popup(addIndex, available.ToArray());
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

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Toggle(createNewPool, "Create New", EditorStyles.miniButtonLeft) && !createNewPool)
            SwitchPoolMode(true);
        if (GUILayout.Toggle(!createNewPool, "Use Existing", EditorStyles.miniButtonRight) && createNewPool)
            SwitchPoolMode(false);
        EditorGUILayout.EndHorizontal();

        if (!createNewPool)
        {
            List<DialoguePoolEntry> pools = dialoguePoolDb.GetAll();
            if (pools.Count > 0)
            {
                existingPoolIndex = ClampIndex(existingPoolIndex, pools.Count);
                int newIndex = EditorGUILayout.Popup("Pool:", existingPoolIndex, BuildPoolLabels(pools));
                if (newIndex != existingPoolIndex)
                    existingPoolIndex = newIndex;

                if (loadedPoolIndex != existingPoolIndex)
                    LoadPoolSelection(existingPoolIndex);

                EditorGUILayout.LabelField("ID:", pools[existingPoolIndex].poolId);
                DrawPoolEditorFields();
            }
            else
            {
                EditorGUILayout.HelpBox("No dialogue pools exist yet. Create one.", MessageType.Info);
                SwitchPoolMode(true);
            }
        }
        else
        {
            string nextPoolId = (dialoguePoolDb.entries.Count + 1).ToString("D3");
            EditorGUILayout.LabelField("ID:", nextPoolId);
            DrawPoolEditorFields();
        }
    }

    private void SwitchPoolMode(bool toCreateNew)
    {
        createNewPool = toCreateNew;
        loadedPoolIndex = -1;

        if (toCreateNew)
        {
            poolName = "";
            transition = "";
            getOn = "";
            during.Clear();
            end = "";
        }
        else
        {
            LoadPoolSelection(existingPoolIndex);
        }
    }

    private void DrawPoolEditorFields()
    {
        poolName = EditorGUILayout.TextField("Pool Name:", poolName);

        DrawYarnField("Transition:", ref transition);
        DrawYarnField("GetOn:", ref getOn);

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

        EditorGUILayout.Space(5);
        if (GUILayout.Button("Validate Yarn Nodes"))
        {
            knownYarnNodes = YarnNodeScanner.ScanFolder();
            hasScannedYarn = true;
        }
    }

    private void LoadPoolSelection(int index)
    {
        List<DialoguePoolEntry> pools = dialoguePoolDb.GetAll();
        if (pools.Count == 0)
            return;

        existingPoolIndex = ClampIndex(index, pools.Count);
        DialoguePoolEntry pool = pools[existingPoolIndex];
        loadedPoolIndex = existingPoolIndex;

        poolName = pool.poolName;
        transition = pool.transition;
        getOn = pool.getOn;
        during = pool.during != null ? new List<string>(pool.during) : new List<string>();
        end = pool.end;
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
        if (string.IsNullOrEmpty(requestName))
        {
            Debug.LogWarning("RequestCreator: Request name is empty.");
            return;
        }

        if (allNpcs == null || allNpcs.Length == 0)
        {
            Debug.LogWarning("RequestCreator: No NPCs available.");
            return;
        }

        List<LocationEntry> locations = locationDb.GetAll();
        if (locations.Count == 0)
        {
            Debug.LogWarning("RequestCreator: LocationDatabase is empty.");
            return;
        }

        npcIndex = ClampIndex(npcIndex, allNpcs.Length);
        spawnIndex = ClampIndex(spawnIndex, locations.Count);
        destinationIndex = ClampIndex(destinationIndex, locations.Count);

        string conditionId;
        string conditionAction;
        if (createNewCondition)
        {
            conditionId = (conditionDb.entries.Count + 1).ToString("D3");
            conditionDb.entries.Add(new ConditionEntry
            {
                conditionId = conditionId,
                includeTags = new List<string>(includeTags),
                excludeTags = new List<string>(excludeTags)
            });
            conditionAction = "created";
        }
        else
        {
            if (conditionDb.entries.Count == 0)
            {
                Debug.LogWarning("RequestCreator: No existing condition to update.");
                return;
            }

            existingConditionIndex = ClampIndex(existingConditionIndex, conditionDb.entries.Count);
            ConditionEntry existing = conditionDb.entries[existingConditionIndex];
            conditionId = existing.conditionId;
            existing.includeTags = new List<string>(includeTags);
            existing.excludeTags = new List<string>(excludeTags);
            conditionDb.entries[existingConditionIndex] = existing;
            conditionAction = "updated";
        }
        EditorUtility.SetDirty(conditionDb);

        string dialoguePoolId;
        string poolAction;
        if (createNewPool)
        {
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
            poolAction = "created";
        }
        else
        {
            if (dialoguePoolDb.entries.Count == 0)
            {
                Debug.LogWarning("RequestCreator: No existing dialogue pool to update.");
                return;
            }

            if (string.IsNullOrEmpty(poolName))
            {
                Debug.LogWarning("RequestCreator: Pool name is empty.");
                return;
            }

            existingPoolIndex = ClampIndex(existingPoolIndex, dialoguePoolDb.entries.Count);
            DialoguePoolEntry existing = dialoguePoolDb.entries[existingPoolIndex];
            dialoguePoolId = existing.poolId;
            existing.poolName = poolName;
            existing.transition = transition;
            existing.getOn = getOn;
            existing.during = new List<string>(during);
            existing.end = end;
            dialoguePoolDb.entries[existingPoolIndex] = existing;
            poolAction = "updated";
        }
        EditorUtility.SetDirty(dialoguePoolDb);

        string requestId;
        string requestAction;
        if (createNewRequest)
        {
            requestId = (requestDb.entries.Count + 1).ToString("D3");
            requestDb.entries.Add(new RideRequestEntry
            {
                requestId = requestId,
                requestName = requestName,
                npcId = allNpcs[npcIndex].id,
                spawnId = locations[spawnIndex].id,
                destinationId = locations[destinationIndex].id,
                duration = duration,
                //priority = DefaultPriority,
                conditionId = conditionId,
                dialoguePoolId = dialoguePoolId
            });
            requestAction = "created";
        }
        else
        {
            if (requestDb.entries.Count == 0)
            {
                Debug.LogWarning("RequestCreator: No existing request to update.");
                return;
            }

            existingRequestIndex = ClampIndex(existingRequestIndex, requestDb.entries.Count);
            RideRequestEntry existing = requestDb.entries[existingRequestIndex];
            requestId = existing.requestId;
            existing.requestName = requestName;
            existing.npcId = allNpcs[npcIndex].id;
            existing.spawnId = locations[spawnIndex].id;
            existing.destinationId = locations[destinationIndex].id;
            existing.duration = duration;
            existing.conditionId = conditionId;
            existing.dialoguePoolId = dialoguePoolId;
            requestDb.entries[existingRequestIndex] = existing;
            requestAction = "updated";
        }
        EditorUtility.SetDirty(requestDb);

        AssetDatabase.SaveAssets();

        Debug.Log(
            $"RequestCreator: Request {requestAction} '{requestName}' (ID: {requestId}), " +
            $"Condition {conditionAction} ({conditionId}), Pool {poolAction} ({dialoguePoolId})."
        );

        if (!createNewRequest)
            LoadRequestSelection(existingRequestIndex);
        if (!createNewCondition)
            LoadConditionSelection(existingConditionIndex);
        if (!createNewPool)
            LoadPoolSelection(existingPoolIndex);
    }
    #endregion

    #region Helpers
    private static int ClampIndex(int index, int count)
    {
        if (count <= 0)
            return 0;
        return Mathf.Clamp(index, 0, count - 1);
    }

    private string[] BuildNpcLabels()
    {
        string[] labels = new string[allNpcs.Length];
        for (int i = 0; i < allNpcs.Length; i++)
        {
            if (allNpcs[i] == null)
                labels[i] = "NULL NPC";
            else
                labels[i] = $"{allNpcs[i].id} - {allNpcs[i].displayName}";
        }
        return labels;
    }

    private static string[] BuildLocationLabels(List<LocationEntry> locations)
    {
        string[] labels = new string[locations.Count];
        for (int i = 0; i < locations.Count; i++)
            labels[i] = $"{locations[i].id} - {locations[i].name}";
        return labels;
    }

    private static string[] BuildRequestLabels(List<RideRequestEntry> requests)
    {
        string[] labels = new string[requests.Count];
        for (int i = 0; i < requests.Count; i++)
            labels[i] = $"{requests[i].requestId} - {requests[i].requestName} ({requests[i].npcId})";
        return labels;
    }

    private static string[] BuildConditionLabels(List<ConditionEntry> conditions)
    {
        string[] labels = new string[conditions.Count];
        for (int i = 0; i < conditions.Count; i++)
            labels[i] = $"{conditions[i].conditionId} - Include: {string.Join(", ", conditions[i].includeTags)} | Exclude: {string.Join(", ", conditions[i].excludeTags)}";
        return labels;
    }

    private static string[] BuildPoolLabels(List<DialoguePoolEntry> pools)
    {
        string[] labels = new string[pools.Count];
        for (int i = 0; i < pools.Count; i++)
            labels[i] = $"{pools[i].poolId} - {pools[i].poolName}";
        return labels;
    }

    private int FindNpcIndexById(string npcId)
    {
        if (allNpcs == null || allNpcs.Length == 0)
            return 0;

        for (int i = 0; i < allNpcs.Length; i++)
        {
            if (allNpcs[i] != null && allNpcs[i].id == npcId)
                return i;
        }
        return 0;
    }

    private static int FindLocationIndexById(List<LocationEntry> locations, string locationId)
    {
        for (int i = 0; i < locations.Count; i++)
        {
            if (locations[i].id == locationId)
                return i;
        }
        return 0;
    }

    private int FindConditionIndexById(string conditionId)
    {
        List<ConditionEntry> conditions = conditionDb.GetAll();
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].conditionId == conditionId)
                return i;
        }
        return -1;
    }

    private int FindPoolIndexById(string poolId)
    {
        List<DialoguePoolEntry> pools = dialoguePoolDb.GetAll();
        for (int i = 0; i < pools.Count; i++)
        {
            if (pools[i].poolId == poolId)
                return i;
        }
        return -1;
    }
    #endregion

    #region Reset fields after creation
    private void ResetFields()
    {
        createNewRequest = true;
        existingRequestIndex = 0;
        loadedRequestIndex = -1;

        requestName = "";
        npcIndex = 0;
        spawnIndex = 0;
        destinationIndex = 0;
        duration = "005";

        createNewCondition = true;
        existingConditionIndex = 0;
        loadedConditionIndex = -1;
        includeTagAddIndex = 0;
        excludeTagAddIndex = 0;
        includeTags.Clear();
        excludeTags.Clear();

        createNewPool = true;
        existingPoolIndex = 0;
        loadedPoolIndex = -1;
        poolName = "";
        transition = "";
        getOn = "";
        during.Clear();
        end = "";

        hasScannedYarn = false;
    }
    #endregion
}