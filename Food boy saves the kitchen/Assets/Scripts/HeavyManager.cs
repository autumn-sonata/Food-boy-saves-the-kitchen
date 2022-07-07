using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyManager : TileManager
{
    /* Manages the heavy tile.
     * Property: All other foods excluding those on tiles are heavy.
     * This means that they cannot be pushed by other foods. 
     */
    protected override void EnableFoodSameTagProperty()
    {
        foreach (GameObject food in foodSameTag)
        {
            //Only if not a player or on a tile.
            if (!food.GetComponent<Tags>().isPlayer() && !food.GetComponent<Tags>().isInAnyTile())
                food.GetComponent<Tags>().enableHeavy();
        }
    }

    protected override void DisableFoodSameTagProperty()
    {
        foreach (GameObject food in foodSameTag)
        {
            food.GetComponent<Tags>().disableHeavy();
        }
    }

    protected override void enableTileProperty()
    {
        col.GetComponent<Tags>().enableHeavyTileTag();
    }

    protected override void disableTileProperty()
    {
        oldCol.GetComponent<Tags>().disableHeavyTileTag();
    }

    protected override void NewColRoutine()
    {
        //Update to new collider
        enableTileProperty();
    }

    protected override void OldColRoutine()
    {
        DisableFoodSameTagProperty();
        disableTileProperty();
    }

    protected override void PlayerAdjustment()
    {
        //Nothing. Does not need to tell coordinator
        //any information.
    }

    protected override void MoveExecuted()
    {
        //No additional work needs to be done.
    }
}
