using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouManager : MonoBehaviour
{
    /* Manages which foods the player can control aka the You tile.
     */
    private Collider2D col; //Food item that is within YouTile.
    private GameObject[] foodSameTag; //To change the player tags.
    private Collider2D oldCol;

    private void Start()
    {
        //Find gameObject on You tile.
        Vector2 youTilePosition = new Vector2(transform.position.x, transform.position.y);
        col = Physics2D.OverlapPoint(youTilePosition, LayerMask.GetMask("Push")); //Look for any pushable object on You Tile.
        col.GetComponent<Tags>().enableYouTileTag();

        foodSameTag = GameObject.FindGameObjectsWithTag(col.GetComponent<Tags>().getFoodName());
        foreach (GameObject food in foodSameTag)
        {
            food.GetComponent<Tags>().enablePlayerTag();
        }
        oldCol = col;
    }

    void Update()
    {
        youFoodChange();
        if (isOnTopOfTile())
        {
            //Update all the Player and You tags, foodSameTag and destination of all objects.
                foreach (GameObject food in foodSameTag)
            {
                food.GetComponent<Tags>().disablePlayerTag();
            }
            oldCol.GetComponent<Tags>().disableYouTileTag();

            col.GetComponent<Tags>().enableYouTileTag();
            foodSameTag = GameObject.FindGameObjectsWithTag(col.tag);
            foreach (GameObject food in foodSameTag)
            {
                food.GetComponent<Tags>().enablePlayerTag();
            }
            oldCol = col; //run this statement once every You tile food object change.
        }
    }

    public bool playerMoveIsDone()
    {
        /* Returns true if the player move is complete; that is when the player
         * moves to its destination.
         */
        
        foreach (GameObject food in foodSameTag)
        {
            if (!food.GetComponent<Tags>().isInAnyTile())
            {
                //food can be moved
                return food.GetComponent<PlayerManager>().isAtDestination();
            }
        }
        return false;
    }

    public GameObject currentObjectOnYouTile()
    {
        return col.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*
         * Function runs when a new collider comes into contact with YouTile's collision box.
         * Delays change of foodSameTag array.
         */

        oldCol = col;
        col = collision;
    }

    private bool isOnTopOfTile()
    {
        /* Function to tell when the player reaches its destination on top of You Tile
         * and if this statement has been run before on the same gameObject.
         */

        return col.GetComponent<PlayerManager>().isAtDestination() && col != oldCol;
    }

    private void youFoodChange()
    {
        if (col != oldCol && colParentAtDest())
        {
            /* There is a change in food items on the You tile.
             * Wait until parent of col is at destination, then
             * update all oldCol children destinations to their transform position.
             */
            
            foreach (GameObject food in foodSameTag)
            {
                foreach (Transform pushedObject in food.transform)
                {
                    pushedObject.GetComponent<PlayerManager>().updateDestinationToCurrPosition();
                }
                food.transform.DetachChildren();
            }
            
        }
    }

    private bool colParentAtDest()
    {
        /* To check whether the parent of col or col itself is at the destination.
         */

        return col.transform.parent != null ? 
            col.transform.parent.GetComponent<PlayerManager>().isAtDestination() :
            col.GetComponent<PlayerManager>().isAtDestination();
    }
}
