using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
    public float playerSpeed = 9000f;
    public Transform destination;
    private float spriteSize;
    // Start is called before the first frame update
    void Start()
    {
        spriteSize = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
        destination.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        float horzPress = Input.GetAxisRaw("Horizontal");
        float vertPress = Input.GetAxisRaw("Vertical");

        //always move towards the destination.
        transform.position = Vector3.MoveTowards(transform.position, destination.position, playerSpeed * Time.deltaTime);
        
        //only change destination position in increments.
        if (Vector3.Distance(destination.position, transform.position) == 0f)
        {
            if (Mathf.Abs(horzPress) == 1f) 
            {
                destination.position = transform.position + new Vector3(horzPress * spriteSize, 0f, 0f);
            } else if (Mathf.Abs(vertPress) == 1f)
            {
                destination.position = transform.position + new Vector3(0f, vertPress * spriteSize, 0f);
            }
        }
    }
}
