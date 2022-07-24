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
    private static readonly string path = Path.Combine(Application.persistentDataPath,
            "gameProg.data");

    public static void Save(PlayerInfo player)
    {
        BinaryFormatter bf = new();
        using FileStream file = File.Create(path);

        PlayerData data = new PlayerData(player);
        bf.Serialize(file, data);
        file.Close();
    }

    public static PlayerData Load()
    {
        if (saveFileExists())
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

    public static bool saveFileExists()
    {
        return File.Exists(path);
    }
}
