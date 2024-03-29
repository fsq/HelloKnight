using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPotion : Item
{
    // How much resource it can heal
    [SerializeField] private ResourceGauge _amount;

    static public HealPotion Create(ResourceGauge amount)
    {
        var potion = new HealPotion();
        potion._amount = amount;
        return potion;
    }

    protected override void Start()
    {
        base.Start();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag(Constants.kTagPlayer))
        {
            HealPlayer(_amount);
            Destruct();
        }
    }
}
