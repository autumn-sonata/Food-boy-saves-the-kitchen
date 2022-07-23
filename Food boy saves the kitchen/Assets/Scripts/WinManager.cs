using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinManager : MonoBehaviour
{
    /* Checks for the winning condition on the board.
     * Fails the player if there is no other way to win aka a reset or undo must be made
     * to further progress.
     */
    private GameObject[,] winConfig; //current winning configuration of the level
    private List<GameObject> foodSameTagTopLeft; //stores all foods that are the same tag as topLeft that is not on the win tile.
    private Vector2 topLeftCenteredCoord;
    private bool hasWon;

    private Vector2 topLeftCorner;
    private Vector2 bottomRightCorner;

    private LayerMask push;
    private GameObject winCanvas;
    private Coordinator coordinator;
    [SerializeField] private GameObject prefabOutline;

    private void Awake()
    {
        /* Prepare for reading the original win configuration.
         */
        push = LayerMask.GetMask("Push");
        winCanvas = GameObject.Find("CanvasWin");
        coordinator = GameObject.Find("Main Camera").GetComponent<Coordinator>();
        if (winCanvas)
        {
            winCanvas.SetActive(false);
        } else
        {
            Debug.LogError("Add CanvasWin prefab to scene.");
        }
        if (!coordinator) Debug.LogError("Add Main Camera to scene. " +
            "Check if Coordinator component is present.");
        hasWon = false;
        foodSameTagTopLeft = new List<GameObject>();
        int x = (int)Math.Ceiling(GetComponent<BoxCollider2D>().size.x);
        int y = (int)Math.Ceiling(GetComponent<BoxCollider2D>().size.y);
        winConfig = new GameObject[y, x];
        UpdateTileCorners();
        topLeftCenteredCoord = new Vector2(transform.position.x - x / 2f,
            transform.position.y + y / 2f) + new Vector2(0.5f, -0.5f);
        
        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                Collider2D withinWinTile = Physics2D.OverlapPoint(topLeftCenteredCoord + new Vector2(j, -i), push);
                if (withinWinTile)
                {
                    withinWinTile.GetComponent<Tags>().enableWinTileTag();
                    winConfig[i, j] = withinWinTile.gameObject;
                }
            }
        }

        Collider2D topLeft = Physics2D.OverlapPoint(topLeftCenteredCoord, push);
        if (topLeft)
        {
            GameObject[] topLeftFoods = GameObject.FindGameObjectsWithTag(topLeft.tag);
            foreach (GameObject food in topLeftFoods)
            {
                if (!food.GetComponent<Tags>().isInWinTile())
                {
                    foodSameTagTopLeft.Add(food);
                }
            }
        }
    }

    public void moveExecuted()
    {
        /* What to run when a move is being done.
         * 1) Get winning configuration on win tile.
         * 2) Search board for winning states.
         */
        updateWinCondition();
        if (winConfig[0, 0]) updateFoodSameTagTopLeft(winConfig[0, 0]);
        checkIfWin();
    }

    private void updateWinCondition()
    {
        /* Checks whether win condition has been updated every move. 
         * If so, update winConfig, foodSameTagTopLeft and topLeftCenteredCoord. 
         */

        //Check topLeftCenteredCoord since coord of winTile might have changed.
        topLeftCenteredCoord = new Vector2(transform.position.x - winConfig.GetLength(1) / 2f, 
            transform.position.y + winConfig.GetLength(0) / 2f) + new Vector2(0.5f, -0.5f);

        for (int row = 0; row < winConfig.GetLength(0); row++)
        {
            for (int col = 0; col < winConfig.GetLength(1); col++)
            {
                if ((winConfig[row, col] && new Vector2(winConfig[row, col].transform.position.x, 
                    winConfig[row, col].transform.position.y) != topLeftCenteredCoord + new Vector2(col, -row)) ||
                    !winConfig[row, col])
                {
                    //Replace if found that there is a different food there or to update to a possible
                    //new object from empty.
                    Collider2D updatedFood =
                        Physics2D.OverlapPoint(topLeftCenteredCoord + new Vector2(col, -row), push);
                    if (updatedFood)
                    {
                        winConfig[row, col] = updatedFood.gameObject;
                    } else
                    {
                        winConfig[row, col] = null;
                    }
                } 
            }
        }
    }

    public void checkIfWin()
    {
        /* Checks if there are any winning configurations in the level.
         * Does this by checking foodSameTagTopLeft and whether the relative configuration
         * matches winConfig.
         */

        foreach (GameObject food in foodSameTagTopLeft)
        {
            if (matchWinConfig(food) && !hasWon)
            {
                hasWon = true; //only ever run once for this scene.
                coordinator.WinFound();

                //Save state of the game
                AddLvlToCompleted();

                //Show where win configuration is in the scene.
                OutlineWinConfig(food);
            }
        }
    }

    private bool matchWinConfig(GameObject foodTopLeft)
    {
        /* Matches win configuration using the food name of each item.
         * Note that the win tile must be completely full before a winning condition 
         */
        Vector2 foodCoord = foodTopLeft.GetComponent<PlayerManager>().destination.position;
        for (int row = 0; row < winConfig.GetLength(0); row++)
        {
            for (int col = 0; col < winConfig.GetLength(1); col++)
            {
                Collider2D foodConfig = Physics2D.OverlapPoint(foodCoord + new Vector2(col, -row), push);

                if (!winConfig[row, col] || !foodConfig || foodConfig.GetComponent<Tags>().isInWinTile() ||
                    !foodConfig.CompareTag(winConfig[row, col].tag) || 
                    winConfig[row, col].GetComponent<Tags>().isCut() != foodConfig.GetComponent<Tags>().isCut() ||
                    winConfig[row, col].GetComponent<Tags>().isCooked() != foodConfig.GetComponent<Tags>().isCooked())
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
        /* Update the foodSameTagTopLeft list to new top left food item since it has changed.
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

    private void UpdateTileCorners()
    {
        topLeftCorner = new Vector2(transform.position.x - winConfig.GetLength(1) / 2f,
            transform.position.y + winConfig.GetLength(0) / 2f);
        bottomRightCorner = new Vector2(transform.position.x + winConfig.GetLength(1) / 2f,
            transform.position.y - winConfig.GetLength(0) / 2f);
    }

    public void ExitTrigger(Dictionary<string, bool[]> hotCold,
        List<HeavyManager> heavyTiles)
    {
        /* Checks if any winConfig elements exited the win tile.
         * If so, disable the win tile tag. Re-enable heavy and hot/cold
         * as necessary.
         * 
         * Current winConfig is outdated during execution of this function.
         */
        UpdateTileCorners();
        for (int row = 0; row < winConfig.GetLength(0); row++)
        {
            for (int col = 0; col < winConfig.GetLength(1); col++)
            {
                if (winConfig[row, col])
                {
                    Vector2 currPosition = winConfig[row, col].transform.position;
                    if (currPosition.x < topLeftCorner.x || bottomRightCorner.x < currPosition.x ||
                        currPosition.y > topLeftCorner.y || bottomRightCorner.y > currPosition.y)
                    {
                        GameObject movingOut = winConfig[row, col];
                        //out of win tile.
                        movingOut.GetComponent<Tags>().disableWinTileTag();

                        //re-enable all the previous tags that it has.
                        //Tags to re-enable include hot/cold, heavy, player
                        if (hotCold.ContainsKey(movingOut.tag))
                        {
                            bool[] hotColdStatus = hotCold[movingOut.tag];
                            if (hotColdStatus.All(status => status))
                            {
                                //Deactivate
                                movingOut.GetComponent<Tags>().enableInactive();
                            }
                            else if (hotColdStatus[0])
                            {
                                //is cold.
                                movingOut.GetComponent<Tags>().enableCold();
                            }
                            else if (hotColdStatus[1])
                            {
                                //is hot
                                movingOut.GetComponent<Tags>().enableHot();
                            } //otherwise stay disabled on hot and cold.
                        }

                        //re-enable heavy.
                        foreach (HeavyManager heavyTile in heavyTiles)
                        {
                            if (movingOut.CompareTag(heavyTile.colFoodTag()))
                                movingOut.GetComponent<Tags>().enableHeavy();
                        }
                    }
                }
            }
        }
    }

    public void EnterTrigger()
    {
        /* Checks if any new object is now in the win tile. 
         * If so, enable the win tile tag and remove children.
         * Disable heavy and hot/cold as necessary.
         */
        Collider2D[] winTileObj = 
            Physics2D.OverlapAreaAll(topLeftCorner + new Vector2(0.5f, -0.5f), 
            bottomRightCorner + new Vector2(-0.5f, 0.5f), push);

        foreach (Collider2D collider in winTileObj)
        {
            Tags colTags = collider.GetComponent<Tags>();
            colTags.enableWinTileTag();
            colTags.disableHeavy();
            colTags.disableHot();
            colTags.disableCold();
            collider.GetComponent<DetachChildren>().detachAllChildren();
        }
    }

    private void OutlineWinConfig(GameObject foodTopLeft)
    {
        /* Outlines the win configuration for the player to see.
         * Sequence of actions to do.
         */
        Vector2 topLeftPos = foodTopLeft.GetComponent<PlayerManager>().destination.transform.position;
        Vector2 middleWinConfig = new(topLeftPos.x + ((float)winConfig.GetLength(1) - 1) / 2,
            topLeftPos.y - ((float)winConfig.GetLength(0) - 1) / 2);
        //middleWinConfig is where the box should appear

        GameObject winOutline = Instantiate(prefabOutline);
        winOutline.transform.position = middleWinConfig;

        //Add flicker on and off functionality
        StartCoroutine(Flicker(winOutline));
    }

    private void AddLvlToCompleted()
    {
        /* Add level completion to PlayerInfo.
         */
        int levelNum = SceneManager.GetActiveScene().buildIndex - 5;
        //Tutorial level not counted.
        if (!FindObjectOfType<PlayerInfo>()) Debug.LogError("Start game from Main menu.");
        if (levelNum >= 1) FindObjectOfType<PlayerInfo>().completedLvl(levelNum);
    }

    private IEnumerator Flicker(GameObject winOutline)
    {
        SpriteRenderer render = winOutline.GetComponent<SpriteRenderer>();
        yield return new WaitForSeconds(0.3f);
        render.enabled = false;
        yield return new WaitForSeconds(0.3f);
        render.enabled = true;
        yield return new WaitForSeconds(2f);
        //CALL POP UP WINDOW
        winCanvas.SetActive(true);
    }
}
