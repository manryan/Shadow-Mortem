using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem {

	public static void SavePlayer(id player, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        //FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        //string path = Application.persistentDataPath + "/gamesave.save";

        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);

        formatter.Serialize(stream, data);

        stream.Close();
    }

    public static PlayerData LoadPlayer(string file)
    {
        //string file = Application.persistentDataPath + "/gamesave.save";
        if (File.Exists(file))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(file, FileMode.Open);

           // formatter.Deserialize(stream);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;


            stream.Close();
            return data;
        }
        else
        {
            Debug.Log("no saved files");
            return null;
        }
    }
}
