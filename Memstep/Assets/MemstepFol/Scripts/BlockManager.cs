using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [Header("ブロック設定")]
    public GameObject blockPrefab;
    public int rows = 3;
    public int columns = 3;
    public float spacing = 2f;

    [Header("色設定")]
    public Color[] colorCandidates = new Color[6]
    {
        Color.red,
        Color.blue,
        Color.yellow,
        Color.green,
        new Color(1f, 0f, 1f),   // 紫
        new Color(1f, 0.5f, 0f)  // オレンジ
    };

    private List<GameObject> blocks = new List<GameObject>();
    private Dictionary<GameObject, Color> originalColors = new Dictionary<GameObject, Color>();

    private Color correctColor;
    private Color[] currentRoundColors; // 今回使う3色
    public Color CorrectColor => correctColor;

    void Start()
    {
        CreateBlocks();
        Choose3Colors();
        SetRowColors();
        ChooseCorrectColor();
    }

    void CreateBlocks()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 pos = new Vector3(x * spacing - spacing, (y * spacing) - (spacing), 0);
                GameObject block = Instantiate(blockPrefab, pos, Quaternion.identity, transform);
                blocks.Add(block);
            }
        }
    }

    void Choose3Colors()
    {
        currentRoundColors = colorCandidates.OrderBy(c => Random.value).Take(3).ToArray();
    }

    void SetRowColors()
    {
        for (int y = 0; y < rows; y++)
        {
            Color[] shuffledColors = currentRoundColors.OrderBy(c => Random.value).ToArray();

            for (int x = 0; x < columns; x++)
            {
                int index = y * columns + x;
                SpriteRenderer renderer = blocks[index].GetComponent<SpriteRenderer>();
                renderer.color = shuffledColors[x];

                // 元の色を保存
                originalColors[blocks[index]] = shuffledColors[x];
            }
        }
    }

    void ChooseCorrectColor()
    {
        correctColor = currentRoundColors[Random.Range(0, currentRoundColors.Length)];
    }

    public bool CheckBlock(GameObject block)
    {
        // 現在の色ではなく、保存された元の色で判定！
        if (originalColors.TryGetValue(block, out Color originalColor))
        {
            return originalColor == correctColor;
        }
        return false;
    }

    public GameObject GetBlockAt(int row, int column)
    {
        return blocks[row * columns + column];
    }

    public List<GameObject> GetAllBlocks() => blocks;

}
