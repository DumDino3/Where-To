using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TagCatalogSO", menuName = "Data/Tag Catalog")]
public class TagDatabaseSO : ScriptableObject
{
    public List<string> tags = new List<string>();

    //Check if a tag exists in the catalog
    public bool IsValidTag(string tag)
    {
        return tags.Contains(tag);
    }
}