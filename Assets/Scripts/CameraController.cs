using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    // Y of the top of center camera box
    private float CenterBoxTopHeight = 3f;
    [SerializeField]
    private float CenterBoxButtomHeight = -1f;
    [SerializeField]
    private float CenterBoxButtomMinHeight = 0f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(Constants.kTagPlayer).transform;
    }

    void LateUpdate()
    {
        if (player == null)
        {
            return;
        }
        var pos = this.transform.position;
        pos.x = player.position.x;
        if (player.position.y > pos.y + CenterBoxTopHeight)
        {
            pos.y += player.position.y - (pos.y + CenterBoxTopHeight);
        }
        else if (player.position.y < pos.y + CenterBoxButtomHeight)
        {
            pos.y += player.position.y - (pos.y + CenterBoxButtomHeight);
            pos.y = Mathf.Max(CenterBoxButtomMinHeight, pos.y);
        }
        this.transform.position = pos;
    }
}
