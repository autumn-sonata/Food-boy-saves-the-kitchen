using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tags : MonoBehaviour
{
    /* objectTags stores all Tags. Tags control the behaviour of the object.
     * Tags include:
     * 1) Food TODO: POSSIBLY REMOVE THIS TAG? SEEMS REDUNDANT
     * 2) The actual food item e.g Cheese, Tomato etc
     * 3) Player
     * 4) Part of winTile
     * 5) Part of youTile
     * If the tag e.g Player is not found at the current moment, then disable Player behaviour.
     */
    private string[] objectTags = new string[5];
    public string foodItemName; //Name of the food item eg Cheese, Tomato etc

    private void Start()
    {
        objectTags[0] = "Food";
        objectTags[1] = foodItemName;
    }

    public bool isFood()
    {
        return objectTags[1] != null;
    }

    public string getFoodName()
    {
        return foodItemName;
    }
    public bool isPlayer()
    {
        return objectTags[2] == "Player";
    }

    public void disablePlayerTag()
    {
        objectTags[2] = null;
    }

    public void enablePlayerTag()
    {
        objectTags[2] = "Player";
    }

    public void enableWinTileTag()
    {
        objectTags[3] = "WinTile";
    }

    public void disableWinTileTag()
    {
        objectTags[3] = null;
    }

    public bool isInWinTile()
    {
        return objectTags[3] == "WinTile";
    }

    public void enableYouTileTag()
    {
        objectTags[4] = "YouTile";
    }

    public void disableYouTileTag()
    {
        objectTags[4] = null;
    }

    public bool isInYouTile()
    {
        return objectTags[4] == "YouTile";
    }

    public bool isInAnyTile()
    {
        return objectTags[3] == "WinTile" || objectTags[4] == "YouTile";
    }
}