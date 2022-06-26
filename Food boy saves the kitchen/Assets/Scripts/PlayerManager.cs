using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    /* This class takes care of how the player moves. 
     * Does this by reading the input of the player only at their destination
     */

    public float playerSpeed = 7f;
    public Transform destination;
    private float spriteSize;

    void Awake()
    {
        destination.SetParent(null);
        spriteSize = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Receive input from InputManager only when objects are at their destination. Only update
        // destination position

        if (GetComponent<Tags>().isPlayer() && !isChild())
        {
            transform.position = Vector3.MoveTowards(transform.position, destination.position, spriteSize * Time.deltaTime * playerSpeed);
            if (isAtDestination())
            {
                //poll for a new destination location.
                float horizontalMove = GetComponent<InputManager>().KeyDirectionHorz();
                float verticalMove = GetComponent<InputManager>().KeyDirectionVert();
                if (Mathf.Abs(horizontalMove) == 1f && GetComponent<Timer>().countdownFinished() && !GetComponent<Tags>().isInAnyTile())
                {
                    Vector2 direction = new Vector2(horizontalMove, 0f);
                    if (GetComponent<ObstacleManager>().allowedToMove(direction))
                        //Needs to move itself and the other food items in front of it
                        destination.position += new Vector3(horizontalMove, 0f, 0f);
                }
                else if (Mathf.Abs(verticalMove) == 1f && GetComponent<Timer>().countdownFinished() && !GetComponent<Tags>().isInAnyTile())
                {
                    Vector2 direction = new Vector2(0f, verticalMove);
                    if (GetComponent<ObstacleManager>().allowedToMove(direction))
                        //Needs to move itself and the other food items in front of it
                        destination.position += new Vector3(0f, verticalMove, 0f);
                }
            }
            else
            {
                GetComponent<Timer>().startTimer(0.2f);
            }
        }

    }

    public bool isAtDestination()
    {
        /* Checks if the player has finished moving one tile movement, via parent or itself (if is player).
         */
        if (isChild())
            return transform.parent.GetComponent<PlayerManager>().destination.position ==
                transform.parent.transform.position;
        return destination.position == transform.position;
    }

    public void updateDestinationToCurrPosition()
    {
        destination.position = transform.position;
    }

    private bool isChild()
    {
        /* True if the gameObject is a child of another gameObject.
         */
        return transform.parent != null;
    }
}
