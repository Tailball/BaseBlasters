﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CombatResolver))]
[RequireComponent(typeof(CharacterMover))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    //UNITY LINKS
    [Header("Links")]
    [SerializeField] List<CardController> EnemyCards = new List<CardController>();
    [SerializeField] Canvas InternalCanvas = null;

    
    //MEMBERS (PRIVATE)
    Animator _anim = null;
    CharacterMover _mover = null;
    CombatResolver _resolver = null;

    float _timeToIdle = 10f;


    //ACCESSORS - MUTATORS (PUBLIC)
    public CharacterMover movementData {
        get { return _mover; }
    }

    public CombatResolver combatData {
        get { return _resolver; }
    }


    //UNITY LIFECYCLE
   void Awake() {
       _anim = GetComponentInChildren<Animator>();
       _mover = GetComponent<CharacterMover>();
       _resolver = GetComponent<CombatResolver>();
   }

   void Update() {
        if(!_mover.isMoving) {
            _timeToIdle -= Time.deltaTime * Random.Range(.1f, 7.5f);
            if(_timeToIdle <= 0) {
                _anim.SetTrigger("Idle");
                _timeToIdle = 12.5f;
            }
        }

        if(InternalCanvas.isActiveAndEnabled) {
            InternalCanvas.transform.LookAt(Camera.main.transform);
        }
    }


    //PRIVATE METHODS
    IEnumerator setMovementCoroutine(Vector3 direction) {
        yield return new WaitForSeconds(.35f);
        _mover.setMovementDirection(direction);
    }


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

        if(!_mover.isValidLocation(transform.position + moveTowards)) {
            //TODO:
            //If we don't make it hop, it will block the game, because it will never set 'hasmovedthisturn'.
            //Perhaps better AI here?
            moveTowards = Vector3.zero;
        }

        StartCoroutine(setMovementCoroutine(moveTowards));
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

    public void setAlert() {
        InternalCanvas.gameObject.SetActive(true);
        InternalCanvas.worldCamera = Camera.main;
    }
}