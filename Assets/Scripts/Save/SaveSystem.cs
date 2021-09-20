using SimplePlatformer.Player;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveGame(PlayerController player, int score, List<CampFire> campFires, 
        List<CellDoor> cellDoors,Dictionary<KeyColor,int> keys, List<GameObject> levelKeys)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        //Path combine for multiple platforms
        string path = Path.Combine(Application.persistentDataPath, "player.save");
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData(player, score, campFires, cellDoors, keys,levelKeys);

        formatter.Serialize(stream, data);

        stream.Close();
    }

    public static GameData LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "player.save");
        if (File.Exists(path)){
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                GameData data = formatter.Deserialize(stream) as GameData;
                stream.Close();
                return data;
            } catch (SerializationException ex) { 
                return null;
            } 
        }
        else
        {
            Debug.LogError("Save file not found!");
            return null;
        }
    }
}
