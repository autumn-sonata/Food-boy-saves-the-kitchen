using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class YouManager : MonoBehaviour
{
    /* Manages which foods the player can control aka the You tile.
     */
    private Collider2D col; //Food item that is within YouTile.
    private List<GameObject> foodSameTag; //To change the player tags.
    private Collider2D oldCol;

    private GameObject playerCoordinator; //the main camera

    private void Awake()
    {
        playerCoordinator = GameObject.Find("Main Camera");
    }

    private void Start()
    {
        //Find gameObject on You tile.
        Vector2 youTilePosition = new Vector2(transform.position.x, transform.position.y);
        col = Physics2D.OverlapPoint(youTilePosition, LayerMask.GetMask("Push")); //Look for any pushable object on You Tile.
        if (col != null)
        {
            if (col.GetComponent<Tags>().isKnife())
            {
                //col is a knife. Make both of them as if they are on the you tile.
                GameObject other = col.GetComponent<SharpController>().getOtherComponent();
                other.GetComponent<Tags>().enableYouTileTag();
            }
            col.GetComponent<Tags>().enableYouTileTag();

            foodSameTag = GameObject.FindGameObjectsWithTag(col.GetComponent<Tags>().getFoodName()).ToList(); //Call "Knife" if it is Knife object.
            List<GameObject> additionalForKnife = new List<GameObject>();
            foreach (GameObject food in foodSameTag)
            {
                food.GetComponent<Tags>().enablePlayerTag();
                if (food.GetComponent<Tags>().isKnife())
                {
                    GameObject other = food.GetComponent<SharpController>().getOtherComponent();
                    other.GetComponent<Tags>().enablePlayerTag();
                    additionalForKnife.Add(other); //foodSameTag has both KnifeHilt and KnifeBlade types.
                }
            }
            foodSameTag.AddRange(additionalForKnife);
            oldCol = col;
        }
    }

    void Update()
    {
        if (col != null)
        {
            if (isOnTopOfTile())
            {
                /* There is a change in food items on the You tile.
                 * Pass info to PlayerMovementCoordinator.
                 * Wait until parent of col is at destination, then
                 * update all oldCol children destinations to their transform position.
                 */

                //Update all the Player and You tags, foodSameTag and destination of all objects.
                foreach (GameObject food in foodSameTag)
                {
                    food.GetComponent<DetachChildren>().detachAllChildren();
                    food.GetComponent<Tags>().disablePlayerTag();
                }
                oldCol.GetComponent<Tags>().disableYouTileTag();
                if (oldCol.GetComponent<Tags>().isKnife())
                {
                    oldCol.GetComponent<SharpController>().getOtherComponent().GetComponent<Tags>().disableYouTileTag();
                }

                col.GetComponent<Tags>().enableYouTileTag();
                if (col.GetComponent<Tags>().isKnife())
                {
                    col.GetComponent<SharpController>().getOtherComponent().GetComponent<Tags>().enableYouTileTag();
                }
                foodSameTag = GameObject.FindGameObjectsWithTag(col.GetComponent<Tags>().getFoodName()).ToList();
                List<GameObject> additionalForKnife = new List<GameObject>();
                foreach (GameObject food in foodSameTag)
                {
                    food.GetComponent<Tags>().enablePlayerTag();
                    if (food.GetComponent<Tags>().isKnife())
                    {
                        GameObject other = food.GetComponent<SharpController>().getOtherComponent();
                        other.GetComponent<Tags>().enablePlayerTag();
                        additionalForKnife.Add(other); //foodSameTag has both KnifeHilt and KnifeBlade types.
                    }
                }
                foodSameTag.AddRange(additionalForKnife);
                playerCoordinator.GetComponent<PlayerMovementCoordinator>().addAndRemovePlayers(foodSameTag);
                oldCol = col; //run this statement once every You tile food object change.
            }
            if (playerCoordinator.GetComponent<PlayerMovementCoordinator>().hasMoved())
            {
                //Update foodSameTag if there are any cut foods, and abandon them.
                foodSameTag.FindAll(food => !food.GetComponent<Tags>().isKnife() && food.GetComponent<Tags>().isCut())
                    .ForEach(food =>
                    {
                        food.GetComponent<DetachChildren>().detachAllChildren();
                        food.GetComponent<Tags>().disablePlayerTag();
                    });
                foodSameTag = foodSameTag.FindAll(food => !food.GetComponent<Tags>().isCut() || food.GetComponent<Tags>().isKnife());
            }
        }
    }

    public List<GameObject> playersAttached()
    {
        return foodSameTag;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*
         * Function runs when a new collider comes into contact with YouTile's collision box.
         * Delays change of foodSameTag array.
         */
        if (col != null)
        {
            oldCol = col;
            col = collision;
        } else
        {
            //Initialisation
            col = collision;
            oldCol = col;
        }
    }

    private bool isOnTopOfTile()
    {
        /* Function to tell when the player reaches its destination on top of You Tile
         * and if this statement has been run before on the same gameObject.
         */

        return col.GetComponent<PlayerManager>().isAtDestination() && col != oldCol;
    }
}
