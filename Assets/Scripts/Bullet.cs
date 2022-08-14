using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Attacks
{
    private string _targetTag;

    static public ResourceGauge GetCostByAttackerTag(string tag)
    {
        var cost = ResourceGauge.EmptyGauge();
        if (tag == Constants.kTagPlayer)
        {
            cost.Energy = 15;
        }
        return cost;
    }

    // Use Player as target tag.
    static public GameObject CreateForMonster(GameObject attacker, AttackerDelegate del,
                                    Vector3 direction, float damage)
    {
        var bullet = Create(attacker, del, direction, damage,
                        Constants.kDefaultBulletSpeed, Constants.kTagPlayer, Constants.kPrefabMonsterBullet);
        bullet.tag = Constants.kTagMonsters;
        bullet.layer = LayerMask.NameToLayer(Constants.kLayerMonsters);
        return bullet;
    }

    // Use Monsters as target tag.
    static public GameObject Create(GameObject attacker, AttackerDelegate del,
                                        Vector3 direction, float damage, float speed)
    {
        return Create(attacker, del, direction, damage,
                        speed, Constants.kTagMonsters, Constants.kPrefabBullet);
    }

    static public GameObject Create(GameObject attacker, AttackerDelegate del,
                                    Vector3 direction, float damage, float speed,
                                    string targetTag, string prefabName)
    {
        var obj = Instantiate(GameManager.Instance.GetPrefab(prefabName));
        if (obj == null)
        {
            Debug.LogError("No prefab found: " + prefabName);
            return null;
        }
        var bullet = obj.GetComponent<Bullet>();
        if (bullet == null)
        {
            Debug.LogError("No Blade component found in prefab: " + prefabName);
            return null;
        }
        bullet.Attacker = attacker;
        bullet.Damage = damage;
        bullet.Delegate = del;
        bullet._targetTag = targetTag;

        // Bullet start pos
        var pos = obj.transform.position;
        pos.x *= Mathf.Sign(direction.x);
        obj.transform.position = attacker.transform.position + pos;

        // Bullet velocity
        var v = obj.GetComponent<Rigidbody2D>().velocity;
        v = direction.normalized * speed;
        obj.GetComponent<Rigidbody2D>().velocity = v;

        // Bullet orientation
        var scale = obj.transform.localScale;
        scale.x *= Mathf.Sign(direction.x); // Flip collider and renderer
        obj.transform.localScale = scale;

        return obj;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(_targetTag) ||
            other.CompareTag(Constants.kTagDoor)) // Or other destructible environment.
        {
            Hit(other.gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer(Constants.kLayerObstacle))
        {
            Destruct();
        }
    }

    public override void hitDone()
    {
        // no-op, penetrate.
    }
}
