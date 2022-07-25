using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    /* Wrapper class to store information for level
     * unlock status.
     * 
     * This class will be serialized and deserialized
     * into and from a binary file.
     */

    private LevelStatus[] levels;
    private bool[] colliders;
    private bool[] levelSprite;
    private bool[] nextArrow;

    #region Constructor
    public PlayerData(PlayerInfo player)
    {
        /* Constructor for the PlayerData class.
         */

        levels = player.getLevelsUnlocked();
        colliders = player.getColliders();
        levelSprite = player.getLevelSprites();
        nextArrow = player.getNextArrow();
    }

    #endregion

    #region PlayerInfo read methods

    public LevelStatus[] getLevelsUnlocked()
    {
        /* Get the levels array.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * LevelStatus[]: Array to tell whether a level
         *   has been locked, unlocked or completed.
         */

        if (levels == null)
        {
            Debug.LogError("Levels in PlayerData is not initialised.");
        }
        return levels;
    }

    public bool[] getColliders()
    {
        /* Get the colliders array.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool[]: Array to tell whether a level's BoxCollider2D should
         *   be enabled or not.
         */

        if (colliders == null)
        {
            Debug.LogError("Colliders in PlayerData is not initialised.");
        }
        return colliders; 
    }

    public bool[] getLevelSprite()
    {
        /* Get the levelSprite array.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool[]: Array to tell whether a level's sprite should be lighted
         *   up or not (as a number visually).
         */

        if (levelSprite == null)
        {
            Debug.LogError("Level sprites in PlayerData is not initialised.");
        }
        return levelSprite;
    }

    public bool[] getNextArrow()
    {
        /* Get the nextArrow array.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool[]: Array to tell whether a new set of levels have been unlocked
         *   or not (on the next page/scene).
         */

        if (nextArrow == null)
        {
            Debug.LogError("NextArrow array in PlayerData is not initialised.");
        }
        return nextArrow;
    }

    #endregion
}
