using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergyBar : MonoBehaviour
{

    [SerializeField] private float _lengthPerEnergy = 2;
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
        var size = _trans.sizeDelta;
        size[0] = _lengthPerEnergy * _player.resource.MaxEnergy;
        _trans.sizeDelta = size;
        _slider.maxValue = _player.resource.MaxEnergy;
        _slider.value = _player.resource.Energy;
    }
}
