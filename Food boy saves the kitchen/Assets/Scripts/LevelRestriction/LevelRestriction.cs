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

    #region Load and Save from file

    public static void Save(PlayerInfo player)
    {
        /* Save PlayerInfo into a binary file stored within the system.
         * 
         * Parameters
         * ----------
         * 1) player: Master script for managing which levels
         *    are currently accessible to the user/player.
         * 
         * Return
         * ------
         * 
         */

        BinaryFormatter bf = new();
        using FileStream file = File.Create(path);

        PlayerData data = new PlayerData(player);
        bf.Serialize(file, data);
        file.Close();
    }

    public static PlayerData Load()
    {
        /* Loads information from binary file into PlayerInfo
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * PlayerData: Class containing all the necessary information
         *   for PlayerInfo to know which levels are currently accessible
         *   to the user/player.
         */

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

    #endregion

    #region File Inquiry
    public static bool saveFileExists()
    {
        /* Helper function to check if there is such a binary file
         * within the system at a current point in time.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool: True if the file exists, false otherwise
         */

        return File.Exists(path);
    }

    #endregion
}
