using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Item
{
    [SerializeField] public int Amount;

    // A map from coin denominations -> prefab scale, 
    // so that coins with different denominations has different sizes.
    // Can also map to different prefabs later.
    private static readonly List<List<float>> _coinSizes;

    // Static constructor to initialize static members once.
    static Coin()
    {
        _coinSizes = new List<List<float>>();
        _coinSizes.Add(new List<float>() { 100, 1f });
        _coinSizes.Add(new List<float>() { 50, 0.85f });
        _coinSizes.Add(new List<float>() { 20, 0.75f });
        _coinSizes.Add(new List<float>() { 10, 0.65f });
        _coinSizes.Add(new List<float>() { 5, 0.55f });
        _coinSizes.Add(new List<float>() { 1, 0.45f });
    }

    static public List<GameObject> Create(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogError("Invalid coin amount: " + amount);
            return null;
        }
        List<GameObject> coins = new List<GameObject>();
        for (int i = 0; amount > 0;)
            if (amount < _coinSizes[i][0])
            {
                ++i;
            }
            else
            {
                var obj = Instantiate(GameManager.Instance.GetPrefab(Constants.kPrefabItemCoin));
                var scale = new Vector3(_coinSizes[i][1], _coinSizes[i][1], 1);
                obj.transform.localScale = scale;
                obj.GetComponent<Coin>().Amount = (int)_coinSizes[i][0];

                coins.Add(obj);
                amount -= (int)_coinSizes[i][0];
            }
        return coins;
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
