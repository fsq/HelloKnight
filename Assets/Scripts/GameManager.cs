using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Mono global instance.
    public static GameManager Instance;

    private GameObject _player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);

            _player = GameObject.FindObjectOfType<NewPlayer>().gameObject;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetPrefab(string name)
    {
        var prefab = (GameObject)Resources.Load("Prefabs/" + name, typeof(GameObject));
        if (prefab == null)
        {
            Debug.LogError("Failed to fetch prefab: " + name);
        }
        return prefab;
    }

    public GameObject GetPlayerGameObj()
    {
        if (_player == null)
        {
            Debug.LogError("No player instance found!");
        }
        return _player;
    }
}
