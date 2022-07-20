using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attacks : MonoBehaviour
{
    abstract public GameObject Attacker { get; set; }

    abstract public float Damage { get; set; }

    // How long does attack anime last.
    abstract public float LastingTime { get; }

    // Maybe do something to the attacker. Healing, Charging, etc.
    // TODO: Refactor GameObject -> interface, hitable?
    abstract public void Hit(GameObject victim);

    // Callback by victim when attack is done.
    virtual public void hitDone()
    {
        Destroy(gameObject);
    }

    // Explicitly destruct this attack.
    virtual public void Destruct()
    {
        Destroy(gameObject);
    }
}