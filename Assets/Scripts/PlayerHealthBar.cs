using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private float _lengthPerHealth = 2;
    [SerializeField] private Slider _slider;

    private FSMPlayer _player;

    private RectTransform _trans;

    void Start()
    {
        _trans = GetComponent<RectTransform>();
        _slider = GetComponent<Slider>();
        _player = GameManager.Instance.GetPlayerGameObj().GetComponent<FSMPlayer>();
    }

    void Update()
    {
        if (_player == null) return;

        // Resize per max health
        var size = _trans.sizeDelta;
        size[0] = _lengthPerHealth * _player.MaxHealth;
        _trans.sizeDelta = size;
        _slider.maxValue = _player.MaxHealth;
        _slider.value = _player.Health;
    }
}
