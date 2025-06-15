using UnityEngine;

public class MainManager : MonoBehaviour
{

    public void Start()
    {
        StartCoroutine(FadeController.FadeOut(1f));
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        SceneManagerHelper.LoadSceneWithFade("Play", 1f);
    }
}
