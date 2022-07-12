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
    private LayerMask push;

    private void Awake()
    {
        //Initialisation
        playerCoordinator = GameObject.Find("Main Camera").GetComponent<Coordinator>();
        push = LayerMask.GetMask("Push");
        //Find gameObject on tile.
        UpdateColliderOnTile(); //Look for any pushable object on tile. Initialise col.
        foodSameTag = new List<GameObject>();
        triggerCalled = false;
    }

    public void Initialise()
    {
        /* Initialises foodSameTag, col and oldCol.
         */
        col = Physics2D.OverlapPoint(transform.position, push);
        if (col)
        {
            //Initialise tags of col.
            enableTileProperty();

            //Initialise tags of foodSameTag
            foodSameTag = GameObject.FindGameObjectsWithTag(col.tag).ToList();
            foodSameTag.RemoveAll(food => food.GetComponent<Tags>().isCut());
            EnableFoodSameTagProperty();

            //Initialise oldCol to be col
            oldCol = col;
        }
    }

    public void TriggerTile()
    {
        /* When movement is done, check whether there is any object on this tile.
         * Then update accordingly.
         */

        UpdateColliderOnTile(); //updates col
        if (col && col != oldCol)
        {
            //new col. col changed, oldCol already initialised to previous col OR
            //tile is empty. col is empty, oldCol is still the previous collider.
            triggerCalled = true;
        }
        //else same col, no change. Do nothing.
    }

    public void OldColUpdate()
    {
        if (triggerCalled && oldCol)
            OldColRoutine();
    }

    public void NewColUpdate()
    {
        if (triggerCalled)
        {
            /* There is a change in items on the tile.
             * Not to be run every single frame, it is expensive!
             * 1) Pass info to Coordinator.
             * 2) Col is at destination, then
             * update all oldCol children destinations to their transform position.
             */

            if (col)
            {
                //Update to new collider
                NewColRoutine();
                col.GetComponent<DetachChildren>().detachAllChildren();

                //Update foodSameTag
                foodSameTag = GameObject.FindGameObjectsWithTag(col.tag).ToList();
                EnableFoodSameTagProperty();
            }
            else
            {
                //No new collider available, clear foodSameTag
                foodSameTag.Clear();
            }

            oldCol = col;
        }
        MoveExecuted();
        PlayerAdjustment();
    }

    public void ActivateDormantHotCold(Dictionary<string, bool[]> hotCold)
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
            if (NotOnAnyTile()) oldColTags.notOnAnyTile();
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

    public void DeactivateDormantHotCold(Dictionary<string, bool[]> hotCold)
    {
        /* If the same tag has both hot and cold, deactivate all objects that
         * are not on tiles. Update foodSameTag accordingly.
         */

        if (triggerCalled && col)
        {
            //Both hot and cold.
            //Deactivate all food:

            if (hotCold.ContainsKey(col.tag) && hotCold[col.tag].All(qn => qn))
            {
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
            triggerCalled = false;
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

    private void UpdateColliderOnTile()
    {
        /* Find collider that is on top of the tile.
         */

        col = Physics2D.OverlapPoint(transform.position, push);
    }

    private bool NotOnAnyTile()
    {
        /* Checks if oldCol is now standing on any tile.
         */
        return !Physics2D.OverlapPoint(oldCol.transform.position, 
            LayerMask.NameToLayer("Tiles"));
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
