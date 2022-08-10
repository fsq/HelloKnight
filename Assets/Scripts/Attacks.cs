using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attacks : MonoBehaviour
{
    // Callback from attacker, trigger when hit happens
    public delegate void AttackerDelegate(GameObject victim);
    public AttackerDelegate Delegate;

    [SerializeField] protected GameObject _attacker;
    public GameObject Attacker { get => _attacker; set => _attacker = value; }

    [SerializeField] protected float _damage;
    public float Damage { get => _damage; set => _damage = value; }

    // How long does attacker anime last.
    [SerializeField] private float _actionDuration;
    public float ActionDuration { get => _actionDuration; set => _actionDuration = value; }

    // How long does attack last (can Hit objects).
    [SerializeField] protected float _lifeSpan = 3;
    public float LifeSpan { get => _lifeSpan; set => _lifeSpan = value; }

    // Maybe do something to the attacker. Healing, Charging, etc.
    virtual public void Hit(GameObject victim)
    {
        var hitable = victim.gameObject.GetComponent<IHitable>();
        if (hitable == null)
        {
            Debug.LogError("There is no hitable interface from victim: " + victim.name);
            return;
        }
        float dealt = hitable.UnderAttack(this);
        Delegate?.Invoke(victim);

        // Don't show floating text if damage is zero.
        if (dealt > 0)
        {
            TextDisplay.DisplayDamage(victim.transform, dealt);
        }
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