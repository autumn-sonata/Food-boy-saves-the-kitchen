using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileManager: MonoBehaviour
{
    /* Script to handle 1x1 sized tiles.
     */
    protected Collider2D col; //Food item that is within hotTile.
    protected List<GameObject> foodSameTag; //To change the player tags.
    protected Collider2D oldCol;
    protected bool triggerCalled; //True if any trigger is called.

    protected Coordinator playerCoordinator; //the main camera

    private void Awake()
    {
        //Initialisation
        playerCoordinator = GameObject.Find("Main Camera").GetComponent<Coordinator>();
        //Find gameObject on tile.
        UpdateColliderOnTile(); //Look for any pushable object on tile. Initialise col.
        foodSameTag = new List<GameObject>();
        triggerCalled = false;
    }

    public void Initialise()
    {
        /* Initialises foodSameTag and oldCol.
         */
        if (col != null)
        {
            //Initialise tags of col.
            enableTileProperty();
            DisableHotColdOnTile();

            //Initialise tags of foodSameTag
            foodSameTag = GameObject.FindGameObjectsWithTag(col.tag).ToList();
            EnableFoodSameTagProperty();

            //Initialise oldCol to be col
            oldCol = col;
        }
    }
    public void ChangeObject()
    {
        //Called when the object on top of tile has been changed
        if (triggerCalled)
        {
            /* There is a change in items on the tile.
             * Not to be run every single frame, it is expensive!
             * 1) Pass info to Coordinator.
             * 2) Wait until parent of col is at destination, then
             * update all oldCol children destinations to their transform position.
             */

            if (oldCol)
            {
                //Update foods of previous col and col's tags 
                OldColRoutine();
            }

            if (col)
            {
                //Update to new collider
                NewColRoutine();
                col.GetComponent<DetachChildren>().detachAllChildren();

                //Update foodSameTag
                foodSameTag = GameObject.FindGameObjectsWithTag(col.tag).ToList();
                EnableFoodSameTagProperty();

                //Disable hot-cold tags 
                DisableHotColdOnTile();
            } else
            {
                //No new collider available, clear foodSameTag
                foodSameTag.Clear();
            }

            //PlayerAdjustment();
            oldCol = col;
            triggerCalled = false;
        }
        MoveExecuted();
        PlayerAdjustment();
    }

    public void EnableHotColdOnTile(Dictionary<string, bool[]> hotCold)
    {
        /* Enable hot cold property when stepping off a tile.
         * 
         * Check with Coordinator whether there are any of the same
         * tag as oldCol in hot and cold. Enable hot/cold for oldCol
         * accordingly.
         */
        if (oldCol && triggerCalled)
        {
            Tags oldColTags = oldCol.GetComponent<Tags>();
            if (hotCold.ContainsKey(oldCol.tag) && !oldCol.GetComponent<Tags>().isInAnyTile())
            {
                bool[] enableHotCold = hotCold[oldCol.tag];
                if (enableHotCold.Length != 2)
                    Debug.LogError("hotCold dictionary bool[] is not of length 2.");

                if (enableHotCold[0])
                {
                    oldColTags.enableCold();
                }
                else
                {
                    oldColTags.disableCold();
                }

                if (enableHotCold[1])
                {
                    oldColTags.enableHot();
                }
                else
                {
                    oldColTags.disableHot();
                }
            }
            else
            {
                oldColTags.disableHot();
                oldColTags.disableCold();
            }
        }
    }

    public void CheckHotCold(Dictionary<string, bool[]> hotCold)
    {
        /* If the same tag has both hot and cold, deactivate all objects that
         * are not on tiles. Update foodSameTag accordingly.
         */

        if (hotCold.ContainsKey(col.tag) && hotCold[col.tag].All(qn => qn))
        {
            //Both hot and cold.
            //Deactivate all food:
            foreach (GameObject food in foodSameTag)
            {
                if (!food.GetComponent<Tags>().isInAnyTile())
                {
                    //Deactivate
                    food.GetComponent<DetachChildren>().detachAllChildren();
                    food.GetComponent<Tags>().enableInactive();
                }
            }

            //Change foodSameTag to match
            foodSameTag = foodSameTag.FindAll(food => !food.GetComponent<Tags>().isInactive());
        }
    }

    public List<GameObject> playersAttached()
    {
        return foodSameTag;
    }

    public bool hasFoodOnYouTile()
    {
        return col != null;
    }

    public string colFoodTag()
    {
        if (col)
            return col.tag;
        return null;
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
         * Function runs when a new collider comes into contact with tile's collision box.
         * Delays change of foodSameTag array.
         * 
         * Used for empty -> filled tile.
         */

        oldCol = col;
        col = collision;
        triggerCalled = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        /* Function runs when a collider leaves the tile.
         * 
         * Used for filled -> empty tile.
         */

        oldCol = collision;
        //Additional check for whether there is anything left on tile.
        UpdateColliderOnTile();
        triggerCalled = true;
    }

    private void UpdateColliderOnTile()
    {
        /* Find collider that is on top of the tile.
         */

        Vector2 youTilePosition = new Vector2(transform.position.x, transform.position.y);
        col = Physics2D.OverlapPoint(youTilePosition, LayerMask.GetMask("Push"));
    }

    protected void DisableHotColdOnTile()
    {
        //Disable hot cold property when stepping into a tile.
        col.GetComponent<Tags>().disableCold();
        col.GetComponent<Tags>().disableHot();
    }

    protected abstract void OldColRoutine(); //oldCol updates.
    protected abstract void NewColRoutine(); //Col updates.

    protected abstract void PlayerAdjustment();

    protected abstract void MoveExecuted();

    protected abstract void enableTileProperty();
    protected abstract void disableTileProperty();
    protected abstract void EnableFoodSameTagProperty();

    protected abstract void DisableFoodSameTagProperty();
}
