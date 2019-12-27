using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameCombatState : MonoBehaviour
{
    //UNITY LINKS
    [SerializeField] GameObject HandArea;


    //MEMBERS (PRIVATE)
    CombatStates _state;
    EnemyController _activeEnemy;
    bool handEnabled = false;


    //ACCESSORS - MUTATORS (PUBLIC)
    public CombatStates state {
        get { return _state; }
    }


    //UNITY LIFECYCLE
    void Awake() {
        _state = CombatStates.DrawingCards;
    }

    void Start() {
        return;
        HandArea.transform.position = new Vector3(HandArea.transform.position.x, 0f, HandArea.transform.position.z);
    }

    void Update() {
        return;
        var handLocation = new Vector3(HandArea.transform.position.x, handEnabled ? 50f : 0f, HandArea.transform.position.z);

        if(Mathf.Abs(HandArea.transform.position.y - handLocation.y) > Mathf.Epsilon) {
            HandArea.transform.position = Vector3.MoveTowards(HandArea.transform.position, handLocation, Time.deltaTime * 265f);
        }
    }


    //PRIVATE METHODS
    void execCombat() {
        switch(_state) {
            case CombatStates.DrawingCards:
                DeckController.instance.drawCardsToHand(3);
                _state = CombatStates.EnemyTurn;
            break;

            case CombatStates.NewRound:
                execNewRound();
            break;

            case CombatStates.EnemyTurn:
                execEnemyTurn();
            break;

            case CombatStates.EnemyTurnCoroutine:
            break;

            case CombatStates.PlayerTurn:
                //Wait for cards (func playerMadeCardMove())
                //Will then automatically go to resolve
            break;

            case CombatStates.Resolve:
                execResolve();
            break;

            case CombatStates.ResolveCoroutine:
            break;

            case CombatStates.EndCombat:
                execEndCombat();
            break;        

            case CombatStates.EndCombatCoroutine:
            break;
        }
    }

    void execNewRound() {
        var dInst = DeckController.instance;
        dInst.moveAllDropPointsToDiscard();

        if(dInst.amountOfCardsInDeck <= 0 && dInst.amountOfCardsInHand <= 0) {
            GameController.instance.changePlaystateToGameOver();
        }
        else {
            dInst.drawCardsToHand(1);
            _state = CombatStates.EnemyTurn;
        }
    }

    void execEnemyTurn() {
        StartCoroutine(runEnemyTurn());
        _state = CombatStates.EnemyTurnCoroutine;
    }

    IEnumerator runEnemyTurn() {
        yield return new WaitForSeconds(.5f);

        _activeEnemy.playCard();
        handEnabled = true;

        yield return new WaitForSeconds(.65f);
        
        _state = CombatStates.PlayerTurn;
    }

    void execResolve() {
        StartCoroutine(runResolveCombat());
        _state = CombatStates.ResolveCoroutine;
    }

    IEnumerator runResolveCombat() {
        handEnabled = false;
        yield return new WaitForSeconds(.65f);

        var gc = GameController.instance;
        var activePlayer = gc.activePlayer;
        var dInst = DeckController.instance;
        dInst.playerDrop.use();
        dInst.enemyDrop.use();

        //TODO: refine // animate
        _activeEnemy.damage(1);

        yield return new WaitForSeconds(.5f);
        
        if(activePlayer.healthPoints <= 0) {
            Destroy(activePlayer);
            gc.changePlaystateToGameOver();
        }
        else if(_activeEnemy.healthPoints <= 0) {
            _state = CombatStates.EndCombat;
        }
        else {
            _state = CombatStates.NewRound;
        }
    }

    void execEndCombat() {
        StartCoroutine(runEndCombat());
        _state = CombatStates.EndCombatCoroutine;
    }

    IEnumerator runEndCombat() {
        yield return new WaitForSeconds(.5f);

        var dInst = DeckController.instance;
        dInst.moveAllDropPointsToDiscard();
        dInst.moveHandToDeck();

        var gc = GameController.instance;
        var activePlayer = gc.activePlayer;
        activePlayer.movementData.jumpBack(true);
        gc.activeRoom.enemies.killEnemy(_activeEnemy);

        yield return new WaitForSeconds(.3f);

        _state = CombatStates.DrawingCards;
        GameController.instance.changePlaystateToExploring();
    }


    //PUBLIC METHODS
    public void exec() {
        execCombat();
    }

    public void setActiveEnemy(EnemyController activeEnemy) {
        _activeEnemy = activeEnemy;
    }

    public void acceptPlayerMove() {
        _state = CombatStates.Resolve;
    }
}