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

    [SerializeField] Transform DeckPile = null;
    [SerializeField] Transform DiscardPile = null;
    [SerializeField] RectTransform HandPile = null;
    [SerializeField] Transform TempPile = null;

    [SerializeField] public RectTransform PlayerDropPoint = null;
    [SerializeField] public RectTransform EnemyDropPoint = null;
    

    //MEMBERS (PRIVATE)


    //ACCESSORS - MUTATORS (PUBLIC)
    public List<CardController> deck {
        get { 
            return DeckPile.GetComponentsInChildren<CardController>().ToList();
        }
    }

    public List<CardController> discard {
        get { 
            return DiscardPile.GetComponentsInChildren<CardController>().ToList();
        }
    }

    public List<CardController> hand {
        get {
            return HandPile.GetComponentsInChildren<CardController>().ToList();
        }
    }

    public CardController playerDrop {
        get {
            return PlayerDropPoint.GetComponentInChildren<CardController>();
        }
    }

    public CardController enemyDrop {
        get {
            return EnemyDropPoint.GetComponentInChildren<CardController>();
        }
    }

    public int amountOfCardsInDeck {
        get { return deck.Count(); }
    }

    public int amountOfCardsInDiscardPile {
        get { return discard.Count(); }
    }

    public int amountOfCardsInHand {
        get { return hand.Count(); }
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


    //PRIVATE METHODS


    //PUBLIC METHODS
    public void shuffle() {
        var cardsInDeck = deck;
        cardsInDeck.ForEach(c => c.transform.SetParent(TempPile));

        var cardsInDeckShuffled = cardsInDeck.OrderBy(c => Guid.NewGuid()).ToList();
        cardsInDeckShuffled.ForEach(c => c.transform.SetParent(DeckPile));
    }

    public void setStartSizeAndFillPool(int amount) {
        for(var i = 0; i < amount; i++) {
            var c = PoolController.instance.getCard().GetComponent<CardController>();

            var instance = Instantiate(c, Vector3.zero, Quaternion.identity, DeckPile);
            instance.SetId(Guid.NewGuid());
        }

        shuffle();
    }

    public void drawCardsToHand(int amount) {
        for(var i = 0; i < amount; i++) {
            if(amountOfCardsInDeck > 0) {
                var card = this.deck.First();
                card.transform.SetParent(HandPile);
            }
        }
    }

    public void addCardToDeck(CardController card) {
        card.transform.SetParent(DeckPile);
        shuffle();
    }

    public void addCardsToDeck(List<CardController> cards) {
        cards.ForEach(c => {
            c.transform.SetParent(DeckPile);
        });

        shuffle();
    }

    public void moveCardsFromDeckToDiscardPile(int amount) {
        for(var i = 0; i < amount; i++) {
            if(amountOfCardsInDeck > 0) {
                var card = this.deck.First();
                card.transform.SetParent(DiscardPile);
            }
        }
    }

    public void moveHandToDiscardPile() {
        hand.ForEach(c => {
            c.transform.SetParent(DiscardPile);
        });
    }

    public void moveHandToDeck() {
        hand.ForEach(c => {
            c.transform.SetParent(DeckPile);
        });

        shuffle();
    }

    public void moveDiscardPileToDeck() {
        discard.ForEach(c => {
            c.transform.SetParent(DeckPile);
        });

        shuffle();
    }

    public void moveCardToDropPoint(CardController card) {
        card.transform.SetParent(PlayerDropPoint, false);
        card.transform.position = PlayerDropPoint.transform.position;

        GameController.instance.playerMadeCardMove();
    }

    public void moveCardToDiscard(CardController card) {
        card.transform.SetParent(DiscardPile);
    }

    public void moveDropPointToDiscard() {
        playerDrop.transform.SetParent(DiscardPile);
    }
}