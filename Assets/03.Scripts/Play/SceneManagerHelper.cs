using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneManagerHelper
{
    // ���� Ȱ��ȭ�Ǿ� �ִ� �� �̸�
    public static string CurrentSceneName
    {
        get { return SceneManager.GetActiveScene().name; }
    }

    // ���� Ȱ��ȭ�Ǿ� �ִ� �� �ε���
    public static int CurrentSceneIndex
    {
        get { return SceneManager.GetActiveScene().buildIndex; }
    }

    // Ư�� �̸��� ���� Fade ���Էηε�
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
                Debug.LogWarning($"'{sceneName}'�̶�� �̸��� ���� ����� �� �����ϴ�.");
            }
        });
    }

    // Ư�� �ε����� ���� Fade ���Էηε�
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
                Debug.LogWarning($"'{sceneIndex}'�̶�� �ε����� ���� ����� �� �����ϴ�.");
            }
        });
    }

    // ���� �� ���ε�
    public static void ReloadCurrentSceneWithFade(float fadeDuration = 1f)
    {
        FadeController.FadeInOut(fadeDuration, () =>
        {
            SceneManager.LoadScene(CurrentSceneName);
        });
    }
}
