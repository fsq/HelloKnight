using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHealthBar : MonoBehaviour
{
    [SerializeField] private float _shakeTime = 0.16f;
    [SerializeField] private float _shakeGranularity = 0.04f;
    [SerializeField] private float _magnitude = 0.12f;
    private Monsters _monster;
    private float _initScale, _prevPercentage;
    private float _shakeLeft;
    private Vector3 _initPos;

    // Start is called before the first frame update
    void Start()
    {
        _monster = GetComponentInParent<Monsters>();
        _initScale = transform.localScale.x;
        _prevPercentage = GetPercentage();
        _initPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        var scale = transform.localScale;
        var currentPercentage = GetPercentage();
        scale.x = _initScale * currentPercentage;
        transform.localScale = scale;

        // Shake if decrease
        if (_prevPercentage > currentPercentage)
        {
            StartCoroutine(Shaker(_shakeTime));
        }
        _prevPercentage = currentPercentage;
    }

    IEnumerator Shaker(float shakeTime)
    {
        while (shakeTime > 0)
        {
            yield return StartCoroutine(Shake(_shakeGranularity));

            shakeTime -= _shakeGranularity;
        }
        transform.localPosition = _initPos;
    }

    IEnumerator Shake(float time)
    {
        float x = _initPos.x + Random.Range(-1f, 1f) * _magnitude;
        float y = _initPos.y + Random.Range(-1f, 1f) * _magnitude;
        transform.localPosition = new Vector3(x, y, _initPos.z);
        yield return new WaitForSeconds(time);
    }

    private float GetPercentage()
    {
        return _monster.Health / _monster.MaxHealth;
    }
}
