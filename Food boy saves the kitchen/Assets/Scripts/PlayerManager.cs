using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    /* This class takes care of how the player moves. 
     * Does this by reading the input of the player only at their destination
     */

    public const float PlayerSpeed = 7f;
    public Transform destination;
    private float spriteSize;

    private void Awake()
    {
        destination.SetParent(null);
        spriteSize = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        if (GetComponent<Tags>().isPlayer() && !isChild())
        {
            transform.position = Vector3.MoveTowards(transform.position, destination.position, spriteSize * Time.deltaTime * PlayerSpeed);
        }
    }

    public bool isAtDestination()
    {
        /* Checks if the player has finished moving one tile movement, via most senior parent or itself (if is player).
         */
        return transform.root.GetComponent<PlayerManager>().destination.position ==
            transform.root.transform.position;
    }

    public void updateDestinationToCurrPosition()
    {
        destination.position = transform.position;
    }

    public bool isChild()
    {
        /* True if the gameObject is a child of another gameObject.
         */
        return transform.root != transform;
    }

    public void moveDestination(Vector3 direction)
    {
        destination.position += direction;
    }
}
