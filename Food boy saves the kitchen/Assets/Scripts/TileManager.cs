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

    #region Initialisation

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
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
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

    #endregion

    #region Tile triggers
    public void TriggerTile()
    {
        /* When movement is done, check whether there is any object on this tile.
         * Then update accordingly.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
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
        /* Updates the tags of the old collider that has been
         * moved out of this tile if needed.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        if (triggerCalled && oldCol)
            OldColRoutine();
    }

    public void NewColUpdate()
    {
        /* Updates tags current collider on top of the tile
         * and other tags of objects of the same type as collider.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

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
                col.GetComponent<Tags>().disableCold();
                col.GetComponent<Tags>().disableHot();

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

    #endregion

    #region Hot cold changes
    public void ActivateDormantHotCold(Dictionary<string, bool[]> hotCold)
    {
        /* Enable hot cold property when stepping off a tile.
         * 
         * Check with Coordinator whether there are any of the same
         * tag as oldCol in hot and cold. Enable hot/cold for oldCol
         * accordingly.
         * 
         * Parameters
         * ----------
         * 1) hotCold: Determines which tag and thus, food type is supposed to
         *   be hot and cold.
         * 
         * Return
         * ------
         * 
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
         * 
         * Parameters
         * ----------
         * 1) hotCold: Determines which tag and thus, food type is supposed to
         *   be hot and cold.
         * 
         * Return
         * ------
         * 
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

    #endregion

    #region Miscellaneous
    public List<GameObject> playersAttached()
    {
        /* Return all objects with a similar tag to
         * the collider
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * List<GameObject>: list of all objects with
         *   a similar tag to the collider
         */

        return foodSameTag;
    }

    public bool hasFoodOnYouTile()
    {
        /* Checks if there is an object on the tile
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool: True if there is an object on the tile,
         *   False otherwise.
         */

        return col != null;
    }

    public string colFoodTag()
    {
        /* If there is an object on the tile,
         * return its tag as a string, otherwise null.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * string: tag of collider, otherwise null.
         */

        if (col)
            return col.tag;
        return null;
    }

    public List<GameObject> getFoodSameTag()
    {
        /* Gets all foods with tag similar to col.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * List<GameObject>: list of all objects with
         *   a similar tag to the collider
         */

        return foodSameTag;
    }

    public Collider2D getCol()
    {
        /* Return collider of object on tile.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * Collider2D: collider of object on tile.
         */

        return col;
    }

    public Collider2D getOldCol()
    {
        /* Return collider of previous object on tile
         * that has been pushed off.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * Collider2D: collider of object previously
         *   on this tile.
         */

        return oldCol;
    }

    public void Undo(List<GameObject> prev, Collider2D collider, Collider2D oldCollider)
    {
        /* Updates foodSameTag.
         * 
         * Parameters
         * ----------
         * 1) prev: previous set of gameObjects with tags similar
         *   to collider at that point in time
         * 
         * 2) collider: collider of object that is on the tile at 
         *   that point in time
         *   
         * 3) oldCollider: collider of previous object that is on 
         *   the tile at that point in time.
         * 
         * Return
         * ------
         * 
         */

        foodSameTag = prev;
        col = collider;
        oldCol = oldCollider;
    }

    private void UpdateColliderOnTile()
    {
        /* Find collider that is on top of the tile.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        col = Physics2D.OverlapPoint(transform.position, push);
    }

    private bool NotOnAnyTile()
    {
        /* Checks if oldCol is now standing on any tile.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool: True if there is no tile in old collider's position,
         *   False otherwise.
         */

        return !Physics2D.OverlapPoint(oldCol.transform.position, 
            LayerMask.NameToLayer("Tiles"));
    }

    #endregion

    #region Abstract methods

    protected abstract void OldColRoutine(); //oldCol updates.
    protected abstract void NewColRoutine(); //Col updates.
    protected abstract void PlayerAdjustment();

    protected abstract void MoveExecuted();

    protected abstract void enableTileProperty();
    protected abstract void disableTileProperty();
    protected abstract void EnableFoodSameTagProperty();

    protected abstract void DisableFoodSameTagProperty();

    #endregion

}
