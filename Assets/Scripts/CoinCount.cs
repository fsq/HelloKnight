using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinCount : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private FSMPlayer _player;

    void Start()
    {
        _player = GameManager.Instance.GetPlayerGameObj().GetComponent<FSMPlayer>();
    }

    void Update()
    {
        _text.text = _player.Coin.ToString();
    }
}
