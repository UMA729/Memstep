using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{
    [Header("参照設定")]
    public BlockManager blockManager;   //ブロックマネージャー
    ColorObstacleManager COManager;     //おじゃま色ブロック
    PlayerController Pcon;              //プレイヤーコントローラー

    public GameObject correctColorDisplay;   //色表示オブジェクト
    public int stage_count = 1;

    [SerializeField] Text stagenum;

    public bool isClear = false; //クリアフラグ
    bool canInput = false;       //キー入力制御
    float memoryTime = 3f;       //記憶時間
    int currentRow = 0;          //ブロックの配列数の管理
    Vector3 OriginPos;

    void Start()
    {
        COManager = FindAnyObjectByType<ColorObstacleManager>();
        Pcon      = FindAnyObjectByType<PlayerController>();
        StartCoroutine(GameSequence());
    }

    IEnumerator NextStageSequence()
    {
        //一秒待つ
        yield return new WaitForSeconds(0.1f);
        //プレイヤーをゴール位置に
        Pcon.Player.transform.position = new Vector3(0, Pcon.Player.transform.position.y * blockManager.Yspacing + 1, 0);

        //エンターキーが押されるまで待つ
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

        //何連続正解中か処理（未実装）

        //動くブロックを消す
        FindAnyObjectByType<ColorObstacleManager>()?.DestroyObj();
        //プレイヤーの位置を最初の位置に
        Pcon.InitializePos();

        isClear = false;
        canInput = false;

        // 元の色を戻して少し待機
        blockManager.RestoreOriginalColors();
        yield return new WaitForSeconds(1f);

        stage_count++;

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

        Pcon.InitializePos();

        canInput = false;
        stagenum.text = stage_count.ToString();

        //記憶フェーズ
        Debug.Log("記憶フェーズ開始");
        yield return new WaitForSeconds(memoryTime);


        if (stage_count >= 3)
        {
            Debug.Log("シャッフル始まるよー");
            blockManager.ShuffleRow();
        }
        if (stage_count == 4)
        {
            StartCoroutine(COManager.SpawnLoop());
        }

        if (stage_count == 5)
        {

        }

        yield return new WaitForSeconds(1f);

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
        if (Input.GetKeyDown(KeyCode.R))RestartGame();


        if (Input.GetKeyDown(KeyCode.Return) && isClear)
        {
            //ブロックを元の色に戻す
            blockManager.RestoreOriginalColors();

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
        Debug.Log("入ってます");

        GameObject block = blockManager.GetBlockAt(currentRow, column);//選択されたブロックオブジェクト

        //正解の処理
        if (blockManager.CheckBlock(block))
        {
            ScoreManager.Instance.AddScore(true);

            Debug.Log($"正解！列 {column + 1}");
            //主人公を選んだ場所のブロックに移動
            Pcon.Player.transform.position = block.transform.position;
            currentRow++;
        }
        //不正解の処理
        else
        {
            ScoreManager.Instance.AddScore(false);

            Debug.Log($"不正解！列 {column + 1}");
            //主人公を選んだ場所のブロックに移動
            Pcon.Player.transform.position = block.transform.position;
            currentRow++;
        }
        //クリア処理
        if (currentRow >= blockManager.rows)
        { 
            Debug.Log("ステージ終了");

            canInput = false;

            StartCoroutine(NextStageSequence());
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

    //色ブロックを黒に
    void DarkenBlocks()
    {
        foreach (var block in blockManager.GetAllBlocks())
        {
            var sr = block.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // 色を保存しておく
                Color original = sr.color;
                sr.color = new Color(original.r * 0.01f, original.g * 0.01f, original.b * 0.01f);
            }
        }
    }

    //ゲームリスタート
    public void RestartGame()
    {
        StopAllCoroutines(); // 進行中のコルーチンを停止
        isClear = false;
        canInput = false;
        currentRow = 0;
        stage_count = 1;

        // ブロックを初期状態に戻す
        blockManager.ResetBlocks(3);

        // プレイヤーを初期位置に戻す
        Pcon.Player.transform.position = OriginPos;

        // スコアなどをリセットする場合はここでリセット
        ScoreManager.Instance?.ResetScore(); // ←スコア導入済みなら

        // ゲームを再開
        StartCoroutine(GameSequence());
    }
}