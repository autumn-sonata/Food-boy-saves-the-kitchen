using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Coordinator : MonoBehaviour
{
    /* Coordinates all player movements to be in sync. Only allows
     * for next set of movements once all players have finished moving.
     * 
     * Also coordinates which food items are to be players among you tiles.
     */
    private const float MoveDelay = 0.2f;

    private HashSet<GameObject> players; //stores every single player on the level. Managed by YouManager
    private Dictionary<string, int> playerTypes; //stores the food types of players on the level
    private bool checkedMove;
    private bool isInitialise;

    private Timer timer;
    private PastMovesManager moveManager;
    private InputManager inputManager;
    private List<SpriteManager> sprites;
    private List<WinManager> winTiles; //All instances of winTiles.
    private List<YouManager> youTiles; //All instances of youTiles.
    private List<ColdManager> coldTiles; //All instances of coldTiles.
    private List<HotManager> hotTiles; //All instances of hotTiles.
    private List<HeavyManager> heavyTiles; //All instances of heavyTiles.
    private List<TileManager> tiles; //All tiles excluding winTiles.
    private List<ConveyerBeltManager> conveyerBelts; //All conveyer belts.

    private void Awake()
    {
        //Initialisation of players and playerTypes
        players = new HashSet<GameObject>();
        playerTypes = new Dictionary<string, int>();

        timer = GetComponent<Timer>();
        moveManager = GameObject.Find("Canvas").GetComponent<PastMovesManager>();
        inputManager = GetComponent<InputManager>();
        sprites = GetAllSprites();
        winTiles = FindObjectsOfType<WinManager>().ToList();
        youTiles = FindObjectsOfType<YouManager>().ToList();
        coldTiles = FindObjectsOfType<ColdManager>().ToList();
        hotTiles = FindObjectsOfType<HotManager>().ToList();
        heavyTiles = FindObjectsOfType<HeavyManager>().ToList();
        tiles = new List<TileManager>();
        tiles = tiles.Concat(youTiles).Concat(coldTiles).Concat(hotTiles)
            .Concat(heavyTiles).ToList();
        conveyerBelts = FindObjectsOfType<ConveyerBeltManager>().ToList();
        checkedMove = false;
        isInitialise = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Get all players that can be moved.
        UpdatePlayerAttrInfo();
    }

    private void Update()
    {
        /* Central call to all other managers.
         */
        PlayerRoutine();
        CheckInactive();

        if (hasMoved())
        {
            foreach (TileManager tile in tiles)
            {
                tile.TriggerTile(); //update collider
            }

            //At this point, moved to new, oldCol is the one to reenable.
            //col is all updated already. just ask to change oldCol hot/cold 
            //properties immediately.

            /* Dictionary parameters -> [Tag name: [isCold?, isHot?]]
             * 
             * The hotCold hashmap represents the objects on hot and cold 
             * tiles and whether  
             */
            Dictionary<string, bool[]> hotCold = new();

            UpdateHotColdPlayers(hotCold);
 
            if (!isInitialise)
            {
                foreach (TileManager tile in tiles)
                {
                    /* Enable hot/cold and heavy again for food 
                     * items going out of tile
                     */
                    tile.ActivateDormantHotCold(hotCold);
                }
  
                //You, cold and hot tile tag + updates in case change of objects
                foreach (TileManager tile in tiles)
                {
                    tile.OldColUpdate(); //run all old col updates before new ones.
                }

                foreach (WinManager winTile in winTiles)
                {
                    winTile.ExitTrigger(hotCold, heavyTiles);
                }
                
                //Tag updates for new objects that stepped into different tiles
                foreach (WinManager winTile in winTiles)
                {
                    winTile.EnterTrigger();
                }

                foreach (TileManager tile in tiles)
                {
                    tile.NewColUpdate();
                }

                //Deactivate objects if both hot and cold.
                foreach (TileManager tile in tiles)
                {
                    tile.DeactivateDormantHotCold(hotCold);
                }
            }

            HeavyActivation();
            SpriteUpdate(); //deactivates gameobject if tags are appropriate.

            //Win tile updates
            executeWinManagers();

            //ask moveManager to make all PastMovesRecords to record their moves.
            moveManager.RecordThisMove();
            checkedMove = true;
            isInitialise = false;
        }

        //Undo and restart
        if (inputManager.KeyPressUndo())
        {
            moveManager.DoUndo();
            executeWinManagers(); //update Win Manager to previous state.
            SpriteUpdate();

            //update players and playerTypes to previous state
            players.Clear();
            playerTypes.Clear();
            UpdatePlayerAttrInfo();
        }
        if (inputManager.KeyPressRestart()) moveManager.RestartLevel();
    }

    public void decrementPlayer(string playerType)
    {
        /* Decrements value of playerType by 1 in playerTypes hashmap.
         */
        playerTypes[playerType]--;
    }

    public void incrementPlayer(string playerType)
    {
        /* Increments value of playerType by 1 in playerTypes hashmap.
         */
        if (!playerTypes.ContainsKey(playerType)) playerTypes.Add(playerType, 0);
            playerTypes[playerType]++;
    }

    public bool isPlayer(string playerType)
    {
        /* Checks whether the playerType is still a valid player
         * (due to other you tiles having the object on it)
         */
        return playerTypes[playerType] > 0;
    }

    public void addAndRemovePlayers(List<GameObject> foods)
    {
        /* To call to add and remove items from players that are no longer players.
         */
        players.RemoveWhere(food => !food.GetComponent<Tags>().isPlayer());
        players.UnionWith(foods); //player tag already enabled
    }

    private bool allMovementsComplete()
    {
        /* Checks whether the moves for all moving players are complete
         * by checking whether they are all at their destination.
         */
        if (players.Count() == 0 && playerTypes.Values.All(numPlayer => numPlayer == 0))
        {
            //No players, force stop music and undo/reset
            //Placeholder:
            Debug.LogError("No players detected! Game over.");
        }

        return !players.Any(food => !food.GetComponent<PlayerManager>().isAtDestination());
    }

    private bool hasMoved()
    {
        /* Ask if a move is done.
         */
        bool movementsComplete = allMovementsComplete();
        if (!movementsComplete) checkedMove = false;
        return movementsComplete && !checkedMove;
    }

    private void executeWinManagers()
    {
        foreach (WinManager winTile in winTiles)
        {
            winTile.moveExecuted();
        }
    }

    private void UpdatePlayerAttrInfo()
    {
        /* Updates players hashset and playerTypes hashmap.
         */
        foreach (TileManager tile in tiles)
        {
            tile.Initialise();
        }

        foreach (TileManager youTile in youTiles)
        {
            players.UnionWith(youTile.playersAttached());
            if (youTile.hasFoodOnYouTile())
            {
                incrementPlayer(youTile.colFoodTag());
            }
        }
    }

    private List<SpriteManager> GetAllSprites()
    {
        SpriteManager[] allObjects = FindObjectsOfType<SpriteManager>();
        return allObjects.ToList();
    }

    private void SpriteUpdate()
    {
        /* Update sprites to match tags.
         * However, if gameObject isInactive, then first: 
         * 1) Disable gameObjects
         * 2) Update sprites list.
         */

        List<SpriteManager> toDisable = sprites.FindAll(sprite => 
            sprite.GetComponent<Tags>().isInactive());
        toDisable.ForEach(sprite => sprite.gameObject.SetActive(false));

        foreach (SpriteManager sprite in sprites)
        {
            sprite.UpdateSprites();
        }
    }

    private void PlayerRoutine()
    {
        if (allMovementsComplete())
        {
            //Everyone has stopped movement, poll for new move through input.
            float horizontalMove = inputManager.KeyDirectionHorz();
            float verticalMove = inputManager.KeyDirectionVert();

            var list = new List<KeyValuePair<PlayerManager, Vector3>>();
            //Allow players that are not children to update their destination.
            //GameObject.Find("Tomato (2)").GetComponent<Tags>().printTags();
            foreach (GameObject player in players)
            {
                if (Mathf.Abs(horizontalMove) == 1f && timer.countdownFinished() && !player.GetComponent<Tags>().isInAnyTile())
                {
                    Vector2 direction = new Vector2(horizontalMove, 0f);
                    if (player.GetComponent<ObstacleManager>().allowedToMove(direction))
                        /* Needs to move itself and the other food items in front of it.
                            * Only done when all other destinations have been reached.
                            */
                        list.Add(new KeyValuePair<PlayerManager, Vector3>
                            (player.GetComponent<PlayerManager>(), new Vector3(horizontalMove, 0f, 0f)));
                }
                else if (Mathf.Abs(verticalMove) == 1f && timer.countdownFinished() && !player.GetComponent<Tags>().isInAnyTile())
                {
                    Vector2 direction = new Vector2(0f, verticalMove);
                    if (player.GetComponent<ObstacleManager>().allowedToMove(direction))
                        /* Needs to move itself and the other food items in front of it.
                            * Only done when all other destinations have been reached.
                            */
                        list.Add(new KeyValuePair<PlayerManager, Vector3>
                            (player.GetComponent<PlayerManager>(), new Vector3(0f, verticalMove, 0f)));
                }
            }

            foreach (var kvp in list)
            {
                PlayerManager player = kvp.Key;
                Vector3 direction = kvp.Value;
                if (!player.isChild() && player.GetComponent<Tags>().isPlayer())
                {
                    player.moveDestination(direction);
                }
            }
        } else
        {
            timer.startTimer(MoveDelay);
        }
    }

    private void CheckInactive()
    {
        players.RemoveWhere(player => player.GetComponent<Tags>().isInactive());
    }

    private void UpdateHotColdPlayers(Dictionary<string, bool[]> hotCold)
    {
        /* Initialises the hotCold hashmap.
         * Updates players based on hotCold.
         */
        foreach (TileManager hotTile in hotTiles)
        {
            if (!String.IsNullOrEmpty(hotTile.colFoodTag()))
                AddOrUpdateDict(hotCold, hotTile.colFoodTag(), new bool[] { false, true });
        }

        foreach (TileManager coldTile in coldTiles)
        {
            if (!String.IsNullOrEmpty(coldTile.colFoodTag()))
                AddOrUpdateDict(hotCold, coldTile.colFoodTag(), new bool[] { true, false });
        }

        List<GameObject> removeFromPlayer = new List<GameObject>();
        //Update players if there are destroys due to same food item being on hot and cold tile.
        foreach (GameObject player in players)
        {
            if (hotCold.ContainsKey(player.tag) && hotCold[player.tag].All(qn => qn))
            {
                //Both hot and cold. Remove from player from players.
                removeFromPlayer.Add(player);
            }
        }

        foreach (GameObject player in removeFromPlayer)
        {
            players.Remove(player);
        }
    }

    private void HeavyActivation()
    {
        /* After update of other tags, update heavy tags.
         */
        Tags[] objWithTag = FindObjectsOfType<Tags>();
        Dictionary<string, bool> heavyTags = new();
        foreach (TileManager tile in heavyTiles)
        {
            heavyTags.Add(tile.colFoodTag(), true);
        }

        foreach (Tags obj in objWithTag)
        {
            if (heavyTags.ContainsKey(obj.tag) && !obj.isPlayer() && !obj.isInAnyTile()) 
            {
                obj.enableHeavy();
            } else
            {
                obj.disableHeavy();
            }
        }
    }

    private static void AddOrUpdateDict(Dictionary<string, bool[]> dict, string key, bool[] value)
    {
        if (dict.ContainsKey(key))
        {
            bool[] lst = dict[key];
            if (lst.Length != value.Length)
            {
                Debug.LogError
                    ("Dictionary bool[] and provided value array length is not the same.");
            }

            for (int i = 0; i < lst.Length; i++)
            {
                if (!lst[i])
                    dict[key][i] = value[i];
            }
        } else
        {
            dict.Add(key, value);
        }
    }
}
