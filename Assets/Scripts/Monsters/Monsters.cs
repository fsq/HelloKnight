using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monsters : MonoBehaviour, IHitable
{
    abstract public float Health { get; set; }

    abstract public float MaxHealth { get; set; }

    abstract public float Damage { get; set; }

    [SerializeField] public int CoinDrop;

    virtual public float UnderAttack(Attacks attack)
    {
        _incomingAttack = attack;
        Health -= attack.Damage;
        StartCoroutine(FlashWhite(_flashDuration));
        return attack.Damage;
    }

    virtual protected void Die()
    {
        EventManager.onMonsterDie(this);
        Destroy(gameObject);
    }

    protected Rigidbody2D _rb;
    protected SpriteRenderer _renderer;

    // Record if is hit between last and current frame.
    protected Attacks _incomingAttack;
    [SerializeField] protected float _backoffDistance = 1f;
    [SerializeField] protected float _backoffSpeed = 12f;
    protected bool _backoffing;

    // Target for attacking, moving, etc.
    [SerializeField] protected GameObject _target;

    [SerializeField] protected float _flashDuration = 0.1f;
    [SerializeField] protected Material _flashMaterial;

    virtual public void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    virtual public void Start()
    {
        if (_target == null)
        {
            _target = GameObject.FindGameObjectWithTag(Constants.kTagPlayer);
        }
        if (_flashMaterial == null)
        {
            _flashMaterial = GameManager.Instance.GetMaterial(Constants.kMatFlashWhite);
        }
    }

    virtual public void Update()
    {
        GetComponent<SpriteRenderer>().flipX = _target.transform.position.x < transform.position.x;
        HandleDamage();
    }

    virtual protected void HandleDamage()
    {
        if (_incomingAttack == null) return;

        _incomingAttack.hitDone();
        if (Health <= 0) Die();

        if (!_backoffing)
        {
            var source = _incomingAttack.transform.position;
            var me = transform.position;
            var direction = Vector3.Normalize(me - source);
            StartCoroutine(Backoff(direction, _backoffDistance, _backoffSpeed));
        }

        _incomingAttack = null;
    }

    virtual protected IEnumerator Backoff(Vector3 direction, float distance, float speed)
    {
        _backoffing = true;

        while (distance > 0)
        {
            var currentDist = speed * Time.deltaTime;
            transform.position += direction * currentDist;
            distance -= currentDist;
            yield return null;
        }
        // _rb.velocity = Vector2.zero;
        _backoffing = false;
    }

    virtual protected IEnumerator FlashWhite(float duration)
    {
        var original = _renderer.material;
        _renderer.material = _flashMaterial;
        yield return new WaitForSeconds(duration);
        _renderer.material = original;
    }

}