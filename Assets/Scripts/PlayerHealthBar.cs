using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private NewPlayer _player;
    [SerializeField] private float _lengthPerHealth = 2;
    [SerializeField] private Slider _slider;

    private RectTransform _trans;

    void Start()
    {
        _trans = GetComponent<RectTransform>();
        _slider = GetComponent<Slider>();
    }

    void Update()
    {
        // Resize per max health
        var size = _trans.sizeDelta;
        size[0] = _lengthPerHealth * _player.MaxHealth;
        _trans.sizeDelta = size;
        _slider.maxValue = _player.MaxHealth;
        _slider.value = _player.Health;
    }
}
