using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckController : MonoBehaviour
{
    //SINGLETON PATTERN
    public static DeckController instance;


    //UNITY LINKS
    [Header("Links")]
    [SerializeField] TMP_Text TxtDeckSize;
    

    //MEMBERS (PRIVATE)
    private List<CardController> _deck;


    //ACCESSORS - MUTATORS (PUBLIC)
    public List<CardController> deck {
        get { return _deck; }
    }

    public int amountOfCards {
        get { return _deck.Count(); }
    }


    //UNITY LIFECYCLE
    void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this) {
            Destroy(gameObject);
        }
    }

    void Start() {
        _deck = new List<CardController>();
    }

    void Update() {
        
    }


    //PRIVATE METHODS


    //PUBLIC METHODS
    public void setStartSizeAndFillPool(int amount) {
        for(var i = 0; i < amount; i++) {
            _deck.Add(PoolController.instance.getCard().GetComponent<CardController>());
        }

        shuffle();
    }

    public void shuffle() {
        _deck = _deck.OrderBy(d => new Guid()).ToList();
    }

    public List<CardController> drawCards(int amount) {
        var drawn = new List<CardController>();

        for(var i = 0; i < amount; i++) {
            if(_deck.Count > 0) {
                drawn.Add(_deck[0]);
                _deck.RemoveAt(0);
            }
        }

        return drawn;
    }

    public void addCards(List<CardController> cards) {
        _deck.AddRange(cards);
        shuffle();
    }

    public void discardCards(int amount) {
        for(var i = 0; i < amount; i++) {
            if(_deck.Count > 0)
                _deck.RemoveAt(0);
        }
    }
}
