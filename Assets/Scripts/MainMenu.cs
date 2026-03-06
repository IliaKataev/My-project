using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject levelsPanel;

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ShowLevels()
    {
        levelsPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
