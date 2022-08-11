using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Item
{
    [SerializeField] public int Amount;

    static public GameObject Create(Vector3 position, int amount)
    {
        var obj = Instantiate(GameManager.Instance.GetPrefab(Constants.kPrefabItemCoin));
        var coin = obj.GetComponent<Coin>();
        coin.transform.position = position;
        coin.Amount = amount;
        return obj;
    }

    protected override void Start()
    {
        base.Start();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag(Constants.kTagPlayer))
        {
            PickupCoin(Amount);
            Destruct();
        }
    }
}
