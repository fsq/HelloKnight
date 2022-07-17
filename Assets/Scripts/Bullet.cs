using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _damage;
    public float Damage { get => _damage; set => _damage = value; }
    [SerializeField] private float _lifeSpan = 3;

    private void Start()
    {
        StartCoroutine(DestroyTimer(_lifeSpan));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.kTagMonsters))
        {
            var monster = other.gameObject.GetComponent<Monsters>();
            monster.TakeDamage(Damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag(Constants.kTagGround))
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyTimer(float lifeSpan)
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(gameObject);
    }
}
