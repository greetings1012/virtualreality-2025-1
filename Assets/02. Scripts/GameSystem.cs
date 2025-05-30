using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    public GameObject gameOverPanel;
    private bool isGameOver = false;

    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        StartCoroutine(ShowGameOverPanel());
    }

    private IEnumerator ShowGameOverPanel()
    {
        Debug.Log("234");
        yield return null;
        //gameOverPanel.SetActive(true);
        //Time.timeScale = 0f;
        
        //yield return new WaitForSecondsRealtime(5f);  

        //Time.timeScale = 1f;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
