using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public FadeManager FadeManager;

    public void LeadNextScene()
    {
        Time.timeScale = 1.0f;
        StartCoroutine(LoadWithFade());
    }

    private IEnumerator LoadWithFade()
    {
        yield return FadeManager.FadeOut();

        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }
}
