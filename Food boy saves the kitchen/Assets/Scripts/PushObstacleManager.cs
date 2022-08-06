using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PushObstacleManager : MonoBehaviour
{
    /* Handles the obstacles that are considered as push objects (mostly other food items)
     */

    private GameObject foreFrontOfPlayer; //Object furthest in front according to direction of movement
    private LayerMask push;
    private Vector2 directionPush;
    private List<GameObject> inFront; //Does not include the player themselves.
    private bool hasHeavy;

    #region Unity specific functions

    private void Awake()
    {
        foreFrontOfPlayer = gameObject;
        push = LayerMask.GetMask("Push");
        directionPush = new Vector2(0f, 0f);
        inFront = new List<GameObject>();
        hasHeavy = false;
    }

    #endregion

    #region Driver functions

    public void updateDirection(Vector2 direction)
    {
        /* Updates direction player is moving within PushObstacleManager.
         * Includes updating:
         * 1) foreFrontOfPlayer
         * 2) directionPush
         * 3) inFront list
         * 4) hasHeavy
         * 
         * Parameters
         * ----------
         * 1) direction: direction as dictated by the user/player
         *   through keyboard input.
         * 
         * Return
         * ------
         * 
         */

        foreFrontOfPlayer = gameObject;
        directionPush = direction;
        hasHeavy = false;
        detachFoodFromPlayer();

        /* Checks whether there are any objects in front of foreFront that can be added and 
         * update foreFrontOfPlayer and inFront list accordingly.
         */
        foreFrontUpdate(directionPush + 
            new Vector2(transform.position.x, transform.position.y));
        attachFoodToPlayer();
    }

    public Vector2 PosInfrontOfPushQueue()
    {
        /* Returns the position that is in front
         * of the front object that the player is pushing.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * Vector2: Position that is at the most front of the 
         *   push queue.
         */

        return new Vector2(foreFrontOfPlayer.transform.position.x, 
            foreFrontOfPlayer.transform.position.y) + directionPush;
    }

    public bool HasHeavyInFront()
    {
        /* Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool: True if there is a heavy in front of the queue,
         *   False otherwise.
         */

        return hasHeavy;
    }

    private void foreFrontUpdate(Vector2 startPosition)
    {
        /* Update the foreFrontOfPlayer and inFront list.
         * 
         * Parameters
         * ----------
         * 1) startPosition: position 1 step in front of this
         *   gameObject in the user/player specified direction.
         * 
         * Return
         * ------
         * 
         */

        int i = 0;
        Collider2D foreFront = Physics2D.OverlapPoint(startPosition, push);
        while (foreFront)
        {
            UpdateSharp(foreFront);
            UpdateHotCold(foreFront, i, startPosition);
            UpdateHasHeavy(foreFront);
            foreFrontOfPlayer = foreFront.gameObject;
            
            inFront.Add(foreFrontOfPlayer);
            foreFront = Physics2D.OverlapPoint(directionPush * ++i + startPosition, push);
        }
    }

    #endregion

    #region Attach/Detach food items to root

    private void attachFoodToPlayer()
    {
        /* Makes all food items in inFront a child of the player.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        foreach (GameObject food in inFront)
        {
            //make sure food items do not have any current children.
            food.GetComponent<DetachChildren>().detachAllChildren();
            food.transform.parent = transform;
        }
    }

    private void detachFoodFromPlayer()
    {
        /* Clear the inFrontOfPlayer list and detach all children. Update all their
         * destination position to its current transform position as well.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        GetComponent<DetachChildren>().detachAllChildren();
        inFront.Clear();
    }

    #endregion

    #region Attribute updates

    private void UpdateSharp(Collider2D foreFront)
    {
        /* Updates the food to become cut if there is a sharp object.
         * 
         * Parameters
         * ----------
         * 1) foreFront: collider of the gameObject all the way
         *   in front of the player in the user/player specified direction.
         * 
         * Return
         * ------
         * 
         */

        if (foreFront.GetComponent<Tags>().isSharp() && !foreFrontOfPlayer.GetComponent<Tags>().isKnife())
        {
            //Make food that pushed into the knife cut.
            foreFrontOfPlayer.GetComponent<Tags>().enableIsCut();
        }

        if (foreFrontOfPlayer.GetComponent<Tags>().isSharp() && !foreFront.GetComponent<Tags>().isKnife())
        {
            //Make food in front of the player knife cut.
            foreFront.GetComponent<Tags>().enableIsCut();
        }
    }

    private void UpdateHotCold(Collider2D foreFront, int indexPos, Vector2 startPosition)
    {
        /* Updates the food to become hot/cold. 
         * If there are both hot and cold adjacent to it, then it will be uncooked.
         * 
         * Parameters
         * ----------
         * 1) foreFront: collider of the gameObject all the way
         *   in front of the player in the user/player specified direction.
         *   
         * 2) indexPos: Index of the position in the foods that are being pushed by the player,
         *   starting from the foods closest to the player.
         * 
         * 3) startPosition: position 1 step in front of this
         *   gameObject in the user/player specified direction.
         *   
         * Return
         * ------
         * 
         */

        //Get item 2 items back (positionally), if there is, else null
        Collider2D prevByTwo = Physics2D.OverlapPoint(startPosition + directionPush * (indexPos - 2), push);
        if (foreFront.GetComponent<Tags>().isHot())
        {
            if (foreFrontOfPlayer.GetComponent<Tags>().isCold())
            {
                DestroyRoutine(foreFront);
            } 
            else if ((indexPos > 0 && prevByTwo.GetComponent<Tags>().isCold()) ||
                (indexPos == 0 && prevByTwo && prevByTwo.GetComponent<Tags>().isCold() &&
                prevByTwo.GetComponent<Tags>().isPlayer()))
            {
                //Middle becomes uncooked.
                foreFrontOfPlayer.GetComponent<Tags>().disableCooked();   
            } 
            else
            {
                foreFrontOfPlayer.GetComponent<Tags>().enableCooked();
            }
        } else if (foreFront.GetComponent<Tags>().isCold())
        {
            if (foreFrontOfPlayer.GetComponent<Tags>().isHot())
            {
                DestroyRoutine(foreFront);
            } else
            {
                foreFrontOfPlayer.GetComponent<Tags>().disableCooked();
            }
        } else
        {
            //foreFront is neither hot or cold. Temporarily assume to be the status of prev
            if (foreFrontOfPlayer.GetComponent<Tags>().isHot())
            {
                foreFront.GetComponent<Tags>().enableCooked();
            } else if (foreFrontOfPlayer.GetComponent<Tags>().isCold())
            {
                foreFront.GetComponent<Tags>().disableCooked();
            }
        }
    }

    private void UpdateHasHeavy(Collider2D foreFront)
    {
        /* Check if the front of the gameObject has any
         * heavy objects.
         * 
         * Parameters
         * ----------
         * 1) foreFront: collider of the gameObject all the way
         *   in front of the player in the user/player specified direction.
         * 
         * Return
         * ------
         * 
         */

        Tags foreFrontTags = foreFront.GetComponent<Tags>();
        if (foreFrontTags.isHeavy() && !foreFrontTags.isPlayer() &&
            !foreFrontTags.isInAnyTile()) hasHeavy = true;
    }

    private void DestroyRoutine(Collider2D foreFront)
    {
        /* Check if the front of the gameObject has any
         * objects that are going to be destroyed due to
         * hot/cold touching each other.
         * 
         * Parameters
         * ----------
         * 1) foreFront: collider of the gameObject all the way
         *   in front of the player in the user/player specified direction.
         * 
         * Return
         * ------
         * 
         */

        foreFront.GetComponent<DetachChildren>().detachAllChildren();
        foreFrontOfPlayer.GetComponent<DetachChildren>().detachAllChildren();
        foreFront.GetComponent<Tags>().enableInactive(); //Just deactivated so undo can reactivate it
        foreFrontOfPlayer.GetComponent<Tags>().enableInactive();

        if (inFront.Any())
        {
            inFront.RemoveAt(inFront.Count - 1);
            if (inFront.Any()) foreFrontOfPlayer = inFront[inFront.Count - 1];
        }
    }

    #endregion
}
