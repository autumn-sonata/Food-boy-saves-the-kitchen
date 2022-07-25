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
         * 
         * Parameters
         * ----------
         * 1) direction: Get the direction of movement as dictated by the 
         *   user/player by keyboard input.
         * 
         * Return
         * ------
         * bool: True if movement of player in direction of movement is possible,
         *   False otherwise.
         */

        GetComponent<PushObstacleManager>().updateDirection(direction);

        return !GetComponent<HeavyObstacleManager>().isTouchingHeavy
            (GetComponent<PushObstacleManager>().PosInfrontOfPushQueue()) &&
            !GetComponent<PushObstacleManager>().HasHeavyInFront();
    }
}
