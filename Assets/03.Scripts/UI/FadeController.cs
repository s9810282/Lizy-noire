using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    [SerializeField] Image fadeImage;
    private static FadeController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        fadeImage = GetComponent<Image>();

        // 맨 처음에는 풀려진 상태
        var color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        fadeImage.gameObject.SetActive(false);
    }

    public static void FadeInOut(float fadeDuration, System.Action onFadeOutComplete)
    {
        instance.StartCoroutine(FadeProcess(fadeDuration, onFadeOutComplete));
    }

    private static IEnumerator FadeProcess(float duration, System.Action onFadeOutComplete)
    {
        instance.fadeImage.gameObject.SetActive(true);

        // Fade In
        yield return instance.StartCoroutine(FadeIn(duration));

        // Callback
        onFadeOutComplete?.Invoke();

        // Fade Out
        yield return instance.StartCoroutine(FadeOut(duration));

        instance.fadeImage.gameObject.SetActive(false);
    }

    private static IEnumerator FadeIn(float duration)
    {
        var color = instance.fadeImage.color;

        float startAlpha = color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 1f, time / duration);
            instance.fadeImage.color = color;
            yield return null;
        }
        color.a = 1f;
        instance.fadeImage.color = color;
    }

    private static IEnumerator FadeOut(float duration)
    {
        var color = instance.fadeImage.color;

        float startAlpha = color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 0f, time / duration);
            instance.fadeImage.color = color;
            yield return null;
        }
        color.a = 0f;
        instance.fadeImage.color = color;
    }
}
