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

    private bool _isDragging = false;
    private Vector3 _positionBeforeDragging;
    private bool _isInDropzone = false;

    private Guid _id;
    

    //ACCESSORS - MUTATORS (PUBLIC)
    public Guid Id {
        get { return _id; }
    }


    //UNITY LIFECYCLE
    void Awake() {
    }

    void Start() {
    }

    void Update() {
        if(_isDragging && !_isInDropzone) {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if(eventData.button == PointerEventData.InputButton.Left)
            DeckController.instance.moveCardToDropPoint(this);
        else 
            Discard();
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

    public void Discard() {
        DeckController.instance.moveCardToDiscard(this);
    }

    public void SetId(Guid id) {
        _id = id;
    }
}