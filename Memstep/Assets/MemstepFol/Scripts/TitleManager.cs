using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    public GameObject titleUI;       // タイトルCanvas
    public GameObject difficultyUI;  // 難易度選択Canvas
    ScreenFader SFader;
    private bool isTransitioning = false;
    void Start()
    {
        SFader = FindAnyObjectByType<ScreenFader>();
        titleUI.SetActive(true);
        difficultyUI.SetActive(false);
    }

    private void Update()
    {
        if (isTransitioning) return;

        // 左クリック or Enter or Space で開始
        if (Input.anyKey )
        {
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        isTransitioning = true;

        // フェードアウトなどの演出を入れたい場合はここに
        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(SFader.FadeFromTop());

        titleUI.SetActive(false);
        difficultyUI.SetActive(true);
    }
}
