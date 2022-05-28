using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouTileProperties : MonoBehaviour
{
    //private readonly int pushLayer = 7;
    // Start is called before the first frame update
    private GameObject parentFood;
    private GameObject[] playerFoods;

    private void OnTriggerEnter2D(Collider2D col)
    {
        parentFood = col.gameObject;
        playerFoods = GameObject.FindGameObjectsWithTag(parentFood.gameObject.tag);

        foreach (GameObject food in playerFoods)
        {
            //Goes to Tags(script) component and changes element1 to Player
            food.GetComponent<Tags>().objectTags[1] = "Player";
        }

        //Prevent parentFood from moving
        parentFood.GetComponent<MovementPlayer>().enabled = false;
    }
    
    void OnTriggerExit2D(Collider2D col)
    {
        parentFood = col.gameObject;
        playerFoods = GameObject.FindGameObjectsWithTag(parentFood.gameObject.tag);

        foreach (GameObject food in playerFoods)
        {
            food.GetComponent<Tags>().objectTags[1] = ".g";
        }
        parentFood.GetComponent<MovementPlayer>().enabled = true;
    }
}
