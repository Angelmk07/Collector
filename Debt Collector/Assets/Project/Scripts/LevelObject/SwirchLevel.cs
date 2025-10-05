using UnityEngine;
using UnityEngine.SceneManagement;

public class SwirchLevel : MonoBehaviour
{
    [SerializeField] private int nextLevelIndex;
    [SerializeField] private PlayerStatus playerStatus;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerStatus.completedLevels++;
            playerStatus.SaveVariables();
            SceneManager.LoadScene(nextLevelIndex);
        }
    }
}