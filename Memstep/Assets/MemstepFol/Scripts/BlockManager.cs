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
    public float Xspacing = 2f;
    public float Yspacing = 1.5f;
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

    //ブロック移動処理
    IEnumerator MoveInArc(GameObject block, Vector3 start, Vector3 end, float height, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // イージング（滑らかに）
            float smoothT = Mathf.SmoothStep(0, 1, t);

            // 放物線の高さを追加
            float yOffset = Mathf.Sin(smoothT * Mathf.PI) * height;

            // XとZは線形補間、Yは放物線
            Vector3 pos = Vector3.Lerp(start, end, smoothT);
            pos.y += yOffset;

            block.transform.position = pos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        block.transform.position = end;
    }

    //ブロック生成
    void CreateBlocks()
    {
        for (int y = 0; y < rows; y++) // 縦方向
        {
            for (int x = 0; x < columns; x++) // 横方向
            {
                Vector3 pos = new Vector3(x * Xspacing - Xspacing, (y * Yspacing) - (Yspacing+1), 0);
                GameObject block = Instantiate(blockPrefab, pos, Quaternion.identity, transform);
                blocks.Add(block);
            }
        }
    }

    //シャッフル開始
    public void ShuffleRow()
    {
        StartCoroutine(ShuffleRowAnimation());
    }
    //シャッフルするブロックの処理
    public IEnumerator ShuffleRowAnimation()
    {
        int rowIndex = Random.Range(0, columns);

        int startIndex = rowIndex * columns;

        // 現在行のブロックを取得
        List<GameObject> rowBlocks = new List<GameObject>();
        for (int i = 0; i < columns; i++)
            rowBlocks.Add(blocks[startIndex + i]);

        // 現在の位置リストを保持
        List<Vector3> originalPositions = rowBlocks.Select(b => b.transform.position).ToList();

        // 並びをランダムに入れ替える
        List<GameObject> shuffled = rowBlocks.OrderBy(b => Random.value).ToList();

        float duration = 0.8f;
        float height = 1.5f;

        // ✅ 各ブロックのアニメーションをまとめて管理
        List<Coroutine> moveCoroutines = new List<Coroutine>();

        for (int i = 0; i < columns; i++)
        {
            GameObject block = shuffled[i];
            Vector3 startPos = block.transform.position;
            Vector3 endPos = originalPositions[i];
            Coroutine move = StartCoroutine(MoveInArc(block, startPos, endPos, height, duration));
            moveCoroutines.Add(move);
            yield return new WaitForSeconds(0.1f); // 少しずつずらして動かす
        }

        //全アニメーションが終わるまで待つ
        yield return new WaitForSeconds(duration + 0.5f);

        //全て動き終わってからリスト更新！
        for (int i = 0; i < columns; i++)
        {
            blocks[startIndex + i] = shuffled[i];
        }
    }

    //3色選択
    void Choose3Colors()
    {
        currentRoundColors = color.OrderBy(c => Random.value).Take(3).ToArray();
    }

    //色設定
    void SetRowColors()
    {
        for (int y = 0; y < rows; y++)
        {
            Color[] shuffledColors = currentRoundColors.OrderBy(c => Random.value).ToArray();

            for (int x = 0; x < columns; x++)
            {
                int index = y * columns + x;
                Color chosenColor = shuffledColors[x % shuffledColors.Length];

                // 🔸 3連続縦方向の回避（2連続はOK）
                if (y >= 2)
                {
                    int aboveIndex1 = (y - 1) * columns + x;
                    int aboveIndex2 = (y - 2) * columns + x;

                    // 上2つの色を取得
                    Color color1 = originalColors[blocks[aboveIndex1]];
                    Color color2 = originalColors[blocks[aboveIndex2]];

                    int safety = 0;
                    while (color1 == color2 && chosenColor == color1 && safety < 10)
                    {
                        // 3連続にならないよう再抽選
                        chosenColor = currentRoundColors[Random.Range(0, currentRoundColors.Length)];
                        safety++;
                    }
                }

                SpriteRenderer renderer = blocks[index].GetComponent<SpriteRenderer>();
                renderer.color = chosenColor;
                originalColors[blocks[index]] = chosenColor;
            }
        }
    }


    //
    void ChooseCorrectColor()
    {
        correctColor = currentRoundColors[Random.Range(0, currentRoundColors.Length)];
    }

    //クリア後の現在のブロック情報
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

    //ブロックが持っている自分の色情報を保存
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

    //
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
