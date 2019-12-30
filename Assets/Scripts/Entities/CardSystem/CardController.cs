using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


public class CardController : MonoBehaviour, 
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    //UNITY LINKS
    [Header("Links")]
    [SerializeField] string IdentifyingName = String.Empty;
    [SerializeField] CardTypes CardType = CardTypes.Combat;
    [SerializeField] TMP_Text Description = null;


    //MEMBERS (PRIVATE)
    private Canvas _canvas;
    private List<ICardEffect> _effects = new List<ICardEffect>();

    private Guid _id;
    private bool _isInDropzone = false;
    
    private Vector3 _growDestination = Vector3.one;
    private float _growSpeed = 3f;
    

    //ACCESSORS - MUTATORS (PUBLIC)
    public Guid Id {
        get { return _id; }
    }

    public bool IsInDropZone {
        get { return _isInDropzone; }
    }


    //UNITY LIFECYCLE
    void Awake() {
        getCardEffects();
    }

    void Start() {
        setDescription();
    }

    void Update() {
        if(!shouldRespondToEvents()) return;

        if((Mathf.Abs(transform.localScale.x - _growDestination.x) > Mathf.Epsilon) ||
           (Mathf.Abs(transform.localScale.y - _growDestination.y) > Mathf.Epsilon) ||
           (Mathf.Abs(transform.localScale.z - _growDestination.z) > Mathf.Epsilon)) {
            transform.localScale = Vector3.MoveTowards(transform.localScale, _growDestination, Time.deltaTime * _growSpeed);
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if(!shouldRespondToEvents()) return;

        if(eventData.button == PointerEventData.InputButton.Left) {
            DeckController.instance.moveCardToDropPoint(this);
            _isInDropzone = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        _growDestination = new Vector3(1.12f, 1.12f, 1.12f);
    }

    public void OnPointerExit(PointerEventData eventData) {
        _growDestination = Vector3.one;
    }


    //PRIVATE METHODS
    void getCardEffects() {
        var cardfx = GetComponents<ICardEffect>();
        
        for(var i = 0; i < cardfx.Length; i++) {
            _effects.Add(cardfx[i]);
        }
    }

    void setDescription() {
        if(_effects.Count <= 0) return;
        
        var sb = string.Empty;
        
        _effects.OrderBy(fx => fx.executionOrder)
                .ToList()
                .ForEach(fx => sb += fx.statusText + "\n");

        Description.text = sb;
    }

    private bool shouldRespondToEvents() {
        if(CardType == CardTypes.Enemy) return false;
        if(_isInDropzone) return false;
        if(GameController.instance.playState != PlayStates.Combat) return false;
        if(GameController.instance.passedCombatState != CombatStates.PlayerTurn) return false;

        return true;
    }


    //PUBLIC METHODS
    public void setId(Guid id) {
        _id = id;
    }

    public void setIsInDropzone(bool isInDropZone) {
        _isInDropzone = isInDropZone;
    }

    public void reset() {
        _isInDropzone = false;
        _growDestination = Vector3.one;
        transform.localScale = Vector3.one;
    }

    public void use(PlayerController player, EnemyController enemy) {
        _effects.OrderBy(fx => fx.executionOrder)
                .ToList()
                .ForEach(fx => fx.execEffect(player, enemy, CardType));
    }
}