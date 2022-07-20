using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : Attacks
{
    [SerializeField] private float _damage;
    public override float Damage { get => _damage; set => _damage = value; }

    [SerializeField] private GameObject _attacker;
    public override GameObject Attacker { get => _attacker; set => _attacker = value; }

    [SerializeField] private float _lifeSpan = 0.2f;
    public override float LifeSpan => _lifeSpan;

    [SerializeField] private float _lastingTime = 0.2f;
    public override float LastingTime => _lastingTime;

    // Can't Hit when false.
    // OnTriggerEnter2D still triggers for all entered objects even if we 
    // disable collider after the first Hit().
    private bool _isActive = true;

    static public GameObject Create(GameObject attacker, AttackerDelegate del,
                                    Vector3 direction, float damage)
    {
        var obj = Instantiate(GameManager.Instance.GetPrefab(Constants.kPrefabBlade),
                                attacker.transform);
        var blade = obj.GetComponent<Blade>();
        blade.Attacker = attacker;
        blade.Damage = damage;
        blade.Delegate = del;

        // Blade orientation
        var scale = obj.transform.localScale;
        scale.x *= Mathf.Sign(direction.x); // Flip collider and renderer
        obj.transform.localScale = scale;

        return obj;
    }

    public override void Hit(GameObject victim)
    {
        base.Hit(victim);
        _isActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isActive) return;
        if (other.CompareTag(Constants.kTagMonsters))
        {
            Hit(other.gameObject);
        }
    }

    public override void hitDone()
    {
        // no-op
    }
}
