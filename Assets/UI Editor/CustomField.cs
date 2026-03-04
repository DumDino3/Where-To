using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class CustomField : VisualElement
{
    public new class UxmlFactory : UxmlFactory<CustomField, UxmlTraits> { }

    public CustomField()
    {
        // Logic to create the GameObject field automatically
        ObjectField field = new ObjectField("Spawn Point")
        {
            objectType = typeof(GameObject),
            allowSceneObjects = false
        };
        
        Add(field);
        
    }
}

public class Gamsd : VisualElement
{
    public new class UxmlFactory : UxmlFactory<Gamsd, UxmlTraits> { }

    public Gamsd()
    {
        // Logic to create the GameObject field automatically
        ObjectField field = new ObjectField("Spawn Point")
        {
            objectType = typeof(GameObject),
            allowSceneObjects = false
        };
        
        Add(field);
        
    }
}

