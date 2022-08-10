using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Item : MonoBehaviour
{
    private GameObject _playerObj;
    private FSMPlayer _player;

    protected Item()
    {
        _playerObj = GameManager.Instance.GetPlayerGameObj();
        _player = _playerObj.GetComponent<FSMPlayer>();
    }

    protected void HealPlayer(ResourceGauge amount)
    {
        _player.Heal(amount);
    }

    virtual protected void Destruct()
    {
        Destroy(gameObject);
    }
}
