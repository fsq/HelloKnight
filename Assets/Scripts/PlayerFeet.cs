using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeet : MonoBehaviour
{
    [SerializeField]
    private int numGround = 0;

    public bool IsInAir()
    {
        return numGround == 0;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag(Constants.kTagGround))
        {
            ++numGround;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag(Constants.kTagGround))
        {
            --numGround;
        }
    }
}
