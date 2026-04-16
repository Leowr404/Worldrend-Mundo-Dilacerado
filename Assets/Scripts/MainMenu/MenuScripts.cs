using UnityEngine;

public class MenuScripts : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Settings;
    public GameObject Credits;
    AudioManager audioManager;
    void Start()
    {
        audioManager = AudioManager.instancia;
        Settings.gameObject.SetActive(false);
        Credits.gameObject.SetActive(false);
        
    }
    public void OpenSettingsMenu()
    {
        Settings.gameObject.SetActive(true);
        audioManager.PlaySFX(audioManager.ClickButton, false);
        
    }

    public void CloseSettingsMenu()
    {
        Settings.gameObject.SetActive(false);
        audioManager.PlaySFX(audioManager.ClickButton, false);
    }

    public void OpenCreditsMenu()
    {
        Credits.gameObject.SetActive(true);
        audioManager.PlaySFX(audioManager.ClickButton, false);

    }

    public void CloseCreditsMenu()
    {
        Credits.gameObject.SetActive(false);
        audioManager.PlaySFX(audioManager.ClickButton, false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
