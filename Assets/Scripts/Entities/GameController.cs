using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameController : MonoBehaviour
{
    //SINGLETON
    public static GameController instance;


    //UNITY LINKS
    [Header("Linking")]
    [SerializeField] GameObject MainUi = null;
    [SerializeField] Camera CameraOrthographic = null;
    [SerializeField] Camera CameraPerspective = null;
    [SerializeField] TMP_Text txtRoundNum = null;
    [SerializeField] GameObject RoomHolder = null;

    [Header("Instances")]
    [SerializeField] GameObject SelectedPlayer = null; //This should change to character select that will pass on the selected playercontroller;
    
    [Header("Tweaking")]
    [SerializeField][Range(1f, 5f)] float EnterSpeed = 1f;

    [Header("Physics Layers")]
    [SerializeField] LayerMask DefFloorLayer;
    [SerializeField] LayerMask DefWallLayer;


    //MEMBERS (PRIVATE)
    GameStates _gameState;
    PlayStates _playState;
    CombatStates _combatState;
    ExploreStates _exploreState;
    
    List<RoomController> _roomsOnFloor = new List<RoomController>();
    RoomController _activeRoom;
    PlayerController _activePlayer;

    int _round = 0;
    

    //ACCESSORS - MUTATORS (PUBLIC)
    public GameStates gameStates {
        get { return _gameState; }
    }

    public PlayStates playState {
        get { return _playState; }
    }

    public CombatStates combatState {
        get { return _combatState; }
    }

    public ExploreStates exploreState {
        get { return _exploreState; }
    }

    public LayerMask floorLayer {
        get { return DefFloorLayer; }
    }

    public LayerMask wallLayer {
        get { return DefWallLayer; }
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
    }

    void Start() {
        MainUi.SetActive(false);

        this._gameState = GameStates.ContentGeneration; 
        this._playState = PlayStates.Initializing;
        this._combatState = 0;
        this._exploreState = ExploreStates.PlayerMoving;
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
                initialize();
            break;

            case PlayStates.Entering:
                enter();
            break;

            case PlayStates.Exploring:
                getExploringInput();
                explore();
            break;

            case PlayStates.Combat:
            break;

            case PlayStates.Exiting:
            break;
        }
    }

    void initialize() {
        initializePool();
        initializeFloor();
        initializePlayer();
        initializeDeck();

        CameraOrthographic.GetComponent<CameraController>().setTarget(_activePlayer.gameObject);
        CameraPerspective.GetComponent<CameraController>().setTarget(_activePlayer.gameObject);

        _playState = PlayStates.Entering;
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
        var dInst = DeckController.instance;
        dInst.setStartSizeAndFillPool(12);

        Debug.Log($"initialized. deck size: {dInst.amountOfCards}");

        Debug.Log($"drawing 6 cards...");
        var cardhand = dInst.drawCards(6);
        for(var i = 0; i < cardhand.Count; i++) {
            Debug.Log($"card {i+1}: {cardhand[i].name}");
        }
        
        Debug.Log($"drawn. deck size: {dInst.amountOfCards}");

        Debug.Log($"drawing 8 cards...");
        var cardhand2 = dInst.drawCards(8);
        for(var i = 0; i < cardhand2.Count; i++) {
            Debug.Log($"card {i+1}: {cardhand2[i].name}");
        }

        Debug.Log($"drawn. deck size: {dInst.amountOfCards}");

        Debug.Log($"drawing 6 cards...");
        var cardhand3 = dInst.drawCards(8);
        for(var i = 0; i < cardhand3.Count; i++) {
            Debug.Log($"card {i+1}: {cardhand3[i].name}");
        }

        Debug.Log($"drawn. deck size: {dInst.amountOfCards}");
    }

    void enter() {
        CameraPerspective.nearClipPlane -= Time.deltaTime * EnterSpeed;
        if(CameraPerspective.nearClipPlane <= 3.8f) {
            CameraPerspective.nearClipPlane = 3.8f;
            _playState = PlayStates.Exploring;
            
            setNewRound();
            MainUi.SetActive(true);
        }
    }

    void getExploringInput() {
        if(_playState == PlayStates.Exploring) {
            if(Input.GetKeyDown(KeyCode.C)) {
                switchCamera();
            }
        }
    }

    void switchCamera() {
        CameraOrthographic.gameObject.SetActive(!CameraOrthographic.gameObject.activeInHierarchy);
        CameraPerspective.gameObject.SetActive(!CameraPerspective.gameObject.activeInHierarchy);
    }

    void explore() {
        switch(_exploreState) {
            case ExploreStates.PlayerMoving:
                var playerMover = _activePlayer.movementData;
                if(playerMover.hasMadeAMoveThisTurn && !playerMover.isMoving) {
                    checkForCombat();
                    _activeRoom.enemies.setAllEnemiesExploreAction(_activePlayer.transform.position);
                    
                    _exploreState = ExploreStates.EnemyMoving;
                }
            break;

            case ExploreStates.EnemyMoving:
                var allEnemiesMadeAMoveThisTurn = _activeRoom.enemies.haveAllEnemiesMadeAMoveThisTurn();
                var allEnemiesDoneMoving = _activeRoom.enemies.haveAllEnemiesStoppedMoving();
                if(allEnemiesMadeAMoveThisTurn && allEnemiesDoneMoving) {
                    checkForCombat();
                    setNewRound();

                    _exploreState = ExploreStates.PlayerMoving;
                }
            break;
        }
    }

    void checkForCombat() {

    }

    void setNewRound() {
        _round++;

        _activePlayer.setNewRound();
        _activeRoom.setNewRound();

        txtRoundNum.text = _round.ToString();
    }


    //PUBLIC METHODS
}
