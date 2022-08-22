using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Item : MonoBehaviour
{
    // Keep single pointer across all Item classes.
    static private GameObject _playerObj;
    static private FSMPlayer _player;

    [SerializeField] public Sprite Icon;
    [SerializeField] public string Name;
    [SerializeField] public string Description;
    [SerializeField] public int Price;

    virtual protected void Start()
    {
        var player = GameManager.Instance.GetPlayerGameObj();
        // TODO: Weird bug if replace with "??=": https://answers.unity.com/questions/1915597/-bug-in-start-when-updating-static-variable-after-2.html
        if (_playerObj == null)
        {
            _playerObj = player;
        }
        if (_player == null)
        {
            _player = _playerObj.GetComponent<FSMPlayer>();
        }
    }

    protected void HealPlayer(ResourceGauge amount)
    {
        _player.Heal(amount);
    }

    protected void PickupCoin(int amount)
    {
        _player.PickupCoin(amount);
    }

    protected void AppendCharm(BuffManager.BuffType type)
    {
        _player.ApplyBuff(type);
    }

    virtual protected void Destruct()
    {
        Destroy(gameObject);
    }
}
