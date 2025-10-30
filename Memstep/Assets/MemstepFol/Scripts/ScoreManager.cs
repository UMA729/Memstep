using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("�X�R�A�ݒ�")]
    public int currentScore = 0;
    public int basePoint = 100;

    [Header("�R���{�ݒ�")]
    public int comboCount = 0;
    public float comboMultiplier = 1f; // 1.0x, 1.5x, 2.0x...
    public float comboStep = 0.5f;     // �A�����Ƃ̏㏸�{��
    public int maxCombo = 10;

    [Header("UI�Q��")]
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

            Debug.Log($"�����I +{gained}�_ (x{comboMultiplier:F1})");
        }
        else
        {
            Debug.Log("�s�����I�R���{���Z�b�g");
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
