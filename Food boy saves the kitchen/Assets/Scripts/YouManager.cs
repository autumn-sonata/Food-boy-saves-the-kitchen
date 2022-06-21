using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class YouManager : MonoBehaviour
{
    /* Manages which foods the player can control aka the You tile.
     */
    private Collider2D col; //Food item that is within YouTile.
    private GameObject[] foodSameTag; //To change the player tags.
    private Collider2D oldCol;

    private GameObject playerCoordinator; //the main camera

    private void Awake()
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
        playerCoordinator = GameObject.Find("Main Camera");
    }

    void Update()
    {
        if (isOnTopOfTile())
        {
            /* There is a change in food items on the You tile.
             * Pass info to PlayerMovementCoordinator.
             * Wait until parent of col is at destination, then
             * update all oldCol children destinations to their transform position.
             */

            //Update all the Player and You tags, foodSameTag and destination of all objects.
            foreach (GameObject food in foodSameTag)
            {
                food.GetComponent<DetachChildren>().detachAllChildren();
                food.GetComponent<Tags>().disablePlayerTag();
            }
            oldCol.GetComponent<Tags>().disableYouTileTag();

            col.GetComponent<Tags>().enableYouTileTag();
            foodSameTag = GameObject.FindGameObjectsWithTag(col.GetComponent<Tags>().getFoodName());
            foreach (GameObject food in foodSameTag)
            {
                food.GetComponent<Tags>().enablePlayerTag();
            }
            playerCoordinator.GetComponent<PlayerMovementCoordinator>().addAndRemovePlayers(foodSameTag);
            oldCol = col; //run this statement once every You tile food object change.
        }
        if (playerCoordinator.GetComponent<PlayerMovementCoordinator>().hasMoved())
        {
            //Update foodSameTag if there are any cut values.
            List<GameObject> sameFood = foodSameTag.ToList();
            sameFood.FindAll(food => food.GetComponent<Tags>().isCut())
                .ForEach(food =>
                {
                    food.GetComponent<DetachChildren>().detachAllChildren();
                    food.GetComponent<Tags>().disablePlayerTag();
                });
            foodSameTag = sameFood.FindAll(food => !food.GetComponent<Tags>().isCut()).ToArray();
        }
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
}
