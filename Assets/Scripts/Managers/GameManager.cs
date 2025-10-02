using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    InputManager inputManager;
    private bool Paused;
    //=======//
    public GameObject pauseCanvas;
    void Start()
    {
        Time.timeScale = 1.0f;
        inputManager = InputManager.Instance;
        Cursor.lockState = CursorLockMode.Locked;
        pauseCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (inputManager.Pause)
        {
            PauseGame();
        }
    }
    public void PauseGame()
    {
       
            Paused = !Paused; // inverte o estado
            Time.timeScale = Paused ? 0f : 1f;
        if (Paused)
        {
            // Libera o mouse
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pauseCanvas.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            pauseCanvas.SetActive(false);
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
