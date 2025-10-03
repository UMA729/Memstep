using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [SerializeField] float R = 0;
    [SerializeField] float G = 0;
    [SerializeField] float B = 0;

    float min = 50;
    float max = 255;

    GameObject[] ColorBlock = new GameObject[3];

    private void Start()
    {
        for (int i = 0; i < ColorBlock.Length; i++)
        {

            R = Random.Range(min, max);
            G = Random.Range(min, max);
            B = Random.Range(min, max);
            RangeChange();
            ColorBlock[i].GetComponent<Renderer>(); 
        }
    }

    private void Update()
    {

    }

    void RangeChange()
    {
        min += Random.Range(20, 100);
        max -= Random.Range(0, 50);
    }
}
