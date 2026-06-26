using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Botão Continuar (só aparece se houver save)")]
    public GameObject continueButton;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;

        // mostra o Continue só se existe auto-save (slot 0 = salvo ao completar quest)
        if (continueButton != null)
            continueButton.SetActive(SaveSystem.Exists(0));
    }

    // Novo jogo — começa do início
    public void StartGame()
    {
        SaveManager.loadOnStart = false;
        LoadingManager.sceneToLoad = "TestPlace";
        SceneManager.LoadScene("Loading");
    }

    // Continuar — carrega o último save ao entrar na cena
    public void ContinueGame()
    {
        SaveManager.loadOnStart = true;
        LoadingManager.sceneToLoad = "TestPlace";
        SceneManager.LoadScene("Loading");
    }

    public void CloseGame()
    {
        //Futuro Codigo Para Salvar antes de quitar//
        Application.Quit();
    }
}
