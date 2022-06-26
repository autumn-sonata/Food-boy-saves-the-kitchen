using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class YouManager : MonoBehaviour
{
    /* Manages which foods the player can control aka the You tile.
     */
    private Collider2D col; //Food item that is within YouTile.
    private List<GameObject> foodSameTag; //To change the player tags.
    private Collider2D oldCol;

    private PlayerMovementCoordinator playerCoordinator; //the main camera

    private void Awake()
    {
        //Initialisation
        playerCoordinator = GameObject.Find("Main Camera").GetComponent<PlayerMovementCoordinator>();
        //Find gameObject on You tile.
        Vector2 youTilePosition = new Vector2(transform.position.x, transform.position.y);
        col = Physics2D.OverlapPoint(youTilePosition, LayerMask.GetMask("Push")); //Look for any pushable object on You Tile.
        foodSameTag = new List<GameObject>(); 
    }

    public void Initialise()
    {
        if (col != null)
        {
            if (col.GetComponent<Tags>().isKnife()) col.GetComponent<SharpController>().enableOtherYouTileTag();
            col.GetComponent<Tags>().enableYouTileTag();

            foodSameTag = GameObject.FindGameObjectsWithTag(col.tag).ToList();
            List<GameObject> additionalForKnife = new List<GameObject>();
            foreach (GameObject food in foodSameTag)
            {
                food.GetComponent<Tags>().enablePlayerTag();
                if (food.GetComponent<Tags>().isKnife())
                {
                    food.GetComponent<SharpController>().enableOtherPlayerTag();
                    additionalForKnife.Add(food.GetComponent<SharpController>().getOtherComponent()); //foodSameTag has both KnifeHilt and KnifeBlade types.
                }
            }
            foodSameTag.AddRange(additionalForKnife);
            oldCol = col;
        }
    }

    public void ChangeYouObject()
    {
        if (col != null && isOnTopOfTile())
        {
           /* There is a change in food items on the You tile.
            * Not to be run every single frame, it is expensive!
            * Pass info to PlayerMovementCoordinator.
            * Wait until parent of col is at destination, then
            * update all oldCol children destinations to their transform position.
            */

            //Tell player movement coordinator about the change in you tile object.
            if (oldCol != null)
            {
                playerCoordinator.decrementPlayer(oldCol.tag);

                //Update all the Player and You tags, foodSameTag and destination of all objects.
                if (!playerCoordinator.isPlayer(oldCol.tag))
                {
                    foreach (GameObject food in foodSameTag)
                    {
                        food.GetComponent<DetachChildren>().detachAllChildren();
                        food.GetComponent<Tags>().disablePlayerTag();
                        if (food.GetComponent<Tags>().isKnife())
                        {
                            food.GetComponent<SharpController>().getOtherComponent().GetComponent<DetachChildren>().detachAllChildren();
                            food.GetComponent<SharpController>().disableOtherPlayerTag();
                        }
                    }
                }
                oldCol.GetComponent<Tags>().disableYouTileTag();
                if (oldCol.GetComponent<Tags>().isKnife()) oldCol.GetComponent<SharpController>().disableOtherYouTileTag();
            }

            //Update to new collider
            playerCoordinator.incrementPlayer(col.tag);
            col.GetComponent<Tags>().enableYouTileTag();
            col.GetComponent<DetachChildren>().detachAllChildren();
            if (col.GetComponent<Tags>().isKnife()) col.GetComponent<SharpController>().enableOtherYouTileTag();

            foodSameTag = GameObject.FindGameObjectsWithTag(col.tag).ToList();
            List<GameObject> additionalForKnife = new List<GameObject>();
            foreach (GameObject food in foodSameTag)
            {
                food.GetComponent<Tags>().enablePlayerTag();
                if (food.GetComponent<Tags>().isKnife())
                {
                    food.GetComponent<SharpController>().enableOtherPlayerTag();
                    //foodSameTag has both KnifeHilt and KnifeBlade types.
                    additionalForKnife.Add(food.GetComponent<SharpController>().getOtherComponent()); 
                }
            }
            foodSameTag.AddRange(additionalForKnife);
            playerCoordinator.addAndRemovePlayers(foodSameTag);
            oldCol = col; //run this statement once every You tile food object change.
        }
    }

    public void moveExecuted()
    {
        //Update foodSameTag if there are any cut foods, and abandon them (exclude from foodSameTag)
        foodSameTag.FindAll(food => !food.GetComponent<Tags>().isKnife() && food.GetComponent<Tags>().isCut())
            .ForEach(food =>
            {
                food.GetComponent<DetachChildren>().detachAllChildren();
                food.GetComponent<Tags>().disablePlayerTag();
            });
        foodSameTag = foodSameTag.FindAll(food => !food.GetComponent<Tags>().isCut() || food.GetComponent<Tags>().isKnife());
    }

    public List<GameObject> playersAttached()
    {
        if (foodSameTag == null)
            Debug.LogError("FoodSameTag in YouManager is not initialised!");
        return foodSameTag;
    }

    public bool hasFoodOnYouTile()
    {
        return col != null;
    }

    public string youFoodTag()
    {
        return col.tag;
    }

    public List<GameObject> getFoodSameTag()
    {
        /* Gets all foods with tag similar to col.
         */
        return foodSameTag;
    }

    public Collider2D getCol()
    {
        return col;
    }

    public Collider2D getOldCol()
    {
        return oldCol;
    }

    public void Undo(List<GameObject> prev, Collider2D collider, Collider2D oldCollider)
    {
        /* Updates foodSameTag.
         */
        foodSameTag = prev;
        col = collider;
        oldCol = oldCollider;
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
