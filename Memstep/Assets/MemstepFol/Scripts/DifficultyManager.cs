using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DifficultyManager : MonoBehaviour
{
    public Button normalButton;
    public Button hardButton;
    public Button exstraButton;

    void Start()
    {
        normalButton.onClick.AddListener(() => SelectDifficulty("Normal"));
        hardButton.onClick.AddListener(() => SelectDifficulty("Hard"));
        exstraButton.onClick.AddListener(() => SelectDifficulty("Exstra"));
    }

    void SelectDifficulty(string mode)
    {
        // 難易度を保存（どのシーンでも読めるように）
        PlayerPrefs.SetString("Difficulty", mode);
        PlayerPrefs.Save();

        SceneManager.LoadScene("GameScene");
    }
}
