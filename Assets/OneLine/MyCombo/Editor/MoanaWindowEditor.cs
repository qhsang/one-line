using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class MoanaWindowEditor
{
    [MenuItem("Moana Games/Reset the game")]
    static void Reset()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        File.Delete(Application.persistentDataPath + "/data.json");
    }

    [MenuItem("Moana Games/Unlock all levels")]
    static void UnlockAllLevel()
    {
        int LEVEL_EACH_PACKAGE = LevelData.totalLevelsPerWorld;

        Dictionary<int, string> TotalLevelCrossed = new Dictionary<int, string>();
        Dictionary<int, int> currentLevel = new Dictionary<int, int>();

        string str = "";
        for(int i = 1; i <= LEVEL_EACH_PACKAGE + 1; i++)
        {
            str += i + (i == LEVEL_EACH_PACKAGE + 1 ? "" : ",");
        }

        for (int i = 1; i <= LevelData.worldNames.Length; i++)
        {
            TotalLevelCrossed.Add(i, str);
            currentLevel.Add(i, LEVEL_EACH_PACKAGE);
        }

        var userData = new PlayerData.PlayerDataObj
        {
            levelcross = TotalLevelCrossed,
            currentLevel = currentLevel,
            totalhints = 50
        };

        string json = JsonConvert.SerializeObject(userData);
        string encrypted = Encryption.Encrypt(json);
        string path = Application.persistentDataPath + "/data.json";
        File.WriteAllText(path, encrypted);
        PlayerPrefs.Save();
    }
}