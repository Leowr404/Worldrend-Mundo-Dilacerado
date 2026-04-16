using UnityEngine;

public class ItemPopUp : MonoBehaviour
{
    [Header("Configuração do item")]
    public string itemName = "Item Sem Nome";

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        // Exibir notificação animada
        NotificationUI.Show(itemName + " coletado!");

        // Destruir item se configurado
        
    }
}
