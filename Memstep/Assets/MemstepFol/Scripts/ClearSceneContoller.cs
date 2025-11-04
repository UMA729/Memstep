using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ClearSceneContoller : MonoBehaviour
{
    public Text scoreText;
    ScoreManager ScoreManager;
    public Button backToTitleButton;

    void Start()
    {
        scoreText.text = FindAnyObjectByType<ScoreManager>()?.currentScore.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        { 
            SceneManager.LoadScene("TitleScene");
        }
    }
}
