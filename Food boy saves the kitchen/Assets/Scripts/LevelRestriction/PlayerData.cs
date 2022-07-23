using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    private GameObject[] levelObj;
    private LevelStatus[] levels;

    public PlayerData(PlayerInfo player)
    {
        levelObj = player.getLevelObj();
        levels = player.getLevelsUnlocked();
    }

    public LevelStatus[] getLevelsUnlocked()
    {
        if (levels == null)
        {
            Debug.LogError("Levels in PlayerData is not initialised.");
        }
        return levels;
    }

    public GameObject[] getLevelObj()
    {
        return levelObj; 
    }
}
