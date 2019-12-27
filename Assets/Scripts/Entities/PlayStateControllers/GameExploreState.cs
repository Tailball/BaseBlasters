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

    void execPlayerMoving(PlayerController activePlayer, EnemiesController enemies) {
        var playerMover = activePlayer.movementData;

        if(playerMover.hasMadeAMoveThisTurn && !playerMover.isMoving) {
            var combat = checkForCombat();

            if(!combat) {
                enemies.setAllEnemiesExploreAction(activePlayer.transform.position);
                _state = ExploreStates.EnemyMoving;
            }
            else {
                setNewExploreRound();
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
                enemies.setAllEnemiesExploreAction(activePlayer.transform.position);
                _playerStartedCombat = false;
            }
        }
    }

    void execMoveToCombat(PlayerController activePlayer, EnemiesController enemies) {
        StartCoroutine(execMoveToCombatCoroutine(activePlayer, enemies));
        _state = ExploreStates.MoveToCombatCoroutine;
    }

    IEnumerator execMoveToCombatCoroutine(PlayerController activePlayer, EnemiesController enemies) {
        yield return new WaitForSeconds(2f);

        //If a player started combat and wins, he gets the perk of starting the next round. Same with enemies.
        if(_playerStartedCombat)
            _state = ExploreStates.PlayerMoving;
        else
            _state = ExploreStates.EnemyMoving;

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