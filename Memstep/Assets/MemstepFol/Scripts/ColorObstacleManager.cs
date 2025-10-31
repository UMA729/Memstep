using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorObstacleManager : MonoBehaviour
{
    public List<GameObject> spawnedObstacles = new List<GameObject>();

    public GameObject obstaclePrefab;

    BlockManager BM;
    GameController GController;
    public float spawnInterval = 1.5f;
    public float speed = 2f;

    void Start()
    {
        BM = FindAnyObjectByType<BlockManager>();
    }

    public IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnObstacle();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnObstacle()
    {
        // 🎯 ブロックの中からランダムに1つ選ぶ
        List<GameObject> allBlocks = BM.GetAllBlocks();
        GameObject targetBlock = allBlocks[Random.Range(0, allBlocks.Count)];

        // ブロック位置を基準にスポーン（少し前面に出すなら z = -1 など）
        Vector3 spawnPos = new Vector3(-5, targetBlock.transform.position.y, 0);

        GameObject obj = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);

        spawnedObstacles.Add(obj);

        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = BM.color[Random.Range(0, BM.color.Length)];
        }

        obj.AddComponent<Rigidbody2D>().gravityScale = 0;
        obj.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);

        if (obj != null)
            Destroy(obj, 8f);

    }

    public void DestroyObj()
    {
        foreach (GameObject obj in spawnedObstacles)
        {
            if (obj != null)
                Destroy(obj);
        }
        spawnedObstacles.Clear();
    }
}