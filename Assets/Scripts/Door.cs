using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IHitable
{

    [SerializeField] private Monsters _monsterTrigger;
    private Animator _anim;

    private void OnEnable()
    {
        if (_monsterTrigger != null)
        {
            EventManager.onMonsterDie += CheckMonsterDeath;
        }
    }

    private void OnDisable()
    {
        if (_monsterTrigger != null)
        {
            EventManager.onMonsterDie -= CheckMonsterDeath;
        }
    }

    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    void Update() { }

    // Act like an obstacle for now.
    public float UnderAttack(Attacks attack)
    {
        Attacks.Destruct(attack);
        return 0;
    }

    void CheckMonsterDeath(Monsters monster)
    {
        if (monster == _monsterTrigger)
        {
            Open();
        }
    }

    private void Open()
    {
        _anim.SetBool("Open", true);
    }
}
