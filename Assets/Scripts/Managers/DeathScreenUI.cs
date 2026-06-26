using UnityEngine;
using UnityEngine.SceneManagement;

// Tela de morte: pausa o jogo e mostra os botões Tentar Novamente / Menu.
public class DeathScreenUI : MonoBehaviour
{
    public static DeathScreenUI Instance;

    [Header("UI")]
    public GameObject deathPanel;

    private void Awake()
    {
        Instance = this;
        if (deathPanel != null) deathPanel.SetActive(false);
    }

    public void ShowDeathScreen()
    {
        if (deathPanel != null) deathPanel.SetActive(true);
        Time.timeScale = 0f; // pausa tudo
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Botão "Tentar Novamente": carrega o último save e revive
    public void Retry()
    {
        Time.timeScale = 1f;
        if (deathPanel != null) deathPanel.SetActive(false);

        if (SaveManager.Instance != null)
            SaveManager.Instance.LoadFromSlot(0);

        PlayerStats stats = FindAnyObjectByType<PlayerStats>();
        if (stats != null) stats.Revive();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Botão "Menu": volta ao menu principal
    public void BackToMenu()
    {
        Time.timeScale = 1f;
        LoadingManager.sceneToLoad = "MainMenu";
        SceneManager.LoadScene("Loading");
    }
}
