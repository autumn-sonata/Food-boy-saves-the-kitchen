using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodTags : MonoBehaviour
{
    /* objectTags stores all Tags related to food items
     * Tags include:
     * 1) Food
     * 2) The actual food item e.g Cheese, Tomato etc
     * 3) Player
     * 4) Part of winTile
     * If the tag e.g Player is not found at the current moment, then disable Player behaviour.
     */
    private string[] objectTags = new string[4];
    public string foodItemName; //Name of the food item eg Cheese, Tomato etc

    private void Start()
    {
        objectTags[0] = "Food";
        objectTags[1] = foodItemName;
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
        GetComponent<MovementPlayer>().enabled = false;
    }

    public bool isInWinTile()
    {
        return objectTags[3] == "WinTile";
    }

    public string getFoodName()
    {
        return foodItemName;
    }
}