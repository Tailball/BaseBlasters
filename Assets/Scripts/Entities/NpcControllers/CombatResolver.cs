using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CombatResolver : MonoBehaviour
{
    //UNITY LINKS
    [Header("Links")]
    [SerializeField] Sprite uiportrait;

    [Header("Tweaks")]
    [SerializeField] int maxHp = 0;


    //MEMBERS (PRIVATE)
    private int _hp;
    private int _activeDefense;
    private int _activeDamage;
    //TODO: extra trackers for poison, curses, longterm shield, invincibility etc


    //ACCESSORS - MUTATORS (PUBLIC)
    public int health {
        get { return _hp; }
    }

    public float healthPercentage {
        get { return (float)_hp / (float)maxHp; }
    }
    
    public string healthText {
        get { return $"{_hp}/{maxHp}"; }
    }

    public Sprite portrait {
        get { return uiportrait; }
    }


    //UNITY LIFECYCLE
    void Start() {
        _hp = maxHp;
    }
    

    //PRIVATE METHODS
    private void reset() {
        _activeDefense = 0;
        _activeDamage = 0;
    }


    //PUBLIC METHODS
    public void addDefense(int def) {
        _activeDefense += def;
    }

    public void addDamage(int dmg) {
        _activeDamage += dmg;
    }

    public int resolveAndReportHP(string npc) {
        var damageAfterDefense = _activeDamage - _activeDefense;
        var nullifiedDamage = Mathf.Max(damageAfterDefense, 0);

        _hp -= nullifiedDamage;

        reset();
        return _hp;
    }
}