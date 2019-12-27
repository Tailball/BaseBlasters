using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterMover))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    //UNITY LINKS
    [Header("Links")]
    [SerializeField] List<CardController> EnemyCards = new List<CardController>();
    [SerializeField] int HealthPoints;

    
    //MEMBERS (PRIVATE)
    Animator _anim = null;
    CharacterMover _mover = null;
    float _timeToIdle = 10f;


    //ACCESSORS - MUTATORS (PUBLIC)
    public CharacterMover movementData {
        get { return _mover; }
    }

    public int healthPoints {
        get { return HealthPoints; }
    }


    //UNITY LIFECYCLE
   void Awake() {
       _anim = GetComponentInChildren<Animator>();
       _mover = GetComponent<CharacterMover>();
   }

   void Start() {
   }

   void Update() {
        if(!_mover.isMoving) {
            _timeToIdle -= Time.deltaTime * Random.Range(.1f, 7.5f);
            if(_timeToIdle <= 0) {
                _anim.SetTrigger("Idle");
                _timeToIdle = 12.5f;
            }
        }
    }


    //PRIVATE METHODS


    //PUBLIC METHODS
    public void setExploreAction(Vector3 playerPos) {
        //TODO AI:
        //Check if player is in same room
        //Check if player is in sight

        Vector3 moveTowards = Vector3.zero;
        if((transform.position.x - playerPos.x) > 0) {
            moveTowards = Vector3.left;
        }
        else if((transform.position.x - playerPos.x) < 0) {
            moveTowards = Vector3.right;
        }
        else if((transform.position.z - playerPos.z) > 0) {
            moveTowards = Vector3.back;
        }
        else if((transform.position.z - playerPos.z) < 0) {
            moveTowards = Vector3.forward;
        }

        if(_mover.isValidLocation(transform.position + moveTowards))
            _mover.setMovementDirection(moveTowards);
        else {
            //TODO:
            //If we don't make it hop, it will block the game, because it will never set 'hasmovedthisturn'.
            //Perhaps better AI here?
            _mover.setMovementDirection(Vector3.zero);
        }
    }

    public void playCard() {
        //TODO: AI
        //Add card AI here
        var randomCard = Random.Range(0, EnemyCards.Count);
        DeckController.instance.setEnemyCardOnDroppoint(EnemyCards[randomCard]);
    }

    public void setNewRound() {
        _mover.setNewRound();
    }

    public void damage(int dmg) {
        HealthPoints -= dmg;
    }
}