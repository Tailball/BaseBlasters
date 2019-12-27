using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameCombatState : MonoBehaviour
{
    //UNITY LINKS
    CombatStates _state;


    //MEMBERS (PRIVATE)
    EnemyController _activeEnemy;


    //ACCESSORS - MUTATORS (PUBLIC)
    public CombatStates state {
        get { return _state; }
    }


    //UNITY LIFECYCLE
    void Awake() {
        _state = CombatStates.DrawingCards;
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

    void execResolve() {
        StartCoroutine(runResolveCombat());
        _state = CombatStates.ResolveCoroutine;
    }

    void execEndCombat() {
        StartCoroutine(runEndCombat());
        _state = CombatStates.EndCombatCoroutine;
    }

    IEnumerator runEnemyTurn() {
        yield return new WaitForSeconds(.5f);

        _activeEnemy.playCard();
        _state = CombatStates.PlayerTurn;
    }

    IEnumerator runResolveCombat() {
        yield return new WaitForSeconds(.5f);

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
            gc.activeRoom.enemies.killEnemy(_activeEnemy);
            _state = CombatStates.EndCombat;
        }
        else {
            _state = CombatStates.NewRound;
        }
    }

    IEnumerator runEndCombat() {
        yield return new WaitForSeconds(.5f);

        var dInst = DeckController.instance;
        dInst.moveAllDropPointsToDiscard();
        dInst.moveHandToDeck();

        yield return new WaitForSeconds(.5f);

        _state = CombatStates.DrawingCards;
        GameController.instance.changePlaystateToExploring();

        //Apply xp, rewards, animations...
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