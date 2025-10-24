using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{
    [Header("参照設定")]
    public BlockManager blockManager;   //ブロックマネージャー
    ColorObstacleManager COManager;
    public GameObject correctColorDisplay;   //色表示オブジェクト
    public int stage_count = 1;

    [SerializeField] Text stagenum;
    [SerializeField] GameObject Hero;   //主人公オブジェクト

    [SerializeField]
    float memoryTime = 3f;       //記憶時間
    bool canInput = false;       //キー入力制御
    bool isClear = false;        //クリアフラグ
    int currentRow = 0;          //ブロックの配列数の管理
    Vector3 OriginPos;

    void Start()
    {
        OriginPos = Hero.transform.position;
        COManager = FindAnyObjectByType<ColorObstacleManager>();
        StartCoroutine(GameSequence());
    }


    IEnumerator NextStageSequence()
    {

        isClear = false;
        canInput = false;

        // 元の色を戻して少し待機
        blockManager.RestoreOriginalColors();
        yield return new WaitForSeconds(1f);

        Hero.transform.position = OriginPos;

        stage_count++;

        if(stage_count == 3)
        {
            blockManager.ShuffleRow(0);
        }
        if (stage_count == 4)
        {
            StartCoroutine(COManager.SpawnLoop());
        }
        // 行数を1増やして次のステージへ
        if (blockManager.rows < 4)
        {
            int nextRows = blockManager.rows + 1;
            Debug.Log($"🚀 次のステージへ！行数: {nextRows}");
            blockManager.ResetBlocks(nextRows);
        }
        else
        {
            blockManager.ResetBlocks(blockManager.rows);
        }


        // ステージ情報リセット
        currentRow = 0;

        // 再びメインシーケンスへ
        StartCoroutine(GameSequence());
    }


    IEnumerator GameSequence()
    {

        canInput = false;
        stagenum.text = stage_count.ToString();

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
        if (Input.GetKeyDown(KeyCode.Return) && isClear)
        {
            Hero.transform.position = new Vector3(0, Hero.transform.position.y * blockManager.spacing + 1, 0);

            //ブロックを元の色に戻す
            blockManager.RestoreOriginalColors();

            StartCoroutine(NextStageSequence());
        }

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

           

            isClear = true;
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

      var sr = correctColorDisplay.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = blockManager.CorrectColor;
        }
        else
        {
            Debug.LogError("spriterendererがありません。");
        }
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
