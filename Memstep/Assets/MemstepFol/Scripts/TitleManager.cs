using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    public GameObject titleUI;       // �^�C�g��Canvas
    public GameObject difficultyUI;  // ��Փx�I��Canvas
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

        // ���N���b�N or Enter or Space �ŊJ�n
        if (Input.anyKey )
        {
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        isTransitioning = true;

        // �t�F�[�h�A�E�g�Ȃǂ̉��o����ꂽ���ꍇ�͂�����
        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(SFader.FadeFromTop());

        titleUI.SetActive(false);
        difficultyUI.SetActive(true);
    }
}
