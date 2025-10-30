using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("スコア設定")]
    public int currentScore = 0;
    public int basePoint = 100;

    [Header("コンボ設定")]
    public int comboCount = 0;
    public float comboMultiplier = 1f; // 1.0x, 1.5x, 2.0x...
    public float comboStep = 0.5f;     // 連続ごとの上昇倍率
    public int maxCombo = 10;

    [Header("UI参照")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddScore(bool isCorrect)
    {
        if (isCorrect)
        {
            comboCount++;
            comboMultiplier = 1f + (comboCount - 1) * comboStep;
            comboMultiplier = Mathf.Min(comboMultiplier, 1f + (maxCombo - 1) * comboStep);

            int gained = Mathf.RoundToInt(basePoint * comboMultiplier);
            currentScore += gained;

            Debug.Log($"正解！ +{gained}点 (x{comboMultiplier:F1})");
        }
        else
        {
            Debug.Log("不正解！コンボリセット");
            comboCount = 0;
            comboMultiplier = 1f;
        }

        //UpdateUI();
    }

    public void ResetScore()
    {
        currentScore = 0;
        comboCount = 0;
        comboMultiplier = 1f;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {currentScore}";

        if (comboText != null)
            comboText.text = comboCount > 1 ? $"Combo: x{comboCount}" : "";
    }
}
