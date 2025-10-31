using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject Player;
    public Vector3 Ori_pos;

    // Start is called before the first frame update
    void Start()
    {
        Ori_pos = new Vector3(0, -4, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializePos()
    {
        Player.transform.position = Ori_pos;
        var sr = Player.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.white;
        }
    }
}
