using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColdManager : HotColdManager
{
    /* Handles "Cold" food implementation, whose
     * attribute is given by the "Cold" tile.
     */

    #region Tile Property Adjustments
    protected override void enableTileProperty()
    {
        /* The current collider with the tile now is known
         * to be on the cold tile.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        col.GetComponent<Tags>().enableColdTileTag();
    }

    protected override void disableTileProperty()
    {
        /* The previous collider with the tile has 
         * been pushed off the cold tile.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        oldCol.GetComponent<Tags>().disableColdTileTag();
    }

    #endregion

    #region Similar food tag Property Adjustments

    protected override void EnableFoodSameTagProperty()
    {
        /* Enables all other foods of the same type 
         * that are not within any tile to have the cold property
         * (blue outline)
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
                food.GetComponent<Tags>().enableCold(); 
        }
    }

    protected override void DisableFoodSameTagProperty()
    {
        /* Disables all other foods of the same type 
         * that are not within any tile to not have the cold property
         * (blue outline)
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
            food.GetComponent<Tags>().disableCold();
        }
    }

    #endregion
}
