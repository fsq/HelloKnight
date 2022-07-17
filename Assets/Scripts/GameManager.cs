using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Mono global instance.
    public static GameManager mono;

    private void Awake()
    {
        if (mono == null)
        {
            mono = this;
            DontDestroyOnLoad(mono);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
