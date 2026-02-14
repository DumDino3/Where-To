using UnityEngine;
using UnityEditor;
using System.IO;

public class NewBehaviourScript
{
    private static string npcDatabasePath = "/Database/NPCSpawnSchedule.csv";
    [MenuItem ("Tools/ImportNPCData")]
    public static void ImportNPCData()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + npcDatabasePath);
        foreach (string line in allLines)
        {
            if(line == allLines[0])
                continue;

            string[] npcData = line.Split(",");
            
            NPC npc = ScriptableObject.CreateInstance<NPC>();
            npc.npcId = npcData[0];
            npc.npcName = npcData[1];

            AssetDatabase.CreateAsset(npc, "Assets/Database/NPCs/" + npc.npcId + ".asset");
        }
        AssetDatabase.SaveAssets();
    }
}
