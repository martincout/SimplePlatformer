using SimplePlatformer.Player;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
   public static void SaveGame(PlayerController player, int score)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        //Path combine for multiple platforms
        string path = Path.Combine(Application.persistentDataPath, "player.save");
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData(player, score);

        formatter.Serialize(stream, data);

        stream.Close();
    }

    public static GameData LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "player.save");
        if (File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found!");
            return null;
        }
    }
}
