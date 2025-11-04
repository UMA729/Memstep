using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    private bool isStarting = false;

    void Update()
    {
        // どこでもクリックで難易度選択へ
        if (Input.anyKeyDown && !isStarting)
        {
            isStarting = true;
            SceneManager.LoadScene("DifLevelScene");
        }
    }
}
