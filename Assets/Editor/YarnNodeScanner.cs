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

        HashSet<string> titleList = new HashSet<string>();

        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning($"YarnNodeScanner: Folder not found at '{folderPath}'");
            return titleList;
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

                //The lion only concerns himself with lines starting with "title:"
                if (!trimmed.StartsWith("title:")) continue;

                //Trim the first 6 letter, which is "title:".
                //Trim blank spaces one more time and take the rest as actual title
                string nodeTitle = trimmed.Substring(6).Trim();
                if (string.IsNullOrEmpty(nodeTitle)) continue;

                //Check for duplicate before adding
                if (!titleList.Add(nodeTitle))
                    Debug.LogError($"YarnNodeScanner: Duplicate '{nodeTitle}' found in '{filePath}'");
            }
        }

        return titleList;
    }

    //Quick check if a title exists
    public static bool NodeExists(string nodeTitle, HashSet<string> titleList)
    {
        return titleList.Contains(nodeTitle);
    }
}