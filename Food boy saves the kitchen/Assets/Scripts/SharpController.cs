using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpController : MonoBehaviour
{
    /* Controls the other sharp component of the knife if this is being moved.
     * 
     * Behaves exactly like a food object. However, it has the ability to become
     * a player if the other component of the blade is being moved.
     */
    public int componentDirection; //NOTE FOR DIRECTION = 1:up, 2:right, 3:down, 4:left
    private GameObject otherComponent; //either the hilt or the blade, whichever is opposite of this

    // Start is called before the first frame update
    void Awake()
    {
        LayerMask push = LayerMask.GetMask("Push");
        Vector2 otherCompCoordinate;
        Vector2 thisPosition = transform.position;
        switch (componentDirection)
        {
            case 1:
                otherCompCoordinate = thisPosition + new Vector2(0, 1);
                break;
            case 2:
                otherCompCoordinate = thisPosition + new Vector2(1, 0);
                break;
            case 3:
                otherCompCoordinate = thisPosition + new Vector2(0, -1);
                break;
            default:
                otherCompCoordinate = thisPosition + new Vector2(-1, 0);
                break;
        }

        otherComponent = Physics2D.OverlapPoint(otherCompCoordinate, push).gameObject;
    }

    public void makeParentOfOtherComponent()
    {
        /* If this is a player or this is being moved by a player, make other component
         * and its pushables a child of this and make it a player as well.
         */
        otherComponent.transform.SetParent(transform);
    }

    public GameObject getOtherComponent()
    {
        return otherComponent;
    }

    public bool canMove(Vector2 directionPush)
    {
        return GetComponent<ObstacleManager>().allowedToMove(directionPush);
    }

    public bool sameOrientation(Vector2 direction)
    {
        /* Returns true when the orientation of the knife is similar to the direction
         * of movement.
         * To prevent stack overflow errors.
         */
        if (componentDirection % 2 == 0)
        {
            return direction.y == 0;
        }
        return direction.x == 0;
    }
}
