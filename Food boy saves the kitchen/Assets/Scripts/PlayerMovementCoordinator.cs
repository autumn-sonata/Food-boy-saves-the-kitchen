using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovementCoordinator : MonoBehaviour
{
    /* Coordinates all player movements to be in sync. Only allows
     * for next set of movements once all players have finished moving.
     */
    List<GameObject> movableObjects;

    // Start is called before the first frame update
    void Start()
    {
        //Get all players that can be moved.
        GameObject[] youTiles = GameObject.FindGameObjectsWithTag("You");
        foreach (GameObject youTile in youTiles)
        {
            GameObject onYouTile = youTile.GetComponent<YouManager>().currentObjectOnYouTile();
            GameObject[] sameFoodTag = GameObject.FindGameObjectsWithTag(onYouTile.GetComponent<Tags>().getFoodName());
            foreach (GameObject food in sameFoodTag)
            {
                if (!food.GetComponent<Tags>().isInAnyTile())
                {
                    //can be moved
                    movableObjects.Add(food);
                }
            }
        }
    }

    void Update()
    {
        if (allMovementsComplete())
        {
            //Make sure movableObjects updated, and permit next move
        }            
    }

    public bool allMovementsComplete()
    {
        /* Checks whether the moves for all moving players are complete
         * by checking whether they are all at their destination.
         */

        return movableObjects.Select(food => food.GetComponent<PlayerManager>().isAtDestination())
            .Any(atDest => false);
    }
}
