using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attacks : MonoBehaviour
{
    // Callback from attacker, trigger when hit happens
    public delegate void AttackerDelegate(GameObject victim);
    public AttackerDelegate Delegate;

    abstract public GameObject Attacker { get; set; }

    abstract public float Damage { get; set; }

    // How long does attacker anime last.
    abstract public float ActionDuration { get; }

    // How long does attack last (can Hit objects).
    abstract public float LifeSpan { get; }

    // Maybe do something to the attacker. Healing, Charging, etc.
    // TODO: Refactor GameObject -> interface, hitable?
    virtual public void Hit(GameObject victim)
    {
        var monster = victim.gameObject.GetComponent<Monsters>();
        monster.UnderAttack(this);
        Delegate?.Invoke(victim);
    }

    // Callback for victim when attack is done.
    virtual public void hitDone()
    {
        Destroy(gameObject);
    }

    // Preferred version to instance.Destruct(). Attacks have internal timer,
    // and can already become null when caller trying to destruct them.
    static public void Destruct(Attacks attack)
    {
        if (attack != null)
        {
            attack.Destruct();
        }
    }

    // Explicitly destruct this attack.
    virtual public void Destruct()
    {
        Destroy(gameObject);
    }

    protected void Start()
    {
        StartCoroutine(DestructTimer());
    }

    virtual public IEnumerator DestructTimer()
    {
        yield return new WaitForSeconds(LifeSpan);
        Destruct();
    }
}