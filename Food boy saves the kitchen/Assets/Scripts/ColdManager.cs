using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColdManager : HotColdManager
{
    /* Handles Cold food implementation.
     */
    protected override void enableTileProperty()
    {
        col.GetComponent<Tags>().enableColdTileTag();
    }

    protected override void disableTileProperty()
    {
        oldCol.GetComponent<Tags>().disableColdTileTag();
    }

    protected override void EnableFoodSameTagProperty()
    {
        foreach (GameObject food in foodSameTag)
        {
            if (!food.GetComponent<Tags>().isInAnyTile())
                food.GetComponent<Tags>().enableCold(); 
        }
    }

    protected override void DisableFoodSameTagProperty()
    {
        foreach (GameObject food in foodSameTag)
        {
            food.GetComponent<Tags>().disableCold();
        }
    }
}
