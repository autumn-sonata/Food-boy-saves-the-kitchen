using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
    public float playerSpeed = 7f;
    public Transform destination;
    public LayerMask collisionHeavy;
    public LayerMask collisionPush;
    private float spriteSize;

    //Pushes object in 
    public void Collide(float horizontalMove, float verticalMove)
    {
        if (!Physics2D.OverlapCircle(destination.position + new Vector3(horizontalMove, verticalMove, 0f), .3f, collisionHeavy))
        {
            destination.position = transform.position + new Vector3(horizontalMove * spriteSize, verticalMove * spriteSize, 0f);
        }
        else if (!Physics2D.OverlapCircle(destination.position + new Vector3(horizontalMove, verticalMove, 0f), .3f, collisionPush))
        {
            Collide(horizontalMove, verticalMove);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        spriteSize = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
        destination.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.tag == "Player")
        {
            float horizontalMove = Input.GetAxisRaw("Horizontal");
            float verticalMove = Input.GetAxisRaw("Vertical");

            //moves object to destination
            transform.position = Vector3.MoveTowards(transform.position, destination.position, playerSpeed * Time.deltaTime);

            //checks for input and moves destination accordingly
            if (Vector3.Distance(destination.position, transform.position) == 0f)
            {
                if (Mathf.Abs(horizontalMove) == 1f)
                {
                    //checks for LayerMask of collisionType "Heavy" in destination, if present don't move
                    //if (!Physics2D.OverlapCircle(destination.position + new Vector3(horizontalMove, 0f, 0f), .3f, collisionHeavy))
                    //{
                    //    destination.position = transform.position + new Vector3(horizontalMove * spriteSize, 0f, 0f);
                    //}
                    Collide(horizontalMove, 0f);
                }
                else if (Mathf.Abs(verticalMove) == 1f)
                {
                    //if (!Physics2D.OverlapCircle(destination.position + new Vector3(0f, verticalMove * spriteSize, 0f), .3f, collisionHeavy))
                    //{
                    //    destination.position = transform.position + new Vector3(0f, verticalMove * spriteSize, 0f);
                    //}
                    Collide(0f, verticalMove);
                }
            }
        }
    }
}
