using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class SpawnPointEditor : EditorWindow
{
    [SerializeField] private GameObject spawnPointPrefab;
    [SerializeField] private Transform spawnPointsParent;
    private bool isSpawning = false;
    private GameObject previewObject;
    
    [MenuItem("Tools/Spawn Point Editor")]
    public static void ShowWindow()
    {
        SpawnPointEditor window = GetWindow<SpawnPointEditor>();
        window.titleContent = new GUIContent("Spawn Point Editor");
        window.Show();
    }
    
    private void CreateGUI()
    {
        var root = rootVisualElement;
        
        // Create UI elements
        var prefabField = new ObjectField("Spawn Point Prefab")
        {
            objectType = typeof(GameObject),
            value = spawnPointPrefab
        };
        
        prefabField.RegisterValueChangedCallback(evt =>
        {
            spawnPointPrefab = evt.newValue as GameObject;
        });
        
        // Parent field
        var parentField = new ObjectField("Spawn Points Parent")
        {
            objectType = typeof(Transform),
            value = spawnPointsParent
        };
        
        parentField.RegisterValueChangedCallback(evt =>
        {
            spawnPointsParent = evt.newValue as Transform;
            Debug.Log($"Parent set to: {spawnPointsParent?.name ?? "null"}");
        });
        
        // Auto-assign button
        var autoAssignButton = new Button(() => AutoAssignDirectorParent())
        {
            text = "Auto-Assign Director Parent"
        };
        
        var createButton = new Button(() => ToggleSpawnMode())
        {
            text = "Create SpawnPoint"
        };
        
        var cancelButton = new Button(() => CancelSpawnMode())
        {
            text = "Cancel"
        };
        
        // Add to root
        root.Add(prefabField);
        root.Add(parentField);
        root.Add(autoAssignButton);
        root.Add(createButton);
        root.Add(cancelButton);
        
        // Simple styling
        createButton.style.height = 30;
        cancelButton.style.height = 30;
        autoAssignButton.style.height = 25;
        createButton.style.marginTop = 10;
        cancelButton.style.marginTop = 5;
        autoAssignButton.style.marginTop = 5;
        autoAssignButton.style.marginBottom = 10;
    }
    
    private void AutoAssignDirectorParent()
    {
        SpawnPointsDirector director = FindObjectOfType<SpawnPointsDirector>();
        if (director != null)
        {
            spawnPointsParent = director.SpawnPointsParent;
            
            // Update the UI field
            var fields = rootVisualElement.Query<ObjectField>().ToList();
            foreach (var field in fields)
            {
                if (field.label == "Spawn Points Parent")
                {
                    field.value = spawnPointsParent;
                    break;
                }
            }
            
            Debug.Log($"Auto-assigned parent: {spawnPointsParent?.name ?? "null"}");
        }
        else
        {
            Debug.LogWarning("No SpawnPointsDirector found in scene!");
        }
    }
    
    private void ToggleSpawnMode()
    {
        if (spawnPointPrefab == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a prefab first!", "OK");
            return;
        }
        
        isSpawning = !isSpawning;
        
        if (isSpawning)
        {
            SceneView.duringSceneGui += OnSceneGUI;
            CreatePreviewObject();
        }
        else
        {
            CancelSpawnMode();
        }
    }
    
    private void CancelSpawnMode()
    {
        isSpawning = false;
        SceneView.duringSceneGui -= OnSceneGUI;
        DestroyPreviewObject();
        SceneView.RepaintAll();
    }
    
    private void CreatePreviewObject()
    {
        if (previewObject == null && spawnPointPrefab != null)
        {
            previewObject = Instantiate(spawnPointPrefab);
            previewObject.name = "SpawnPoint_Preview";
            
            // Make it semi-transparent for preview
            var renderers = previewObject.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                var materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    var material = new Material(materials[i]);
                    if (material.HasProperty("_Color"))
                    {
                        var color = material.color;
                        color.a = 0.5f;
                        material.color = color;
                    }
                    materials[i] = material;
                }
                renderer.materials = materials;
            }
        }
    }
    
    private void DestroyPreviewObject()
    {
        if (previewObject != null)
        {
            DestroyImmediate(previewObject);
            previewObject = null;
        }
    }
    
    private void OnSceneGUI(SceneView sceneView)
    {
        if (!isSpawning) return;
        
        Event e = Event.current;
        
        if (e.type == EventType.MouseMove)
        {
            UpdatePreviewPosition(e.mousePosition, sceneView);
            e.Use();
        }
        
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            PlaceSpawnPoint(e.mousePosition, sceneView);
            e.Use();
        }
        
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            CancelSpawnMode();
            e.Use();
        }
    }
    
    private void UpdatePreviewPosition(Vector2 mousePosition, SceneView sceneView)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (previewObject != null)
            {
                previewObject.transform.position = hit.point;
                previewObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                previewObject.SetActive(true);
            }
        }
        else
        {
            if (previewObject != null)
            {
                previewObject.SetActive(false);
            }
        }
        
        sceneView.Repaint();
    }
    
    private void PlaceSpawnPoint(Vector2 mousePosition, SceneView sceneView)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Create the actual spawn point
            GameObject spawnPoint = PrefabUtility.InstantiatePrefab(spawnPointPrefab) as GameObject;
            spawnPoint.transform.position = hit.point;
            spawnPoint.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            
            // Ensure the prefab has SpawnPoint component
            SpawnPoint spawnPointComponent = spawnPoint.GetComponent<SpawnPoint>();
            if (spawnPointComponent == null)
            {
                spawnPointComponent = spawnPoint.AddComponent<SpawnPoint>();
            }
            
            // Set parent BEFORE registering (if available)
            if (spawnPointsParent != null)
            {
                spawnPoint.transform.SetParent(spawnPointsParent);
            }
            
            // Register undo
            Undo.RegisterCreatedObjectUndo(spawnPoint, "Create Spawn Point");
            
            // Select the new object
            Selection.activeGameObject = spawnPoint;
            
            Debug.Log($"Spawn point created at {hit.point}");
            sceneView.Repaint();
        }
    }
    
    private void OnDisable()
    {
        CancelSpawnMode();
    }
}