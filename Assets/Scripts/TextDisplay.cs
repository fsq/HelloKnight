using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextDisplay : MonoBehaviour
{
    [SerializeField] private float _damage;
    // 1. Text pops up and enlarges quickly
    [SerializeField] private float _popDuration = 0.15f;
    [SerializeField] private float _popSpeed = 6f;
    [SerializeField] private float _fontEnlargeSpeed = 15f;
    // 2. Text shrinks quickly
    [SerializeField] private float _shrinkDuration = 0.15f;
    [SerializeField] private float _shrinkSpeed = 6f;
    [SerializeField] private float _fontShrinkSpeed = 15f;
    // 3. Text steadily floats up and disappears
    [SerializeField] private float _floatDuration = 0.6f;
    [SerializeField] private float _floatSpeed = 4f;
    [SerializeField] private float _fontDisappearSpeed = 0.8f;
    private TextMeshPro _tmp;

    public static void DisplayDamage(Transform transform, float damage)
    {
        Display(transform, damage, Constants.kPrefabDamageText);
    }

    public static void DisplayHealthRecovery(Transform transform, float damage)
    {
        Display(transform, damage, Constants.kPrefabHealthRecoveryText);
    }

    private static void Display(Transform transform, float damage, string prefabName)
    {
        var prefab = GameManager.Instance.GetPrefab(prefabName);
        var obj = Instantiate(prefab);
        obj.transform.position = transform.position;
        var text = obj.GetComponent<TextDisplay>();
        text.Init(damage);
    }

    public void Init(float damage)
    {
        _damage = damage;
    }

    private void Start()
    {
        _tmp = GetComponent<TextMeshPro>();
        _tmp.text = _damage.ToString();
    }

    private void Update()
    {
        if (_popDuration > 0)
        {
            _tmp.transform.position += Vector3.up * _popSpeed * Time.deltaTime;
            _tmp.fontSize += _fontEnlargeSpeed * Time.deltaTime;
            _popDuration -= Time.deltaTime;
        }
        else if (_shrinkDuration > 0)
        {
            _tmp.transform.position += Vector3.up * _shrinkSpeed * Time.deltaTime;
            _tmp.fontSize -= _fontShrinkSpeed * Time.deltaTime;
            _shrinkDuration -= Time.deltaTime;
        }
        else if (_floatDuration > 0)
        {
            _tmp.transform.position += Vector3.up * _floatSpeed * Time.deltaTime;
            // Shrink slowly
            _tmp.fontSize -= _fontShrinkSpeed / 6 * Time.deltaTime;
            var color = _tmp.color;
            color.a -= _fontDisappearSpeed * Time.deltaTime;
            _tmp.color = color;
            _floatDuration -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
