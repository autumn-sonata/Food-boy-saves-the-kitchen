using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class YouManager : TileManager
{
    /* Manages which foods the player can control aka the You tile.
     */

    protected override void OldColRoutine()
    {
        playerCoordinator.decrementPlayer(oldCol.tag);

        //Update all the Player and You tags, foodSameTag and destination of all objects.
        if (!playerCoordinator.isPlayer(oldCol.tag))
        {
            DisableFoodSameTagProperty();
        }
        disableTileProperty();
    }

    protected override void NewColRoutine()
    {
        //Update to new collider
        playerCoordinator.incrementPlayer(col.tag);
        enableTileProperty();
    }

    protected override void PlayerAdjustment()
    {
        playerCoordinator.addAndRemovePlayers(foodSameTag);
    }

    protected override void MoveExecuted()
    {
        //Update foodSameTag if there are any foods cut/uncut different from col,
        //and abandon them (exclude from foodSameTag)

        bool colCut = col.GetComponent<Tags>().isCut();
        if (!col.GetComponent<Tags>().isKnife())
        {
            foodSameTag.FindAll(food => food.GetComponent<Tags>().isCut() != colCut)
            .ForEach(food =>
            {
                food.GetComponent<DetachChildren>().detachAllChildren();
                food.GetComponent<Tags>().disablePlayerTag();
            });
            foodSameTag = foodSameTag.FindAll(food => food.GetComponent<Tags>().isPlayer());
        }
    }

    protected override void enableTileProperty()
    {
        col.GetComponent<Tags>().enableYouTileTag();
    }

    protected override void disableTileProperty()
    {
        oldCol.GetComponent<Tags>().disableYouTileTag();
    }

    protected override void EnableFoodSameTagProperty()
    {
        foreach (GameObject food in foodSameTag)
        {
            food.GetComponent<Tags>().enablePlayerTag(); 
        }
    }

    protected override void DisableFoodSameTagProperty()
    {
        foreach (GameObject food in foodSameTag)
        {
            food.GetComponent<DetachChildren>().detachAllChildren();
            food.GetComponent<Tags>().disablePlayerTag();
        }
    }
}
