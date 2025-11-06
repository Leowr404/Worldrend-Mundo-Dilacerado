using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Configuração do Item")]
    [SerializeField] private string itemName; // Nome do item (ex: "Poção de Vida")
    [SerializeField] private int itemID;      // ID do item (para controle futuro em inventário)
    [SerializeField] private int amount = 1;  // Quantidade (ex: 1 poção, 10 moedas)

    // Detecta quando algo entra no "trigger" do item
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se quem entrou é o jogador (precisa da Tag "Player")
        if (other.CompareTag("Player"))
        {
            Collect(other.gameObject);
            PlayerStats health = other.GetComponent<PlayerStats>();
            if (health != null)
            {
                health.TakeDamage(Random.Range(50,5)); // aplica dano
                Debug.Log("Playter Atingido");
            }

        }
    }

    // Lógica da coleta
    private void Collect(GameObject player)
    {
        Debug.Log("Coletou: " + itemName + " x" + amount);


        // 🔹 Aqui você pode ligar ao inventário do player futuramente:
        // player.GetComponent<Inventory>().AddItem(itemID, amount);

        // Remove o item do cenário
        Destroy(gameObject);
    }
}
