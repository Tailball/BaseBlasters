using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterMover))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    //UNITY LINKS
    [Header("Links")]
    [SerializeField] List<CardController> EnemyCards;
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
                _timeToIdle = 20f;
            }
        }
    }

   void FixedUpdate() {
   }


    //PRIVATE METHODS


    //PUBLIC METHODS
    public void setNewRound() {
        _mover.setNewRound();
    }

    public void setExploreAction(Vector3 playerPos) {
        //TODO:
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

        _mover.setMovementDirection(moveTowards);
    }

    public void playCard() {
        //TODO: Add card AI here

        var randomCard = Random.Range(0, EnemyCards.Count);
        DeckController.instance.setEnemyCardOnDroppoint(EnemyCards[randomCard]);
    }

    public void damage(int dmg) {
        HealthPoints -= dmg;
    }
}