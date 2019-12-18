using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMover))]
public class EnemyController : MonoBehaviour
{
    //UNITY LINKS

    
    //MEMBERS (PRIVATE)
    Animator _anim = null;
    CharacterMover _mover = null;
    float _timeToIdle = 10f;


    //ACCESSORS - MUTATORS (PUBLIC)
    public CharacterMover movementData {
        get { return _mover; }
    }


    //UNITY LIFECYCLE
   void Awake() {
       _anim = GetComponentInChildren<Animator>();
       _mover = GetComponent<CharacterMover>();
   }

   void Start() {
   }

   void Update() {
        if(!_mover.IsMoving) {
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
}
