using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouTileProperties : MonoBehaviour
{
    private GameObject[] foodSameTag;
    private Collider2D col; //Food item that is within YouTile.

    private void Start()
    {
        foodSameTag = new List<GameObject>().ToArray();
    }
    private void Update()
    {

        if (col != null && col.GetComponent<MovementPlayer>().isAtDestination())
        {
            //Alter tag of player and food
            foreach (GameObject food in foodSameTag)
            {

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
        if (col != null)
        {
            col.GetComponent<MovementPlayer>().enabled = true;
        }
        col = collision;
    }
}
