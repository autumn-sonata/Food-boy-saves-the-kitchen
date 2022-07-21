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
    private List<GameObject> invisibleCache; //stores items that are invisible when holding tab
    private List<GameObject> tmpPlayers;
    private bool checkedMove;
    private bool isInitialise;
    private bool hasUndone;
    private bool playerCanMove;
    private bool winFound;

    private Timer playerTimer;
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
        players = new();
        playerTypes = new();
        tmpPlayers = new();
        invisibleCache = new();

        playerTimer = GetComponent<Timer>();
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
        hasUndone = false;
        playerCanMove = true;
        winFound = false;
        InitialiseAllAttributes(); //initialise cooked and cut
    }

    // Start is called before the first frame update
    void Start()
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

    private void Update()
    {
        /* Central call to all other managers.
         */
        if (inputManager.KeyHoldTab() && !invisibleCache.Any())
        {
            /* Make all movable objects that are currently visible
             * to be temporarily invisible as long as tab key is held.
             */
            int pushLayer = LayerMask.NameToLayer("Push");
            invisibleCache = FindObjectsOfType<GameObject>().ToList()
                .FindAll(obj => obj.layer == pushLayer);
            invisibleCache.ForEach(obj => obj.SetActive(false));
        } else if (!inputManager.KeyHoldTab())
        {
            if (invisibleCache.Any())
            {
                invisibleCache.ForEach(obj => obj.SetActive(true));
                invisibleCache.Clear();
            }

            PlayerRoutine();
            LevelTagPlayerUpdate(); //updates all the tags on the board.
            ConveyerBeltRoutine();
            LevelTagBeltUpdate();
        }
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

    public void WinFound()
    {
        /* Disable any movements of any player.
         */
        winFound = true;
    }

    private void InitialiseAllAttributes()
    {
        /* Initialise cut and cooked if stated.
         */
        foreach (AttributeInitialisation attr in FindObjectsOfType<AttributeInitialisation>())
        {
            attr.Initialise();
        }
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
        return allMovementsComplete() && !checkedMove;
    }

    private void executeWinManagers()
    {
        foreach (WinManager winTile in winTiles)
        {
            winTile.moveExecuted();
        }
    }

    private void checkWinConfig()
    {
        foreach (WinManager winTile in winTiles)
        {
            winTile.checkIfWin();
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
        if (allMovementsComplete() && playerCanMove)
        {
            if (tmpPlayers.Any())
            {
                tmpPlayers.ForEach(obj => {
                    obj.GetComponent<Tags>().disablePlayerTag();
                    obj.GetComponent<DetachChildren>().detachAllChildren();
                });
                players.RemoveWhere(player => !player.GetComponent<Tags>().isPlayer());
            }
            //Everyone has stopped movement, poll for new move through input.
            float horizontalMove = inputManager.KeyDirectionHorz();
            float verticalMove = inputManager.KeyDirectionVert();

            var list = new List<KeyValuePair<PlayerManager, Vector3>>();
            //Allow players that are not children to update their destination.
            foreach (GameObject player in players)
            { 
                if (Mathf.Abs(horizontalMove) == 1f && playerTimer.countdownFinished() && 
                    !player.GetComponent<Tags>().isInAnyTile() && !winFound)
                {
                    Vector2 direction = new(horizontalMove, 0f);
                    playerCanMove = false;
                    if (player.GetComponent<ObstacleManager>().allowedToMove(direction))
                        /* Needs to move itself and the other food items in front of it.
                            * Only done when all other destinations have been reached.
                            */
                        list.Add(new KeyValuePair<PlayerManager, Vector3>
                            (player.GetComponent<PlayerManager>(), new Vector3(horizontalMove, 0f, 0f)));
                }
                else if (Mathf.Abs(verticalMove) == 1f && playerTimer.countdownFinished() && 
                    !player.GetComponent<Tags>().isInAnyTile() && !winFound)
                {
                    Vector2 direction = new(0f, verticalMove);
                    playerCanMove = false;
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
        } else if (!allMovementsComplete())
        {
            playerTimer.startTimer(MoveDelay);
        }
    }

    private void ConveyerBeltRoutine()
    {
        /* Mimics player movement on the conveyer belt.
         * Applies same logic as moving an object as a player.
         * 
         * Uses "players" from playerConveyerBelt instead.
         * Only runs when there is a player update.
         */

        if (allMovementsComplete() && !playerCanMove && !isInitialise)
        {
            var list = new List<KeyValuePair<PlayerManager, Vector3>>();
            tmpPlayers.Clear();
            //Allow players that are not children to update their destination.
            foreach (ConveyerBeltManager belt in conveyerBelts)
            {
                float horizontalMove = belt.getPushDirectionHorz();
                float verticalMove = belt.getPushDirectionVert();
                Collider2D objOnTop = belt.getObjOnTop();
                if (Mathf.Abs(horizontalMove) == 1f && !winFound)
                {
                    Vector2 direction = new(horizontalMove, 0f);
                    if (objOnTop && objOnTop.GetComponent<ObstacleManager>().allowedToMove(direction))
                        /* Needs to move itself and the other food items in front of it.
                            * Only done when all other destinations have been reached.
                            */
                        list.Add(new KeyValuePair<PlayerManager, Vector3>
                            (objOnTop.GetComponent<PlayerManager>(), new Vector3(horizontalMove, 0f, 0f)));
                }
                else if (Mathf.Abs(verticalMove) == 1f && !winFound)
                {
                    Vector2 direction = new(0f, verticalMove);
                    if (objOnTop && objOnTop.GetComponent<ObstacleManager>().allowedToMove(direction))
                        /* Needs to move itself and the other food items in front of it.
                            * Only done when all other destinations have been reached.
                            */
                        list.Add(new KeyValuePair<PlayerManager, Vector3>
                            (objOnTop.GetComponent<PlayerManager>(), new Vector3(0f, verticalMove, 0f)));
                }
            }

            foreach (var kvp in list)
            {
                PlayerManager player = kvp.Key;
                Vector3 direction = kvp.Value;
                //Unconditionally move this object as it is on a conveyer belt.
                player.moveDestination(direction);
                player.GetComponent<Tags>().enablePlayerTag();
            }

            tmpPlayers = list.ConvertAll(obj => obj.Key.gameObject).FindAll(obj => !players.Contains(obj));
            players.UnionWith(tmpPlayers);
            playerCanMove = true;
        }
    }

    private void CheckInactive()
    {
        players.RemoveWhere(player => player.GetComponent<Tags>().isInactive());
    }

    private void CheckUndo()
    {
        if (hasUndone)
        {
            hasUndone = false;
            //update tiles
            foreach (TileManager tile in tiles)
            {
                tile.Initialise();
            }
        }
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
            if (!String.IsNullOrEmpty(tile.colFoodTag()))
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

    private void LevelTagPlayerUpdate()
    {
        CheckInactive();
        CheckUndo();

        if (hasMoved())
        {
            TagRoutine();
        }
        SpriteUpdate();
    }

    private void LevelTagBeltUpdate()
    {
        CheckInactive();
        CheckUndo();

        if (hasMoved())
        {
            TagRoutine();
            //ask moveManager to make all PastMovesRecords to record their moves.
            moveManager.RecordThisMove();
            checkedMove = true;
            isInitialise = false;
        }
        SpriteUpdate();
        checkWinConfig(); //check cut 
        UndoRestartRoutine();
    }

    private void TagRoutine()
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<DetachChildren>().detachAllChildren();
        }

        foreach (TileManager tile in tiles)
        {
            tile.TriggerTile(); //update collider
        }

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
    }

    private void UndoRestartRoutine()
    {
        //Undo and restart
        if (inputManager.KeyPressUndo() && !winFound)
        {
            hasUndone = true;
            moveManager.DoUndo();
            executeWinManagers(); //update Win Manager to previous state.
            SpriteUpdate();
            //update players and playerTypes to previous state
            players.Clear();
            playerTypes.Clear();
            foreach (TileManager youTile in youTiles)
            {
                players.UnionWith(youTile.playersAttached());
                if (youTile.hasFoodOnYouTile())
                {
                    incrementPlayer(youTile.colFoodTag());
                }
            }
        }
        if (inputManager.KeyPressRestart() && !winFound) moveManager.RestartLevel();
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

    private void DebugTags(string name)
    {
        /* For debugging tags. 
         * 
         * Parameters: name of object in scene
         */
        GameObject.Find(name).GetComponent<Tags>().printTags();
    }
}
