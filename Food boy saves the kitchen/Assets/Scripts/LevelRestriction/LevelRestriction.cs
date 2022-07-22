using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public static class LevelRestriction
{
    /* IO from PlayerData to Binary file.
     */

    public static void Save(PlayerInfo playerInfo)
    {
        BinaryFormatter bf = new();
        using FileStream file = File.Create(Path.Combine(Application.persistentDataPath,
            "/gameProg.dat"));

        PlayerData data = new PlayerData(playerInfo);
        bf.Serialize(file, data);
        file.Close();
    }

    public static PlayerData Load()
    {
        string path = Path.Combine(Application.persistentDataPath + "/gameProg.dat");
        if (File.Exists(path))
        {
            BinaryFormatter bf = new();
            using FileStream file = File.OpenRead(path);

            PlayerData data = bf.Deserialize(file) as PlayerData;
            file.Close();
            return data;
        } else
        {
            Debug.LogError("MaxLvlFile not found in " + path);
            return null;
        }
    }
}
