
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public string itemName;
    public int price;

    private Button button;
    AudioManager audioManager;

    void Start()
    {
        button = GetComponent<Button>();
        UpdateButtonState();
        EconomyManager.Instance.OnMoneyChanged += UpdateButtonState;
        audioManager = AudioManager.instancia;
    }

    public void Buy()
    {
        if (EconomyManager.Instance.TryBuy(price))
        {
            //Debug.Log($"🛒 Item comprado: {itemName}");
            audioManager.PlaySFX(audioManager.purchase, false);
        }

        // Atualiza o botão depois da compra
        UpdateButtonState();
    }

    public void UpdateButtonState()
    {
        if (button != null)
        {
            bool canBuy = EconomyManager.Instance.GetMoney() >= price;
            button.interactable = canBuy;

            // (Opcional) deixar o botão mais cinza
            var colors = button.colors;
            colors.normalColor = canBuy ? Color.white : new Color(0.5f, 0.5f, 0.5f);
            button.colors = colors;
        }
    }
}
