using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBeltManager : MonoBehaviour
{
    /* Applies conveyer belt logic for conveyer belts.
     * 
     * All objects on the conveyer belt will move 1 step every move
     * in the direction specified. This direction of movement will
     * only occur after all normal player movements have been done.
     */

    //specifies the direction of movement of conveyer belt.
    public string direction; //either "left", "right", "up", down"
    private Vector2 directionPush; //specifies the direction of movement

    private LayerMask push;

    #region Unity specific functions

    private void Awake()
    {
        /* Initialisation of direction that the conveyer belt is 
         * pushing in.
         */

        push = LayerMask.GetMask("Push");

        //translate the direction of movement into directionPush.
        switch (direction)
        {
            case "left":
                directionPush = new Vector2(-1, 0);
                break;
            case "right":
                directionPush = new Vector2(1, 0);
                break;
            case "up":
                directionPush = new Vector2(0, 1);
                break;
            case "down":
                directionPush = new Vector2(0, -1);
                break;
            default:
                Debug.LogError("Invalid direction in ConveyerBeltManager. " +
                    "Refer to ConveyerBeltManager for instructions.");
                break;
        }
    }

    #endregion

    #region Getting conveyer belt information

    /* When movement is done, check whether there is any object on this tile.
     * Then update accordingly.
     */
    public Collider2D getObjOnTop()
    {
        /* Gets the current element which is on top of the conveyer belt.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * Collider2D: collider of the object on top of the conveyer belt.
         */

        return Physics2D.OverlapPoint(transform.position, push);
    }

    public float getPushDirectionHorz()
    {
        /* Determine conveyer belt's x-axis push.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * float: 1 if pushing to right, -1 if pushing to left,
         *   0 if not pushing within the x-axis.
         */

        return directionPush.x;
    }

    public float getPushDirectionVert()
    {
        /* Determine conveyer belt's y-axis push.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * float: 1 if pushing upwards, -1 if pushing downwards,
         *   0 if not pushing within the y-axis. 
         */

        return directionPush.y;
    }

    #endregion
}
