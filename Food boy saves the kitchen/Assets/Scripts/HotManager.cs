using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotManager : HotColdManager
{
    /* Handles Hot food implementation.
     */
    protected override void enableTileProperty()
    {
        col.GetComponent<Tags>().enableHotTileTag();
    }

    protected override void disableTileProperty()
    {
        oldCol.GetComponent<Tags>().disableHotTileTag();
    }

    protected override void EnableFoodSameTagProperty()
    {
        foreach (GameObject food in foodSameTag)
        {
            if (!food.GetComponent<Tags>().isInAnyTile())
                food.GetComponent<Tags>().enableHot();
        }
    }

    protected override void DisableFoodSameTagProperty()
    {
        foreach (GameObject food in foodSameTag)
        {
            food.GetComponent<Tags>().disableHot();
        }
    }
}
