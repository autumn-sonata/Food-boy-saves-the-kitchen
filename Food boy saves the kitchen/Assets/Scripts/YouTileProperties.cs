using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouTileProperties : MonoBehaviour
{
    private GameObject[] foodSameTag;
<<<<<<< Updated upstream
    public string originalTag;
    private Collider2D col;
=======
    private Collider2D col; //Food item that is within YouTile.
>>>>>>> Stashed changes

    private void Start()
    {
        foodSameTag = new List<GameObject>().ToArray();
    }
    private void Update()
    {
<<<<<<< Updated upstream
        if (col != null && col.GetComponent<MovementPlayer>().destination.position == col.transform.position)
=======
        if (col != null && col.GetComponent<MovementPlayer>().isAtDestination())
>>>>>>> Stashed changes
        {
            //Alter tag of player and food
            foreach (GameObject food in foodSameTag)
            {
<<<<<<< Updated upstream
                food.tag = originalTag;
            }

            foodSameTag = GameObject.FindGameObjectsWithTag(col.tag);
            originalTag = col.tag;
            foreach (GameObject food in foodSameTag)
            {
                food.tag = "Player";
            }

            //Disable movement of gameObject on the You tile
            col.GetComponent<MovementPlayer>().enabled = false;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
=======
                food.GetComponent<FoodTags>().disablePlayerTag();
            }

            foodSameTag = GameObject.FindGameObjectsWithTag(col.GetComponent<FoodTags>().getFoodName());
            foreach (GameObject food in foodSameTag)
            {
                food.GetComponent<FoodTags>().enablePlayerTag();
            }

            //Disable movement of gameObject on the You tile
            col.GetComponent<MovementPlayer>().enabled = false;

        }
    }

    public GameObject getObjOnYouTile()
    {
        return col.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*
         * Enable movement of previous collider, and make col the new food item coming into YouTile.
         */
>>>>>>> Stashed changes
        if (col != null)
        {
            col.GetComponent<MovementPlayer>().enabled = true;
        }
        col = collision;
    }
}

