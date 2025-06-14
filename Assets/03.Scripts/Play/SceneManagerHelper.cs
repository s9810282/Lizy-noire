using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneManagerHelper
{
    // 현재 활성화되어 있는 씬 이름
    public static string CurrentSceneName
    {
        get { return SceneManager.GetActiveScene().name; }
    }

    // 현재 활성화되어 있는 씬 인덱스
    public static int CurrentSceneIndex
    {
        get { return SceneManager.GetActiveScene().buildIndex; }
    }

    // 특정 이름의 씬을 Fade 포함로로드
    public static void LoadSceneWithFade(string sceneName, float fadeDuration = 1f)
    {
        FadeController.FadeInOut(fadeDuration, () =>
        {
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning($"'{sceneName}'이라는 이름의 씬을 사용할 수 없습니다.");
            }
        });
    }

    // 특정 인덱스의 씬을 Fade 포함로로드
    public static void LoadSceneWithFade(int sceneIndex, float fadeDuration = 1f)
    {
        FadeController.FadeInOut(fadeDuration, () =>
        {
            if (sceneIndex >= 0 &&
                sceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(sceneIndex);
            }
            else
            {
                Debug.LogWarning($"'{sceneIndex}'이라는 인덱스의 씬을 사용할 수 없습니다.");
            }
        });
    }

    // 현재 씬 리로드
    public static void ReloadCurrentSceneWithFade(float fadeDuration = 1f)
    {
        FadeController.FadeInOut(fadeDuration, () =>
        {
            SceneManager.LoadScene(CurrentSceneName);
        });
    }
}
