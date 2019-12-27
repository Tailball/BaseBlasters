using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameExploreState : MonoBehaviour
{
    //UNITY LINKS
    [SerializeField] Camera CameraOrthographic = null;
    [SerializeField] Camera CameraPerspective = null;
    [SerializeField] TMP_Text txtRoundNum = null;


    //MEMBERS (PRIVATE)
    bool _playerStartedCombat = false;
    ExploreStates _state;
    int _round = 0;

    EnemyController _activeEnemy;


    //ACCESSORS - MUTATORS (PUBLIC)
    public ExploreStates state {
        get { return _state; }
    }

    public Camera cameraPerspective {
        get { return CameraPerspective; }
    }


    //UNITY LIFECYCLE
    void Awake() {
        _state = ExploreStates.PlayerMoving;
    }


    //PRIVATE METHODS
    void getExploringInput() {
        if(GameController.instance.gameState != GameStates.Ingame) return;

        if(Input.GetKeyDown(KeyCode.C)) {
            switchCamera();
        }
    }

    void switchCamera() {
        CameraOrthographic.gameObject.SetActive(!CameraOrthographic.gameObject.activeInHierarchy);
        CameraPerspective.gameObject.SetActive(!CameraPerspective.gameObject.activeInHierarchy);
    }

    void execExplore() {
        var gc = GameController.instance;

        var activePlayer = gc.activePlayer;
        var enemies = gc.activeRoom.enemies;

        switch(_state) {
            case ExploreStates.MoveFromCombat: 
                //first frame after combat, we want to clean up stuff before they make a move
                execMoveFromCombat(activePlayer, enemies);
            break;

            case ExploreStates.PlayerMoving:
                execPlayerMoving(activePlayer, enemies);
            break;

            case ExploreStates.EnemyMoving:
                execEnemyMoving(activePlayer, enemies);
            break;

            case ExploreStates.MoveToCombat:
                execMoveToCombat(activePlayer, enemies);
            break;

            case ExploreStates.MoveToCombatCoroutine:
                //
            break;
        }
    }

    void execMoveFromCombat(PlayerController activePlayer, EnemiesController enemies) {
        if(_playerStartedCombat) {
            setNewExploreRound();
            _state = ExploreStates.PlayerMoving;
        }
        else {
            enemies.setAllEnemiesExploreAction(activePlayer.transform.position);
            _state = ExploreStates.EnemyMoving;
        }
    }

    void execPlayerMoving(PlayerController activePlayer, EnemiesController enemies) {
        var playerMover = activePlayer.movementData;

        if(playerMover.hasMadeAMoveThisTurn && !playerMover.isMoving) {
            var combat = checkForCombat();

            if(!combat) {
                enemies.setAllEnemiesExploreAction(activePlayer.transform.position);
                _state = ExploreStates.EnemyMoving;
            }
            else {
                _playerStartedCombat = true;
            }
        }
    }

    void execEnemyMoving(PlayerController activePlayer, EnemiesController enemies) {
        var allEnemiesMadeAMoveThisTurn = enemies.haveAllEnemiesMadeAMoveThisTurn();
        var allEnemiesDoneMoving = enemies.haveAllEnemiesStoppedMoving();
        
        if(allEnemiesMadeAMoveThisTurn && allEnemiesDoneMoving) {
            setNewExploreRound();
            var combat = checkForCombat();

            if(!combat) {
                _state = ExploreStates.PlayerMoving;
            }
            else {
                _playerStartedCombat = false;
            }
        }
    }

    void execMoveToCombat(PlayerController activePlayer, EnemiesController enemies) {
        StartCoroutine(execMoveToCombatCoroutine(activePlayer, enemies));
        _state = ExploreStates.MoveToCombatCoroutine;
    }

    IEnumerator execMoveToCombatCoroutine(PlayerController activePlayer, EnemiesController enemies) {
        yield return new WaitForSeconds(.5f);

        _activeEnemy.setAlert();

        yield return new WaitForSeconds(.75f);

        activePlayer.movementData.jumpBack(false);
        _activeEnemy.movementData.jumpBack(false);

        yield return new WaitForSeconds(1f);

        // If a player started combat and wins, he gets the perk of starting the next round. Same with enemies.
        // Because we need to clean up and set new directions for the enemies at the end of combat (not right now)
        // we need to have an intermediary state. 
        // Since we then directly move to combat states, it will only be triggered when we go back into exploring state
        // Movement can happen during combat (jumpback()) and therefore, we need to wait before giving an enemy a new direction
        // until the end of combat (or the first frame of exploring -> the movefromcombat state)
        _state = ExploreStates.MoveFromCombat;
        
        GameController.instance.changePlaystateToCombat(_activeEnemy);
    }

    bool checkForCombat() {
        var gc = GameController.instance;

        var colliders = Physics.OverlapSphere(gc.activePlayer.transform.position, .2f, gc.npcLayer);
        
        if(colliders.Length > 0) {
            _activeEnemy = colliders[0].GetComponent<EnemyController>();
            _state = ExploreStates.MoveToCombat;

            return true;
        }

        return false;
    }


    //PUBLIC METHODS
    public void exec() {
        getExploringInput();
        execExplore();
    }

    public void setExploringCameraTarget(GameObject target) {
        CameraOrthographic.GetComponent<CameraController>().setTarget(target);
        CameraPerspective.GetComponent<CameraController>().setTarget(target);
    }

    public void setNewExploreRound() {
        var gc = GameController.instance;

        _round++;

        gc.activePlayer.setNewRound();
        gc.activeRoom.setNewRound();

        txtRoundNum.text = _round.ToString();
    }
}