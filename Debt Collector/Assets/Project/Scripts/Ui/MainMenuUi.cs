using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUi : MonoBehaviour
{
    public void EnterGame(int indexNextLevel)
    {
        SceneManager.LoadScene(indexNextLevel);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}