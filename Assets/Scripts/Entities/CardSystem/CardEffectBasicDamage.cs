using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardEffectBasicDamage : MonoBehaviour,
    ICardEffect
{
    //UNITY LINKS
    [Header("Tweaking")]
    [SerializeField] int damage = 1;


    //MEMBERS (PRIVATE)


    //ACCESSORS - MUTATORS (PUBLIC)
    public int executionOrder { 
        get { return 1; } 
    }

    public string statusText { 
        get { return $"Damage {damage}"; }
    }


    //UNITY LIFECYCLE


    //PRIVATE METHODS


    //PUBLIC METHODS
    public void execEffect(PlayerController player, EnemyController enemy, CardTypes cardtype) {
        if(cardtype == CardTypes.Combat) {
            enemy.combatData.addDamage(damage);
        }

        else if(cardtype == CardTypes.Enemy) {
            player.combatData.addDamage(damage);
        }
    }
}