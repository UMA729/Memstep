using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{
    [Header("参照設定")]
    public BlockManager blockManager;
    public Image correctColorDisplay;

    [Header("設定")]
    public float memoryTime = 3f; // 記憶時間
    public bool canInput = false;
    public int currentRow = 0;

    void Start()
    {
        StartCoroutine(GameSequence());
    }

    IEnumerator GameSequence()
    {
        canInput = false;

        // 1️⃣ 記憶フェーズ
        Debug.Log("🧠 記憶フェーズ開始");
        yield return new WaitForSeconds(memoryTime);

        // 2️⃣ ブロックを黒くする（暗転代わり）
        Debug.Log("🌑 ブロックを暗転");
        DarkenBlocks();

        // 3️⃣ 正解色ディスプレイを更新（暗転後に残す）
        UpdateCorrectColorDisplay();

        // 4️⃣ 1秒待ってから入力開始
        yield return new WaitForSeconds(1f);

        Debug.Log("✅ 入力開始OK");
        canInput = true;
    }

    void Update()
    {
        if (!canInput) return;

        if (Input.GetKeyDown(KeyCode.A)) SelectBlock(0);
        if (Input.GetKeyDown(KeyCode.S)) SelectBlock(1);
        if (Input.GetKeyDown(KeyCode.D)) SelectBlock(2);
    }

    void SelectBlock(int column)
    {
        GameObject block = blockManager.GetBlockAt(currentRow, column);

        if (blockManager.CheckBlock(block))
        {
            Debug.Log($"✅ 正解！列 {column + 1}");
            block.GetComponent<SpriteRenderer>().color = Color.white;
            currentRow++;

            if (currentRow >= blockManager.rows)
            {
                Debug.Log("🎉 全クリア！");
                canInput = false;
            }
        }
        else
        {
            Debug.Log($"❌ 不正解！列 {column + 1}");
            block.GetComponent<SpriteRenderer>().color = Color.black;
            canInput = false;
        }
    }

    void UpdateCorrectColorDisplay()
    {
        if (correctColorDisplay == null)
        {
            Debug.LogError("❌ correctColorDisplay が未設定です！");
            return;
        }

        correctColorDisplay.color = blockManager.CorrectColor;
        correctColorDisplay.enabled = true;
        Debug.Log($"🎨 正解色をUIに反映: {blockManager.CorrectColor}");
    }

    void DarkenBlocks()
    {
        foreach (var block in blockManager.GetAllBlocks())
        {
            var sr = block.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // 色を保存しておく
                Color original = sr.color;
                sr.color = new Color(original.r * 0.1f, original.g * 0.1f, original.b * 0.1f);
            }
        }
    }
}
