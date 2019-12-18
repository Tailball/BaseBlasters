using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterBaseController : MonoBehaviour
{
    //UNITY LINKS
    [Header("Tweaking")]
    [SerializeField] protected float MovementSpeed = 1f;


    //MEMBERS (PRIVATE)
    protected Rigidbody _rb = null;
    protected Animator _anim = null;
    
    protected Vector3 _destination;
    protected float _rotDestinationY;
    protected bool _madeAMoveThisTurn;
    protected bool _isMoving;


    //ACCESSORS - MUTATORS (PUBLIC)
    public Vector3 Position {
        get { return new Vector3(transform.position.x, 0, transform.position.z); }
    }

    public bool HasMadeAMoveThisTurn {
        get { return _madeAMoveThisTurn; }
    }

    public bool IsMoving {
        get { return _isMoving; }
    }


    //UNITY LIFECYCLE
    protected virtual void Awake() {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();
    }

    protected virtual void Start() {
        _destination = transform.position;
        _madeAMoveThisTurn = false;
        _isMoving = false;
    }

    protected virtual void Update() {
    }

    protected virtual void FixedUpdate() {
        move();
        rotate();
    }


    //PRIVATE METHODS
    protected void move() {
        if(!_isMoving) return;

        var intermediatePosition = Vector3.MoveTowards(transform.position, _destination, Time.fixedDeltaTime * this.MovementSpeed);
        _rb.MovePosition(intermediatePosition);

        var diffX = Mathf.Abs(transform.position.x - _destination.x);
        var diffY = Mathf.Abs(transform.position.z - _destination.z);
        if(diffX <= 0.01f && diffY <= 0.01f) {
            _rb.MovePosition(_destination);
            _isMoving = false;
        }
    }

    protected void rotate() {
        if(!_isMoving) return;

        _rb.MoveRotation(Quaternion.Euler(0, _rotDestinationY, 0));
    }


    //PUBLIC METHODS
    public void setDestination(Vector3 action) {
        var currRotationAngle = transform.rotation.eulerAngles.y;

        if(action.x != 0) {
            _destination = new Vector3(transform.position.x + action.x, transform.position.y, transform.position.z);
            _rotDestinationY = action.x > 0 ? 90 : 270; 
        }
        else if(action.z != 0) {
            _destination = new Vector3(transform.position.x, transform.position.y, transform.position.z + action.z);
            _rotDestinationY = action.z > 0 ? 0 : 180;
        }

        _anim.SetTrigger("Move");
        _madeAMoveThisTurn = true;
        _isMoving = true;
    }

    public void setNewRound() {
        _madeAMoveThisTurn = false;
        _isMoving = false;
    }
}