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
    [SerializeField] GameObject ExploreUI = null;
    [SerializeField] GameObject CombatUI = null;

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
    [SerializeField] LayerMask DefNPCLayer;


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
        ExploreUI.SetActive(false);
        CombatUI.SetActive(false);

        this._gameState = GameStates.ContentGeneration; 
        this._playState = PlayStates.Initializing;
        this._exploreState = ExploreStates.PlayerMoving;
        this._combatState = CombatStates.DrawingCards;
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
                execEnter();
            break;

            case PlayStates.Exploring:
                getExploringInput();
                execExplore();
            break;

            case PlayStates.Combat:
                execCombat();
            break;

            case PlayStates.Exiting:
            break;
        }
    }

    void execInitialize() {
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
        DeckController.instance.setStartSizeAndFillPool(6);
    }

    void execEnter() {
        CameraPerspective.nearClipPlane -= Time.deltaTime * EnterSpeed;

        if(CameraPerspective.nearClipPlane <= 3.8f) {
            CameraPerspective.nearClipPlane = 3.8f;
            _playState = PlayStates.Exploring;
            
            setNewRound();
            ExploreUI.SetActive(true);
            CombatUI.SetActive(false);
        }
    }

    void execExplore() {
        switch(_exploreState) {
            case ExploreStates.PlayerMoving:
                var playerMover = _activePlayer.movementData;

                if(playerMover.hasMadeAMoveThisTurn && !playerMover.isMoving) {
                    var combat = checkForCombat();

                    if(!combat) {
                        _activeRoom.enemies.setAllEnemiesExploreAction(_activePlayer.transform.position);
                        _exploreState = ExploreStates.EnemyMoving;
                    }
                    else {
                        setNewRound();
                        _exploreState = ExploreStates.PlayerMoving;
                        _playState = PlayStates.Combat;
                    }
                }
            break;

            case ExploreStates.EnemyMoving:
                var allEnemiesMadeAMoveThisTurn = _activeRoom.enemies.haveAllEnemiesMadeAMoveThisTurn();
                var allEnemiesDoneMoving = _activeRoom.enemies.haveAllEnemiesStoppedMoving();

                if(allEnemiesMadeAMoveThisTurn && allEnemiesDoneMoving) {
                    setNewRound();
                    var combat = checkForCombat();

                    if(!combat) {
                        _exploreState = ExploreStates.PlayerMoving;
                    }
                    else {
                        _activeRoom.enemies.setAllEnemiesExploreAction(_activePlayer.transform.position);
                        _exploreState = ExploreStates.EnemyMoving;
                        _playState = PlayStates.Combat;
                    }
                }
            break;
        }
    }

    void execCombat() {
        switch(_combatState) {
            case CombatStates.DrawingCards:
                DeckController.instance.drawCardsToHand(2);
                _combatState = CombatStates.EnemyTurn;
            break;

            case CombatStates.EnemyTurn:
                //Put any active cards in discard pile
                //Select card and play
                _combatState = CombatStates.PlayerTurn;
            break;

            case CombatStates.PlayerTurn:
                //Wait for cards (func playerMadeCardMove())
                //Will automatically go to resolve
            break;

            case CombatStates.Resolve:
                //Apply both cards
                //Define if anyone died
                //Go either to EndCombat or EnemyTurn state
                _combatState = CombatStates.EndCombat;
            break;

            case CombatStates.EndCombat:
                DeckController.instance.moveDropPointToDiscard();
                DeckController.instance.moveHandToDeck();

                var deck = DeckController.instance.amountOfCardsInDeck;
                var discard = DeckController.instance.amountOfCardsInDiscardPile;
                var hand = DeckController.instance.amountOfCardsInHand;
                Debug.Log($"After combat. deck: {deck}, hand: {hand}, discard: {discard}");

                //Apply xp, rewards, animations...
                _combatState = CombatStates.DrawingCards;
                _playState = PlayStates.Exploring;
            break;        
        }
    }

    bool checkForCombat() {
        var colliders = Physics.OverlapSphere(_activePlayer.transform.position, .2f, DefNPCLayer);
        
        if(colliders.Length > 0) {
            var enemy = colliders[0];
            _playState = PlayStates.Combat;

            ExploreUI.SetActive(false);
            CombatUI.SetActive(true);

            return true;
        }

        return false;
    }

    void setNewRound() {
        _round++;

        _activePlayer.setNewRound();
        _activeRoom.setNewRound();

        txtRoundNum.text = _round.ToString();
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


    //PUBLIC METHODS
    public void playerMadeCardMove() {
        //TODO: 
        //1. define current monster we're fighting
        //2. set monsters card before player move
        //3. define if cards have been placed (this function = actual trigger)
        //4. move to resolve state

        _combatState = CombatStates.Resolve;
    }
}