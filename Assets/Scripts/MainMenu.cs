using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("TestPlace");
    }
    public void CloseGame()
    {
        //Futuro Codigo Para Salvar antes de quitar//
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
