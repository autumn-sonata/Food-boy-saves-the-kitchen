using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObstacleManager : MonoBehaviour
{
    /* Manages all possible obstacles
     * 
     * Current obstacles include:
     * 1) Other food items
     * 2) Walls in the level
     */

    public bool allowedToMove(Vector2 direction)
    {
        /* Does a check on whether the player is able to move in that direction.
         */
        GetComponent<PushObstacleManager>().updateDirection(direction);

        return GetComponent<PushObstacleManager>().allOtherComponentsCanMove() &&
            !GetComponent<HeavyObstacleManager>()
                .isTouchingHeavy(GetComponent<PushObstacleManager>().PosInfrontOfPushQueue());
    }
}
