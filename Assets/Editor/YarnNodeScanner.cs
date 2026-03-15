using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class YarnNodeScanner
{
    //Default scan folder, adjust to match your project structure
    private const string DEFAULT_YARN_FOLDER = "Assets/-System- Dialogue/Dialogue";

    //Scan all .yarn files in a folder (recursive), extract node titles
    //Returns HashSet of all unique node titles found
    public static HashSet<string> ScanFolder(string folderPath = null)
    {
        if (string.IsNullOrEmpty(folderPath))
            folderPath = DEFAULT_YARN_FOLDER;

        HashSet<string> nodeSet = new HashSet<string>();

        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning($"YarnNodeScanner: Folder not found at '{folderPath}'");
            return nodeSet;
        }

        //Find all .yarn files recursively
        string[] yarnFiles = Directory.GetFiles(folderPath, "*.yarn", SearchOption.AllDirectories);

        foreach (string filePath in yarnFiles)
        {
            //Read all lines from this .yarn file
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string trimmed = line.Trim();

                //Yarn node headers use "title: nodeName"
                if (!trimmed.StartsWith("title:")) continue;

                //Extract the title value after "title:"
                string nodeTitle = trimmed.Substring(6).Trim();
                if (string.IsNullOrEmpty(nodeTitle)) continue;

                //Check for duplicate before adding
                if (!nodeSet.Add(nodeTitle))
                    Debug.LogError($"YarnNodeScanner: Duplicate node title '{nodeTitle}' found in '{filePath}'");
            }
        }

        return nodeSet;
    }

    //Quick check if a title exists
    public static bool NodeExists(string nodeTitle, HashSet<string> nodeSet)
    {
        return nodeSet.Contains(nodeTitle);
    }
}