using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongBladeCharm : Item
{
    [SerializeField] private BuffManager.BuffType type = BuffManager.BuffType.LongBlade;

    protected override void Start()
    {
        base.Start();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag(Constants.kTagPlayer))
        {
            AppendCharm(type);
            Destruct();
        }
    }
}
