using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [Header("ブロック設定")]
    public GameObject blockPrefab;
    public int rows = 3;      // 横方向（行）
    public int columns = 3;   // 縦方向（列）
    public float spacing = 2f;
    public float shuffleSpeed = 3f; // ←追加：シャッフル時の移動スピード

    [Header("色設定")]
    public Color[] color = new Color[6]
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
    private Color[] currentRoundColors;

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
        for (int y = 0; y < rows; y++) // 縦方向
        {
            for (int x = 0; x < columns; x++) // 横方向
            {
                Vector3 pos = new Vector3(x * spacing - spacing, (y * spacing) - (spacing+1), 0);
                GameObject block = Instantiate(blockPrefab, pos, Quaternion.identity, transform);
                blocks.Add(block);
            }
        }
    }

    //シャッフル
    public void ShuffleRow(int rowIndex)
    {
        StartCoroutine(ShuffleRowAnimation(rowIndex));
    }

    public IEnumerator ShuffleRowAnimation(int rowIndex)
    {
        Debug.Log($"🌀 シャッフル開始 行={rowIndex}");

        List<GameObject> rowBlocks = new List<GameObject>();
        int startIndex = rowIndex * columns;
        for (int i = 0; i < columns; i++)
            rowBlocks.Add(blocks[startIndex + i]);

        // 現在の色を取得
        List<Color> colors = new List<Color>();
        foreach (var block in rowBlocks)
            colors.Add(block.GetComponent<SpriteRenderer>().color);

        // シャッフル
        var shuffled = colors.OrderBy(c => Random.value).ToList();

        // 見た目にわかるように少し間を置いて変更
        for (int i = 0; i < columns; i++)
        {
            rowBlocks[i].GetComponent<SpriteRenderer>().color = shuffled[i];
            yield return new WaitForSeconds(0.2f); // ←アニメっぽく見える
        }

        Debug.Log($"✅ シャッフル完了 行={rowIndex}");
    }


    void Choose3Colors()
    {
        currentRoundColors = color.OrderBy(c => Random.value).Take(3).ToArray();
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
                originalColors[blocks[index]] = shuffledColors[x];
            }
        }
    }

    void ChooseCorrectColor()
    {
        correctColor = currentRoundColors[Random.Range(0, currentRoundColors.Length)];
    }

    public void ResetBlocks(int newRows)
    {
        // 既存ブロック削除
        foreach (var block in blocks)
            if (block != null)
                Destroy(block);

        StopAllCoroutines();


        blocks.Clear();
        originalColors.Clear();

        // 行数を更新して再構築
        rows = newRows;
        CreateBlocks();
        Choose3Colors();
        SetRowColors();
        ChooseCorrectColor();
    }

    public void SaveOriginalColors()
    {
        originalColors.Clear();
        foreach (var block in blocks)
        {
            var sr = block.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                originalColors[block] = sr.color;
            }
        }
    }

    public void RestoreOriginalColors()
    {
        foreach (var kvp in originalColors)
        {
            if (kvp.Key != null)
            {
                kvp.Key.GetComponent<SpriteRenderer>().color = kvp.Value;
            }
        }
    }

    public bool CheckBlock(GameObject block)
    {
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
