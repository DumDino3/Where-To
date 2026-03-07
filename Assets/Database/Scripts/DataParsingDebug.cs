using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class DataParsingDebug: MonoBehaviour
{
    [SerializeField] private DictionaryType dictionaryType = DictionaryType.Request;

    private enum DictionaryType
    {
        Request
    }
    
    public void CheckId(string id)
    {
        Debug.Log(DataParser.GetRideRequest(id));
    }
}
