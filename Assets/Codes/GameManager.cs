using NUnit.Framework;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public void Start()
    {
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        Time.timeScale = 1f;
        EnemyController.eliminados = 0;
    }
}
