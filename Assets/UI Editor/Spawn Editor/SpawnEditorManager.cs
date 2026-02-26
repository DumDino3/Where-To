using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnEditorManager : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    // fields
    private LayerMaskField raycastLayerDropdownField;
    private ObjectField spawnPointPrefabField;
    private Button createSpawnPointButton;
    private Button cancelSpawnPointButton;

    // data
    private GameObject spawnPointPrefab;

    // logic
    private Vector3 spawnPointPosition;
    private Vector3 detectedFaceNormal;
    private Vector3 detectedPoint;
    private bool isPlacing = false;

    [MenuItem("Window/UI Toolkit/SpawnEditorManager")]
    public static void ShowExample()
    {
        SpawnEditorManager wnd = GetWindow<SpawnEditorManager>();
        wnd.titleContent = new GUIContent("SpawnEditorManager");
    }

    private void OnEnable()
    {
        // Called when the window is opened or scripts recompile
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        InitializeFields(root);
    }

    #region Initialization
    private void InitializeFields(VisualElement root)
    {
        raycastLayerDropdownField = root.Q<LayerMaskField>("raycastLayerDropdownField");

        spawnPointPrefabField = rootVisualElement.Q<ObjectField>("SpawnPoint");
        spawnPointPrefabField.RegisterValueChangedCallback(evt =>
            spawnPointPrefab = evt.newValue as GameObject
        );

        createSpawnPointButton = rootVisualElement.Q<Button>("CreatSpawn");
        createSpawnPointButton.clicked += CreateSpawnPoint;

        cancelSpawnPointButton = rootVisualElement.Q<Button>("Cancel");
        cancelSpawnPointButton.clicked += CancelSpawnPoint;
    }
    #endregion

    #region Event Handlers
    private void CreateSpawnPoint()
    {
        Debug.Log("Created new spawn point (placement mode ON)");
        isPlacing = true;
    }

    private void CancelSpawnPoint()
    {
        Debug.Log("Cancelled spawn point creation (placement mode OFF)");
        isPlacing = false;
    }
    #endregion

    
    private void OnSceneGUI(SceneView sceneView)
    {
        DetectNormal(isPlacing);
        placeSpawn();

        // Optionally, draw something to visualize the hit
        if (isPlacing && detectedPoint != Vector3.zero)
        {
            Handles.color = Color.green;
            Handles.DrawWireDisc(detectedPoint, detectedFaceNormal, 0.3f);
            Handles.ArrowHandleCap(
                0,
                detectedPoint,
                Quaternion.LookRotation(detectedFaceNormal),
                1f,
                EventType.Repaint
            );
        }
        
        sceneView.Repaint();
    }

    //working it ass off to detect for the face
    private void DetectNormal(bool isSpawnMode)
    {
        if (!isSpawnMode)
            return;

        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView == null || sceneView.camera == null)
            return;

        Event e = Event.current;
        if (e == null || e.type != EventType.MouseMove)
            return;

        // Fallback when layer null in editor
        int mask = ~0;
        if (raycastLayerDropdownField != null)
        {
            mask = raycastLayerDropdownField.value;
        }

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask))
        {
            spawnPointPosition = hit.point;
            detectedFaceNormal = hit.normal;
            detectedPoint = hit.point;
        }
    }

    private void placeSpawn()
    {
        Event e = Event.current;

        if (e != null && e.type == EventType.MouseDown && e.button == 0 && isPlacing)
        {
            Instantiate(spawnPointPrefab, spawnPointPosition, spawnPointPrefab.transform.rotation);
            e.Use();
            GUIUtility.hotControl = 0;
        }
    }

}