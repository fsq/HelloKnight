using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Item : MonoBehaviour
{
    // Keep single pointer across all Item classes.
    static private GameObject _playerObj;
    static private FSMPlayer _player;

    virtual protected void Start()
    {
        _playerObj ??= GameManager.Instance.GetPlayerGameObj();
        _player ??= _playerObj.GetComponent<FSMPlayer>();
    }

    protected void HealPlayer(ResourceGauge amount)
    {
        _player.Heal(amount);
    }

    protected void PickupCoin(int amount)
    {
        _player.PickupCoin(amount);
    }

    virtual protected void Destruct()
    {
        Destroy(gameObject);
    }
}
