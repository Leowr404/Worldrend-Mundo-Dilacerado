using UnityEngine;

public class OpenShop : MonoBehaviour
{
    public GameObject ShopCanvas;
    public GameObject interactionIcon;

    private UiManager uiManager;
    private bool playerInRange;

    AudioManager audioManager;

    void Start()
    {
        //uiManager = FindAnyObjectByType<UiManager>();
        if (interactionIcon) interactionIcon.SetActive(false);
        //audioManager = AudioManager.instancia;
        ShopCanvas.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && InputManager.Instance.Interact)
        {
            //audioManager.PlayNPCTalk();
            InputManager.Instance.SwitchToUI();
            Cursor.lockState = CursorLockMode.None;
            ShopCanvas.SetActive(true );
        }
    }
    public void CloseShop()
    {
        ShopCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        InputManager.Instance.SwitchToPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactionIcon) interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactionIcon) interactionIcon.SetActive(false);
            ShopCanvas.SetActive(false);
        }
    }
}
