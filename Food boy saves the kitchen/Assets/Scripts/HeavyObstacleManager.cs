using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyObstacleManager : MonoBehaviour
{
    /* Set rules for interaction between obstacles and food items (including player)
     * Is linked to the player.
     */
    private LayerMask heavy;
    private void Awake()
    {
        heavy = LayerMask.GetMask("Heavy");
    }

    public bool isTouchingHeavy(Vector2 position)
    {
        //checks whether an object is touching this heavy object.
        return Physics2D.OverlapCircle(position, 0.3f, heavy, 0f, 0f);
    }
}
