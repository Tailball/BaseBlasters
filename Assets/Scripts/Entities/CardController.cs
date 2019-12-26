using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class CardController : MonoBehaviour, 
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    //UNITY LINKS
    [Header("Links")]
    [SerializeField] CardTypes CardType;
    

    //MEMBERS (PRIVATE)
    private Canvas _canvas;

    private Guid _id;
    private bool _isInDropzone = false;
    

    //ACCESSORS - MUTATORS (PUBLIC)
    public Guid Id {
        get { return _id; }
    }

    public bool IsInDropZone {
        get { return _isInDropzone; }
    }


    //UNITY LIFECYCLE
    void Awake() {
    }

    void Start() {
    }

    void Update() {
    }

    public void OnPointerClick(PointerEventData eventData) {
        if(CardType == CardTypes.Enemy) return;
        if(_isInDropzone) return;
        if(GameController.instance.playState != PlayStates.Combat) return;
        if(GameController.instance.passedCombatState != CombatStates.PlayerTurn) return;

        if(eventData.button == PointerEventData.InputButton.Left) {
            DeckController.instance.moveCardToDropPoint(this);
            _isInDropzone = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    public void OnPointerExit(PointerEventData eventData) {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }


    //PRIVATE METHODS


    //PUBLIC METHODS
    public void Use() {
        //call static card engine to execute current behaviour?
    }

    public void SetId(Guid id) {
        _id = id;
    }
}