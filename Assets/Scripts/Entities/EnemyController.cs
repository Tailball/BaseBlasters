using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : CharacterBaseController
{
    //UNITY LINKS

    
    //MEMBERS (PRIVATE)
    float _timeToIdle = 10f;


    //ACCESSORS - MUTATORS (PUBLIC)


    //UNITY LIFECYCLE
    protected override void Awake() {
        base.Awake();
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();

        if(!_isMoving) {
            _timeToIdle -= Time.deltaTime * Random.Range(.1f, 7.5f);
            if(_timeToIdle <= 0) {
                _anim.SetTrigger("Idle");
                _timeToIdle = 20f;
            }
        }
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
    }


    //PRIVATE METHODS


    //PUBLIC METHODS
}
