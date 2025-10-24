using UnityEngine;
using System.Collections;

public class ColorObstacleManager : MonoBehaviour
{
    public GameObject obstaclePrefab;
    GameController GC;
    public float spawnInterval = 1.5f;
    public float speed = 2f;

     Color[] colors = new Color[6]
    {
        Color.red,
        Color.blue,
        Color.yellow,
        Color.green,
        new Color(1f, 0f, 1f),   // Ž‡
        new Color(1f, 0.5f, 0f)  // ƒIƒŒƒ“ƒW
    };

    void Start()
    {
        GC = FindObjectOfType<GameController>();
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
        Vector3 spawnPos = new Vector3(-10f, Random.Range(-2f, 2f), 0);
        GameObject obj = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);

        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = colors[Random.Range(0, colors.Length)];
        }

        obj.AddComponent<Rigidbody2D>().gravityScale = 0;
        obj.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);

        Destroy(obj, 8f);
    }
}
