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
    }






//--------------------- RIDE REQUEST -----------------------







    private static Dictionary<string, string> requestIdDictionary = new Dictionary<string, string>();
    public static Dictionary<string, string> RequestIdDictionary { get { return requestIdDictionary; } }

    private void LoadRideRequests()
    {
        requestIdDictionary.Clear();

        // Assign the CSV
        TextAsset rideRequestCSV = Resources.Load<TextAsset>("CsvDatabase/RIDE_REQUEST");
        if(rideRequestCSV == null) {
            Debug.LogWarning("no Ride Request csv found");
            return;
        }

        //Split the CSV into single lines
        string[] rideRequestLines = rideRequestCSV.text.Split('\n');

        //Further split each line into fields
        for (int i = 0; i < rideRequestLines.Length; i++)
        {
            //Trim the blank spaces at the end and beginning of the line, then check if it there is still any text left
            string line = rideRequestLines[i].Trim();
            if (string.IsNullOrEmpty(line)){
                Debug.LogWarning("there are no lines");
                continue;
            }

            //Split by "," to separate data on each line into fields
            line = line.TrimEnd(',');
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
                continue;
            }
            string id = idKey;

            //Check the rest of the fields
            for(int e = 2; e >= 2 && e <= 6; e++)
            {
                string field = fields[e];
                if (string.IsNullOrEmpty(field) && field.Length != 3 && !int.TryParse(field, out int result)){
                    Debug.LogWarning($"Invalid ID on row {i}");
                }
                else {Debug.Log("verified duration - destination");}
            }

            for(int e = 7; e >= 7 && e <= 8; e++)
            {
                string field = fields[e];
                if (string.IsNullOrEmpty(field) && field.Length != 2 && !int.TryParse(field, out int result)){
                    Debug.LogWarning($"Invalid ID on row {i}");
                }
                else {Debug.Log("verified duration and tag");}
            }
            
            // Finally, add an key value pair to dictionary
            requestIdDictionary[id] = String.Concat(fields[2].Trim(),fields[3].Trim(),fields[4].Trim(),fields[5].Trim(),fields[6].Trim(),fields[7].Trim(),fields[8].Trim());
        }

        Debug.Log($"DataParser: Loaded {requestIdDictionary.Count} ride requests.");
        Debug.Log($"Example Id: ID = 001 and Value = {GetRideRequest("001")}");
    }

    public static string GetRideRequest(string id)
    {
        requestIdDictionary.TryGetValue(id, out string rideRequest);
        return rideRequest;
    }
}
