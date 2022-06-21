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
    private List<GameObject> otherForeFrontParents; //store other forefront parents that come from eg knife.

    // Start is called before the first frame update
    void Start()
    {
        foreFrontOfPlayer = gameObject;
        push = LayerMask.GetMask("Push");
        directionPush = new Vector2(0f, 0f);
        inFront = new List<GameObject>();
        otherForeFrontParents = new List<GameObject>();
    }

    public bool hasPushInFront()
    {
        /* Checks whether there is any food objects in front of the player 
         * that is pushed by the player.
         */
        return inFront.Count != 0;
    }

    public void updateDirection(Vector2 direction)
    {
        /* Updates direction player is moving within PushObstacleManager.
         * Includes updating:
         * 1) foreFrontOfPlayer
         * 2) directionPush
         * 3) inFront list
         */

        //forget previous inFront list

        if (isSameDirection(direction))
        {
            // No need to attach or detach food unless there is suddenly something in front of 
            // foreFrontOfPlayer that is a push object.
            foreFrontUpdateSameDir(); //handles foreFrontOfPlayer and inFront list.
        } else
        {
            //There is a change of direction, disassociate what children currently handling and 
            //attach potential new children to the player.
            foreFrontUpdateDiffDir(direction);
        }
    }

    private void foreFrontUpdateDiffDir(Vector2 direction)
    {
        /* Function handles detachment of old children, and updates PushObstacleManager
         * variables with new information.
         */
        foreFrontOfPlayer = gameObject;
        directionPush = direction;
        detachFoodFromPlayer();

        foreFrontUpdate(directionPush + new Vector2(transform.position.x, transform.position.y));
        attachFoodToPlayer();
    }

    private void foreFrontUpdateSameDir()
    {
        /* Function to run when a same direction is pressed.
         * Checks whether there are any objects in front of foreFront that can be added and 
         * update foreFrontOfPlayer and inFront list accordingly.
         */

        foreFrontUpdate(PosInfrontOfPushQueue());
        attachFoodToPlayer();
    }

    private void foreFrontUpdate(Vector2 startPosition)
    {
        /* Update the foreFrontOfPlayer and inFront list.
         */
        int i = 0;
        Collider2D foreFront = Physics2D.OverlapPoint(startPosition, push);
        while (foreFront)
        {
            if (foreFront.GetComponent<Tags>().isSharp())
            {
                foreFrontOfPlayer.GetComponent<Tags>().enableIsCut();
            }
            foreFrontOfPlayer = foreFront.gameObject;
            if (foreFrontOfPlayer.GetComponent<Tags>().isKnife() &&
                !foreFrontOfPlayer.GetComponent<SharpController>().sameOrientation(directionPush))
            {
                //Attach entirety of other component and its pushables to this object.
                foreFrontOfPlayer.GetComponent<SharpController>().makeParentOfOtherComponent();
                otherForeFrontParents.Add(foreFrontOfPlayer.GetComponent<SharpController>().getOtherComponent());
            }

            inFront.Add(foreFrontOfPlayer);
            foreFront = Physics2D.OverlapPoint(directionPush * ++i + startPosition, push);
        }
    }

    private void attachFoodToPlayer()
    {
        /* Makes all food items in inFront a child of the player.
         */
        foreach (GameObject food in inFront)
        {
            food.transform.parent = transform;
        }
    }

    public void detachFoodFromPlayer()
    {
        //Clear the inFrontOfPlayer list and detach all children. Update all their
        //destination position to its current transform position as well.
        GetComponent<DetachChildren>().detachAllChildren();
        inFront.Clear();
        otherForeFrontParents.Clear();
    }

    public Vector2 PosInfrontOfPushQueue()
    {
        /* Returns the position that is in front
         * of the front object that the player is pushing.
         */

        return new Vector2(foreFrontOfPlayer.transform.position.x, foreFrontOfPlayer.transform.position.y) + directionPush;
    }

    public bool allOtherComponentsCanMove()
    {
        return otherForeFrontParents.All(parent => parent.GetComponent<SharpController>().canMove(directionPush));
    }

    private bool isSameDirection(Vector2 newDirection)
    {
        /* Checks whether the player is moving in the same direction as previous move.
         */
        return directionPush == newDirection;
    }
}
