using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyManager : TileManager
{
    /* Manages the heavy tile.
     * Property: All other foods excluding those on tiles are heavy.
     * This means that they cannot be pushed by other foods. 
     */

    #region Similar food tag Property Adjustments

    protected override void EnableFoodSameTagProperty()
    {
        /* Enables all other foods of the same type 
         * that are not within any tile to have the heavy property.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        foreach (GameObject food in foodSameTag)
        {
            //Only if not a player or on a tile.
            if (!food.GetComponent<Tags>().isPlayer() && !food.GetComponent<Tags>().isInAnyTile())
                food.GetComponent<Tags>().enableHeavy();
        }
    }

    protected override void DisableFoodSameTagProperty()
    {
        /* Disables all other foods of the same type 
         * that are not within any tile to not have the heavy property.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        foreach (GameObject food in foodSameTag)
        {
            food.GetComponent<Tags>().disableHeavy();
        }
    }

    #endregion

    #region Tile Property Adjustments

    protected override void enableTileProperty()
    {
        /* The current collider with the tile now is known
         * to be on the heavy tile.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        col.GetComponent<Tags>().enableHeavyTileTag();
    }

    protected override void disableTileProperty()
    {
        /* The previous collider with the tile has 
         * been pushed off the heavy tile.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        oldCol.GetComponent<Tags>().disableHeavyTileTag();
    }

    #endregion

    #region Routine calls 

    protected override void NewColRoutine()
    {
        /* Update to a new collider
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        enableTileProperty();
    }

    protected override void OldColRoutine()
    {
        /* Disable the properties of the object type 
         * that stepped off this tile
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

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

    #endregion
}
