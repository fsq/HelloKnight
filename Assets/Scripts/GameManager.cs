using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Mono global instance.
    public static GameManager Instance;

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

    public GameObject GetPrefab(string name)
    {
        var prefab = (GameObject)Resources.Load("Prefabs/" + name, typeof(GameObject));
        if (prefab == null)
        {
            Debug.LogError("Failed to fetch prefab: " + name);
        }
        return prefab;
    }
}
