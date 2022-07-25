using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    /* This class takes care of how the player moves. 
     * Does this by reading the input of the player only at their destination
     */

    public const float PlayerSpeed = 10f;
    public Transform destination;
    private float spriteSize;

    #region Unity specific functions

    private void Awake()
    {
        /* Initialises spriteSize and destination position of the gameObject to 
         * not follow the gameObject itself.
         */

        destination.SetParent(null);
        spriteSize = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        /* Constantly moves the player to the supposed destination position
         */

        if (GetComponent<Tags>().isPlayer() && !isChild())
        {
            //Constantly moves players that are supposed to move to their destination.
            transform.position = Vector3.MoveTowards(transform.position, destination.position, spriteSize * Time.deltaTime * PlayerSpeed);
        }
    }

    #endregion

    #region Destination functions

    public bool isAtDestination()
    {
        /* Checks if the player has finished moving one tile movement, via most senior parent or itself (if is player).
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool: True if the root gameObjects destination is reached,
         *   False otherwise.
         */

        return transform.root.GetComponent<PlayerManager>().destination.position ==
            transform.root.transform.position;
    }

    public void updateDestinationToCurrPosition()
    {
        /* Updates the destination position to current position.
         * 
         * Method is to be used by Undo.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */
        destination.position = transform.position;
    }

    public void moveDestination(Vector3 direction)
    {
        /* Move the destination based on the direction given,
         * so that the objects themselves will follow suit.
         * 
         * Parameters
         * ----------
         * 1) direction: direction of movement as dictated
         *   by the user/player.
         * 
         * Return
         * ------
         * 
         */

        destination.position += direction;
    }

    #endregion

    #region Miscellaneous

    public bool isChild()
    {
        /* True if the gameObject is a child of another gameObject.
         */

        return transform.root != transform;
    }

    #endregion
}
