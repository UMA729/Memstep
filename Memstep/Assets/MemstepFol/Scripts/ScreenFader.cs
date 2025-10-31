using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image fadePanel;
    public float fadeSpeed = 1.5f;

    // �ォ��Ó]���Ă����A�j���[�V����
    public IEnumerator FadeFromTop()
    {
        if (fadePanel == null)
        {
            Debug.LogError("FadePanel�����ݒ�ł��I");
            yield break;
        }

        // �ォ�牺�ɍ�����L�΂�
        RectTransform rt = fadePanel.rectTransform;
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(0, 0);

        fadePanel.color = new Color(0, 0, 0, 1);

        float height = Screen.height;
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * fadeSpeed;
            float h = Mathf.Lerp(0, height, elapsed);
            rt.sizeDelta = new Vector2(0, h);
            yield return null;
        }

        rt.sizeDelta = new Vector2(0, height);
    }

    // �t�Đ��F��ɖ߂�i�t�F�[�h�A�E�g�j
    public IEnumerator FadeToTop()
    {
        RectTransform rt = fadePanel.rectTransform;
        float height = Screen.height;
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * fadeSpeed;
            float h = Mathf.Lerp(height, 0, elapsed);
            rt.sizeDelta = new Vector2(0, h);
            yield return null;
        }

        rt.sizeDelta = new Vector2(0, 0);
    }
}
