using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotManager : HotColdManager
{
    /* Handles Hot food implementation.
     */

    #region Tile Property Adjustments
    protected override void enableTileProperty()
    {
        /* The current collider with the tile now is known
         * to be on the hot tile.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        col.GetComponent<Tags>().enableHotTileTag();
    }

    protected override void disableTileProperty()
    {
        /* The previous collider with the tile has 
         * been pushed off the hot tile.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        oldCol.GetComponent<Tags>().disableHotTileTag();
    }

    #endregion

    #region Similar food tag Property Adjustments

    protected override void EnableFoodSameTagProperty()
    {
        /* Enables all other foods of the same type 
         * that are not within any tile to have the hot property
         * (orange outline)
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
            if (!food.GetComponent<Tags>().isInAnyTile())
                food.GetComponent<Tags>().enableHot();
        }
    }

    protected override void DisableFoodSameTagProperty()
    {
        /* Disables all other foods of the same type 
         * that are not within any tile to not have the hot property
         * (orange outline)
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
            food.GetComponent<Tags>().disableHot();
        }
    }

    #endregion
}
