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
    private void Awake()
    {
        heavy = LayerMask.GetMask("Heavy");
    }

    public bool isTouchingHeavy(Vector2 position, Vector2 direction)
    {
        return Physics2D.OverlapCircle(position, 0.3f, heavy, 0f, 0f);
    }
}
