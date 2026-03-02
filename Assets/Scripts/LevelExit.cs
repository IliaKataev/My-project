using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public int nextSceneIndex;
    public GameObject levelCompletePanel;
    private bool triggered = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggered && collision.CompareTag("Player"))
        {
            triggered = true;
            levelCompletePanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    //private void LoadNextLevel()
    //{
    //    SceneManager.LoadScene(nextSceneIndex);
    //}
}
