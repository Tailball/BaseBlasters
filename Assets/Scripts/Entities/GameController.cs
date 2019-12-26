using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(GameExploreState))]
[RequireComponent(typeof(GameCombatState))]
[RequireComponent(typeof(GameEnterState))]
[RequireComponent(typeof(PoolController))]
[RequireComponent(typeof(DeckController))]
public class GameController : MonoBehaviour
{
    //SINGLETON
    public static GameController instance;


    //UNITY LINKS
    [Header("Linking")]
    [SerializeField] GameObject ExploreUI = null;
    [SerializeField] GameObject CombatUI = null;
    [SerializeField] GameObject RoomHolder = null;

    [Header("Instances")]
    [SerializeField] GameObject SelectedPlayer = null; //This should change to character select that will pass on the selected playercontroller;

    [Header("Physics Layers")]
    [SerializeField] LayerMask DefFloorLayer = 0;
    [SerializeField] LayerMask DefWallLayer = 0;
    [SerializeField] LayerMask DefNPCLayer = 0;


    //MEMBERS (PRIVATE)
    GameStates _gameState;
    PlayStates _playState;
    
    GameExploreState _exploreState = null;
    GameCombatState _combatState = null;
    GameEnterState _enterState = null;
        
    List<RoomController> _roomsOnFloor = new List<RoomController>();
    RoomController _activeRoom;
    PlayerController _activePlayer;
    

    //ACCESSORS - MUTATORS (PUBLIC)
    public GameStates gameState {
        get { return _gameState; }
    }

    public PlayStates playState {
        get { return _playState; }
    }

    public ExploreStates passedExploreState {
        get { return _exploreState.state; }
    }

    public CombatStates passedCombatState {
        get { return _combatState.state; }
    }

    public LayerMask floorLayer {
        get { return DefFloorLayer; }
    }

    public LayerMask wallLayer {
        get { return DefWallLayer; }
    }

    public LayerMask npcLayer {
        get { return DefNPCLayer; }
    }

    public PlayerController activePlayer {
        get { return _activePlayer; }
    }

    public RoomController activeRoom {
        get { return _activeRoom; }
    }


    //UNITY LIFECYCLE
    void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this) {
            Destroy(gameObject);
        }

        _exploreState = GetComponent<GameExploreState>();
        _combatState = GetComponent<GameCombatState>();
        _enterState = GetComponent<GameEnterState>();
    }

    void Start() {
        this._gameState = GameStates.ContentGeneration; 
        changePlaystateToInitializing();
    }

    void Update() {
        switch(_gameState) {
            //Maybe these aren't necessary if we have a dedicated menu controller and dedicated ingame controller?
            case GameStates.MainMenu:
            case GameStates.AboutMenu:
            case GameStates.SettingsMenu:
            case GameStates.Credits:
                //Todo
            break;

            case GameStates.ContentGeneration:
                handleContentGeneration();
            break;

            case GameStates.Ingame:
                handlePlayState();
            break;
        }
    }


    //PRIVATE METHODS
    void handleContentGeneration() {
        //TODO
        _gameState = GameStates.Ingame;
    }

    void handlePlayState() {
        switch(_playState) {
            case PlayStates.Initializing:
                execInitialize();
            break;

            case PlayStates.Entering:
                _enterState.exec();
            break;

            case PlayStates.Exploring:
                _exploreState.exec();
            break;

            case PlayStates.Combat:
                _combatState.exec();
            break;

            case PlayStates.Exiting:
            break;

            case PlayStates.GameOver:
            break;
        }
    }

    void execInitialize() {
        initializePool();
        initializeFloor();
        initializePlayer();
        initializeDeck();

        _enterState.setCamera(_exploreState.cameraPerspective);
        _exploreState.setExploringCameraTarget(_activePlayer.gameObject);
        changePlaystateToEntering();
    }

    void initializePool() {
    }

    void initializeFloor() {
        var testRoom = Instantiate(PoolController.instance.getRoom(), Vector3.zero, Quaternion.identity, RoomHolder.transform);
        var roomController = testRoom.GetComponent<RoomController>();
        _roomsOnFloor.Add(roomController);
        _activeRoom = roomController;
        _activeRoom.enemies.instantiateEnemies();
    }

    void initializePlayer() {
        var player = Instantiate(SelectedPlayer, Vector3.zero, Quaternion.Euler(0, 180, 0));
        _activePlayer = player.GetComponent<PlayerController>();
    }

    void initializeDeck() {
        DeckController.instance.setStartSizeAndFillPool(6);
    }


    //PUBLIC METHODS
    public void changePlaystateToInitializing() {
        _playState = PlayStates.Initializing;
    }

    public void changePlaystateToEntering() {
        _enterState.enabled = true;
        _combatState.enabled = false;
        _exploreState.enabled = false;

        _playState = PlayStates.Entering;
    }

    public void changePlaystateToCombat(EnemyController withEnemy) {
        _enterState.enabled = false;
        _combatState.enabled = false;
        _exploreState.enabled = true;
        
        ExploreUI.SetActive(false);
        CombatUI.SetActive(true);

        _combatState.setActiveEnemy(withEnemy);
        _playState = PlayStates.Combat;
    }

    public void changePlaystateToExploring() {
        _enterState.enabled = false;
        _combatState.enabled = false;
        _exploreState.enabled = true;
        
        ExploreUI.SetActive(true);
        CombatUI.SetActive(false);

        _exploreState.setNewExploreRound();

        _playState = PlayStates.Exploring;
    }

    public void changePlaystateToGameOver() {
        Debug.Log("GAME OVER");
        _playState = PlayStates.GameOver;
    }

    public void acceptPlayerMove() {
        _combatState.acceptPlayerMove();
    }
}