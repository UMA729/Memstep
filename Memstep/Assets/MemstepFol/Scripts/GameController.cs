using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{
    [Header("参照設定")]
    public BlockManager blockManager;   //ブロックマネージャー
    public Image correctColorDisplay;   //色表示オブジェクト

    [SerializeField] GameObject Hero;   //主人公オブジェクト

    [Header("設定")]
    public float memoryTime = 3f;       //記憶時間
    public bool canInput = false;       //キー入力制御
    public int currentRow = 0;          //ブロックの配列数の管理

    void Start()
    {
        StartCoroutine(GameSequence());
    }

    IEnumerator GameSequence()
    {
        canInput = false;

        //記憶フェーズ
        Debug.Log("記憶フェーズ開始");
        yield return new WaitForSeconds(memoryTime);

        //ブロックを黒くする（暗転代わり）
        Debug.Log("ブロックを暗転");
        blockManager.SaveOriginalColors();
        DarkenBlocks();

        //正解色ディスプレイを更新（暗転後に残す）
        UpdateCorrectColorDisplay();

        //1秒待ってから入力開始
        yield return new WaitForSeconds(0.5f);

        Debug.Log("入力開始OK");
        canInput = true;
    }

    void Update()
    {
        if (!canInput) return;

        //キー入力
        if (Input.GetKeyDown(KeyCode.A)) SelectBlock(0);
        if (Input.GetKeyDown(KeyCode.S)) SelectBlock(1);
        if (Input.GetKeyDown(KeyCode.D)) SelectBlock(2);
    }

    //選択したブロックの処理
    void SelectBlock(int column)
    {
        GameObject block = blockManager.GetBlockAt(currentRow, column);//選択されたブロックオブジェクト

        //正解の処理
        if (blockManager.CheckBlock(block))
        {
            Debug.Log($"正解！列 {column + 1}");
            //主人公を選んだ場所のブロックに移動
            Hero.transform.position = block.transform.position;
            currentRow++;
        }
        //不正解の処理
        else
        {
            Debug.Log($"不正解！列 {column + 1}");
            //主人公を選んだ場所のブロックに移動
            Hero.transform.position = block.transform.position;
            currentRow++;
        }
        //クリア処理
        if (currentRow >= blockManager.rows)
        {
            Debug.Log("ステージ終了");
            canInput = false;

            //ブロックを元の色に戻す
            blockManager.RestoreOriginalColors();
        }
    }

    //正解色を表示
    void UpdateCorrectColorDisplay()
    {
        if (correctColorDisplay == null)
        {
            Debug.LogError("correctColorDisplay が未設定です！");
            return;
        }

        correctColorDisplay.color = blockManager.CorrectColor;
        correctColorDisplay.enabled = true;
        Debug.Log($"正解色をUIに反映: {blockManager.CorrectColor}");
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
