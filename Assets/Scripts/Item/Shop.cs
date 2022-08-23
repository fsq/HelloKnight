using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private GameObject _tutorialText;
    void Start()
    {
        _tutorialText = transform.Find("Tutorial").gameObject;
    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.kTagPlayer))
        {
            _tutorialText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        {
            if (other.CompareTag(Constants.kTagPlayer))
            {
                _tutorialText.SetActive(false);
            }
        }
    }
}
