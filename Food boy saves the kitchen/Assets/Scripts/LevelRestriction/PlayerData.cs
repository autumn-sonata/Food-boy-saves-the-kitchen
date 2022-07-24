using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    private LevelStatus[] levels;
    private bool[] colliders;
    private bool[] levelSprite;
    private bool[] nextArrow;

    public PlayerData(PlayerInfo player)
    {
        levels = player.getLevelsUnlocked();
        colliders = player.getColliders();
        levelSprite = player.getLevelSprites();
        nextArrow = player.getNextArrow();
    }

    public LevelStatus[] getLevelsUnlocked()
    {
        if (levels == null)
        {
            Debug.LogError("Levels in PlayerData is not initialised.");
        }
        return levels;
    }

    public bool[] getColliders()
    {
        if (colliders == null)
        {
            Debug.LogError("Colliders in PlayerData is not initialised.");
        }
        return colliders; 
    }

    public bool[] getLevelSprite()
    {
        if (levelSprite == null)
        {
            Debug.LogError("Level sprites in PlayerData is not initialised.");
        }
        return levelSprite;
    }

    public bool[] getNextArrow()
    {
        if (nextArrow == null)
        {
            Debug.LogError("NextArrow array in PlayerData is not initialised.");
        }
        return nextArrow;
    }
}
