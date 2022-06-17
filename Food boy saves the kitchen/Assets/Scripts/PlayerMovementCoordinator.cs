using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovementCoordinator : MonoBehaviour
{
    /* Coordinates all player movements to be in sync. Only allows
     * for next set of movements once all players have finished moving.
     */
    List<GameObject> players;

    // Start is called before the first frame update
    void Start()
    {
        //Get all players that can be moved.
        GameObject[] youTiles = GameObject.FindGameObjectsWithTag("You");
        players = new List<GameObject>();
        foreach (GameObject youTile in youTiles)
        {
            GameObject onYouTile = youTile.GetComponent<YouManager>().currentObjectOnYouTile();
            GameObject[] sameTag = GameObject.FindGameObjectsWithTag(onYouTile.GetComponent<Tags>().getFoodName());
            players.AddRange(sameTag);
        }
    }

    public bool allMovementsComplete()
    {
        /* Checks whether the moves for all moving players are complete
         * by checking whether they are all at their destination.
         */

        return !players.Any(food => !food.GetComponent<PlayerManager>().isAtDestination());
    }

    public void addAndRemovePlayers(GameObject[] foods)
    {
        /* To call to add and remove items from players that are no longer players.
         */
        players.RemoveAll(food => !food.GetComponent<Tags>().isPlayer());
        players.AddRange(foods);
    }
}
