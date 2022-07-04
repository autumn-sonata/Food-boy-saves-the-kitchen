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
    private bool triggerCalled; //True if any trigger is called.

<<<<<<< Updated upstream
    private Coordinator playerCoordinator; //the main camera

    private void Awake()
    {
        //Initialisation
        playerCoordinator = GameObject.Find("Main Camera").GetComponent<Coordinator>();
        //Find gameObject on You tile.
        UpdateColliderOnTile(); //Look for any pushable object on You Tile.
        foodSameTag = new List<GameObject>();
        triggerCalled = false;
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
        if (triggerCalled)
        {
            /* There is a change in food items on the You tile.
             * Not to be run every single frame, it is expensive!
             * Pass info to Coordinator.
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

            if (col != null)
            {
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
            } else
            {
                //No new collider available, clear foodSameTag
                foodSameTag.Clear();
            }
 
            playerCoordinator.addAndRemovePlayers(foodSameTag);
            oldCol = col; 
            triggerCalled = false;
        }
    }

    public void moveExecuted()
    {
        //Update foodSameTag if there are any cut foods, and abandon them (exclude from foodSameTag)
        foodSameTag.FindAll(food => !food.GetComponent<Tags>().isKnife() && food.GetComponent<Tags>().isCut())
=======
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
>>>>>>> Stashed changes
            .ForEach(food =>
            {
                food.GetComponent<DetachChildren>().detachAllChildren();
                food.GetComponent<Tags>().disablePlayerTag();
            });
<<<<<<< Updated upstream
        foodSameTag = foodSameTag.FindAll(food => !food.GetComponent<Tags>().isCut() || food.GetComponent<Tags>().isKnife());
    }

    public List<GameObject> playersAttached()
=======
            foodSameTag = foodSameTag.FindAll(food => food.GetComponent<Tags>().isPlayer());
        }
    }

    protected override void enableTileProperty()
>>>>>>> Stashed changes
    {
        if (foodSameTag == null)
            Debug.LogError("FoodSameTag in YouManager is not initialised!");
        return foodSameTag;
    }

<<<<<<< Updated upstream
    public bool hasFoodOnYouTile()
    {
        return col != null;
=======
    protected override void disableTileProperty()
    {
        oldCol.GetComponent<Tags>().disableYouTileTag();
>>>>>>> Stashed changes
    }

    public string youFoodTag()
    {
        return col.tag;
    }

    public List<GameObject> getFoodSameTag()
    {
<<<<<<< Updated upstream
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
         * 
         * Used for empty -> filled you tile.
         */

        oldCol = col;
        col = collision;
        triggerCalled = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        /* Function runs when a collider leaves the you tile.
         * 
         * Used for filled -> empty you tile.
         */

        oldCol = collision;
        //Additional check for whether there is anything left on you tile.
        UpdateColliderOnTile();
        triggerCalled = true;
    }

    private void UpdateColliderOnTile()
    {
        /* Find collider that is on top of the you tile.
         */

        Vector2 youTilePosition = new Vector2(transform.position.x, transform.position.y);
        col = Physics2D.OverlapPoint(youTilePosition, LayerMask.GetMask("Push"));
=======
        foreach (GameObject food in foodSameTag)
        {
            food.GetComponent<DetachChildren>().detachAllChildren();
            food.GetComponent<Tags>().disablePlayerTag();
        }
>>>>>>> Stashed changes
    }
}
