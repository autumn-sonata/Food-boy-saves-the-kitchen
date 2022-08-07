using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class YouManager : TileManager
{
    /* Manages which foods the player can control aka the You tile.
     */

    #region Routines

    protected override void OldColRoutine()
    {
        /* Update all the Player and You tags, foodSameTag and destination of all objects.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        playerCoordinator.decrementPlayer(oldCol.tag);

        if (!playerCoordinator.isPlayer(oldCol.tag))
        {
            DisableFoodSameTagProperty();
        }
        disableTileProperty();
    }

    protected override void NewColRoutine()
    {
        /* Update to new collider
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        playerCoordinator.incrementPlayer(col.tag);
        enableTileProperty();
    }

    protected override void PlayerAdjustment()
    {
        /* Updates which item type in the level should be 
         * able to move by itself currently.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        playerCoordinator.addAndRemovePlayers(foodSameTag);
    }

    protected override void MoveExecuted()
    {
        /* Update foodSameTag if there are any foods cut/uncut different from col,
         * and abandon them (exclude from foodSameTag)
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        if (col)
        {
            if (!col.GetComponent<Tags>().isKnife())
            {
                foodSameTag.FindAll(food => food.GetComponent<Tags>().isCut())
                .ForEach(food =>
                {
                    food.GetComponent<DetachChildren>().detachAllChildren();
                    food.GetComponent<Tags>().disablePlayerTag();
                });
                foodSameTag = foodSameTag.FindAll(food => food.GetComponent<Tags>().isPlayer());
            }
        }
    }

    #endregion

    #region Tile Property Adjustments

    protected override void enableTileProperty()
    {
        /* The current collider with the tile now is known
         * to be on the You tile.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        col.GetComponent<Tags>().enableYouTileTag();
    }

    protected override void disableTileProperty()
    {
        /* The previous collider with the tile has 
         * been pushed off the You tile.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        oldCol.GetComponent<Tags>().disableYouTileTag();
    }

    #endregion

    #region Similar food tag Property Adjustments

    protected override void EnableFoodSameTagProperty()
    {
        /* Enables all other foods of the same type 
         * that are not within any tile to have player movement
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
            food.GetComponent<Tags>().enablePlayerTag(); 
        }
    }

    protected override void DisableFoodSameTagProperty()
    {
        /* Disables all other foods of the same type 
         * that are not within any tile to have player movement
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
            food.GetComponent<DetachChildren>().detachAllChildren();
            food.GetComponent<Tags>().disablePlayerTag();
        }
    }

    #endregion
}
