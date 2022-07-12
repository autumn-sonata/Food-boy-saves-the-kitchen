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

    private void Awake()
    {
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

    /* When movement is done, check whether there is any object on this tile.
     * Then update accordingly.
     */
    public Collider2D getObjOnTop()
    {
        /* Gets the current element which is on top of the conveyer belt.
         */
        return Physics2D.OverlapPoint(transform.position, push);
    }

    public float getPushDirectionHorz()
    {
        return directionPush.x;
    }

    public float getPushDirectionVert()
    {
        return directionPush.y;
    }
}
