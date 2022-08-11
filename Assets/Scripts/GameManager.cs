using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Mono global instance.
    public static GameManager Instance;

    private GameObject _player;
    private GameObject _canvas;
    private Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();
    private Dictionary<string, Material> _materials = new Dictionary<string, Material>();

    private void OnEnable()
    {
        EventManager.onMonsterDie += OnMonsterDie;
    }

    private void OnDisable()
    {
        EventManager.onMonsterDie -= OnMonsterDie;
    }

    private void OnMonsterDie(Monsters monster)
    {
        if (monster == null)
        {
            Debug.LogError("'monster' parameter cannot be null.");
            return;
        }
        // TODO: Move to a Spawner class.
        SpawnCoin(monster.transform.position, monster.CoinDrop);
    }

    private void SpawnCoin(Vector3 position, int amount)
    {
        Coin.Create(position, amount);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Material GetMaterial(string name)
    {
        Material material;
        if (!_materials.TryGetValue(name, out material))
        {
            material = (Material)Resources.Load("Materials/" + name, typeof(Material));
            if (material == null)
            {
                Debug.LogError("Failed to fetch material: " + name);
                return null;
            }
            _materials.Add(name, material);
        }
        return material;
    }

    public GameObject GetPrefab(string name)
    {
        GameObject prefab;
        if (!_prefabs.TryGetValue(name, out prefab))
        {
            prefab = (GameObject)Resources.Load("Prefabs/" + name, typeof(GameObject));
            if (prefab == null)
            {
                Debug.LogError("Failed to fetch prefab: " + name);
                return null;
            }
            _prefabs.Add(name, prefab);
        }
        return prefab;
    }

    public GameObject GetPlayerGameObj()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag(Constants.kTagPlayer).gameObject;
        }
        return _player;
    }

    public GameObject GetCanvas()
    {
        if (_canvas == null)
        {
            _canvas = GameObject.FindObjectOfType<Canvas>().gameObject;
        }
        return _canvas;
    }
}
