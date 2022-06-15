using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinManager : MonoBehaviour
{
    /* Checks for the winning condition on the board.
     * Fails the player if there is no other way to win aka a reset or undo must be made
     * to further progress.
     */
    private GameObject[,] winConfig; //current winning configuration of the level
    private List<GameObject> foodSameTagTopLeft; //stores all foods that are the same tag as topLeft that is not on the win tile.
    private Vector2 topLeftCenteredCoord;
    private GameObject playerTrackMovement; //tracks whether a move has taken place.
    private Vector3 destChecked;

    private LayerMask push;
    private GameObject[] youTile;

    // Start is called before the first frame update
    void Start()
    {
        /* Prepare for reading the original win configuration.
         */
        push = LayerMask.GetMask("Push");
        youTile = GameObject.FindGameObjectsWithTag("You");
        foodSameTagTopLeft = new List<GameObject>();

        int x = (int)Math.Ceiling(GetComponent<BoxCollider2D>().size.x);
        int y = (int)Math.Ceiling(GetComponent<BoxCollider2D>().size.y);
        winConfig = new GameObject[y, x];
        topLeftCenteredCoord = new Vector2(transform.position.x - x / 2f, 
            transform.position.y + y / 2f) + new Vector2(0.5f, -0.5f);

        GameObject topLeft = Physics2D.OverlapPoint(topLeftCenteredCoord, push).gameObject;
        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                GameObject withinWinTile = Physics2D.OverlapPoint(topLeftCenteredCoord + new Vector2(j, i), push).gameObject;
                withinWinTile.GetComponent<Tags>().enableWinTileTag();
                winConfig[i, j] = withinWinTile;
            }
        }

        GameObject[] topLeftFoods = GameObject.FindGameObjectsWithTag(topLeft.tag);
        foreach (GameObject food in topLeftFoods)
        {
            if (!food.GetComponent<Tags>().isInWinTile())
            {
                foodSameTagTopLeft.Add(food);
            }
        }

        findNewPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasMoved())
        {
            Debug.Log("Run");
            /* 1) Get winning configuration on win tile.
             * 2) Search board for winning states.
             */
            updateWinCondition();
            checkIfWin();
        }
    }

    private void updateWinCondition()
    {
        /* Checks whether win condition has been updated. 
         * If so, update winConfig, foodSameTagTopLeft and topLeftCenteredCoord. 
         */
        Vector2 currTopLeftCenteredCoord = new Vector2(transform.position.x - winConfig.GetLength(1) / 2f, 
            transform.position.y + winConfig.GetLength(0) / 2f) + new Vector2(0.5f, -0.5f);
        if (topLeftCenteredCoord != currTopLeftCenteredCoord)
        {
            topLeftCenteredCoord = currTopLeftCenteredCoord;
        }

        for (int row = 0; row < winConfig.GetLength(0); row++)
        {
            for (int col = 0; col < winConfig.GetLength(1); col++)
            {
                Vector2 currPosition = winConfig[row, col].transform.position;
                if (currPosition != topLeftCenteredCoord + new Vector2(col, row))
                {
                    //Only replace if found that there is a different food there.
                    GameObject updatedFood = 
                        Physics2D.OverlapPoint(topLeftCenteredCoord + new Vector2(row, col), push).gameObject;
                    winConfig[row, col] = updatedFood;
                    if (row == 0 && col == 0)
                    {
                        updateFoodSameTagTopLeft(updatedFood);
                    }
                }
            }
        }
    }

    private void checkIfWin()
    {
        /* Checks if there are any winning configurations in the level.
         * Does this by checking foodSameTagTopLeft and whether the relative configuration
         * matches winConfig.
         */

        foreach (GameObject food in foodSameTagTopLeft)
        {
            if (matchWinConfig(food))
            {
                Debug.Log("You Win!");
            }
        }
    }

    private bool matchWinConfig(GameObject food)
    {
        Vector2 foodCoord = food.transform.position;
        for (int row = 0; row < winConfig.GetLength(0); row++)
        {
            for (int col = 0; col < winConfig.GetLength(1); col++)
            {
                Collider2D foodConfig = Physics2D.OverlapPoint(foodCoord + new Vector2(col, row), push);
                if (foodConfig == null || winConfig[row, col].GetComponent<Tags>().getFoodName() != 
                    foodConfig.GetComponent<Tags>().getFoodName())
                {
                    //Configuration is wrong
                    return false;
                }
            }
        }

        return true;
    }

    private void updateFoodSameTagTopLeft(GameObject topLeft)
    {
        /* Update the foodSameTagTopLeft list to new top left food item if it has changed.
         */
        foodSameTagTopLeft.Clear();

        foreach (GameObject food in GameObject.FindGameObjectsWithTag(topLeft.tag))
        {
            if (!food.GetComponent<Tags>().isInWinTile())
            {
                foodSameTagTopLeft.Add(food);
            }
        }
    }

    private bool hasMoved()
    {
        /* 1) Finds a player that is movable (not on any tile). Keep track if this player
         * is movable. If not, find another player that is movable.
         * 
         * 2) Ask if a move is done
         */
        if (playerTrackMovement.GetComponent<Tags>().isInAnyTile() || playerTrackMovement.transform.parent != null)
        {
            //Change playerTrackMovement
            findNewPlayer();
        }
        
        return playerTrackMovement.GetComponent<PlayerManager>().isAtDestination() && 
            !checkedAtThisDestination();
    }

    private void findNewPlayer()
    {
        /* Updates playerTrackMovement and finds another valid player.
         */
        foreach (GameObject onYouTile in youTile)
        {
            //MUST CONFIRM THAT THERE IS A MOVABLE PLAYER FOOD AT THE START OF THE LEVEL!
            GameObject[] sameTag = 
                GameObject.FindGameObjectsWithTag(onYouTile.GetComponent<YouManager>().currentObjectOnYouTile().tag);
            foreach (GameObject food in sameTag)
            {
                if (!food.GetComponent<Tags>().isInAnyTile() && food.transform.parent == null)
                {
                    playerTrackMovement = food;
                    break;
                }
            }

            if (playerTrackMovement != null)
            {
                break;
            }
        }
    }

    private bool checkedAtThisDestination()
    {
        /* Check if SPRITE destination of player has been updated.
         */
        Vector3 currPosPlayer = playerTrackMovement.GetComponent<PlayerManager>().destination.position;
        if (destChecked == null || destChecked != currPosPlayer)
        {
            //Need to check this location
            destChecked = currPosPlayer;
            return false;
        }
        return true;
    }
}
