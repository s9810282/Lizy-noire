using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    [SerializeField] Image fadeImage;
    private static FadeController instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        
    }

    public static void FadeInOut(float fadeDuration, System.Action onFadeOutComplete)
    {
        instance.StartCoroutine(FadeProcess(fadeDuration, onFadeOutComplete));
    }

    private static IEnumerator FadeProcess(float duration, System.Action onFadeOutComplete)
    {

        // Fade In
        yield return instance.StartCoroutine(FadeIn(duration));

        // Callback
        onFadeOutComplete?.Invoke();

        // Fade Out
        yield return instance.StartCoroutine(FadeOut(duration));


    }

    public static IEnumerator FadeIn(float duration)
    {
        instance.fadeImage.gameObject.SetActive(true);
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

    public static IEnumerator FadeOut(float duration)
    {
        instance.fadeImage.gameObject.SetActive(true);

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
        instance.fadeImage.gameObject.SetActive(false);
    }
}
