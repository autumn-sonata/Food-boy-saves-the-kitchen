using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyObstacleManager : MonoBehaviour
{
    /* Set rules for interaction between obstacles and food items (including player)
     * Is linked to the player.
     * 
     * Prevents the player from moving in a certain direction if there is a wall.
     */

    private LayerMask heavy;

    #region Unity specific functions
    private void Awake()
    {
        /* Initialise the heavy layermask.
         */

        heavy = LayerMask.GetMask("Heavy");
    }

    #endregion

    public bool isTouchingHeavy(Vector2 position)
    {
        /* Checks whether this position is on top of a heavy layer object
         * aka the walls surrounding each level.
         * 
         * Parameters
         * ----------
         * 1) position: 2D coordinate on the unity scene to check for 
         *   level bounding walls.
         * 
         * Return
         * ------
         * 
         */
        return Physics2D.OverlapCircle(position, 0.3f, heavy, 0f, 0f);
    }
}
