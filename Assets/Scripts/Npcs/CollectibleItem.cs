using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("Configuração do item")]
    public string itemID;
    public string itemName = "Item";
    public int amount = 1;
    public bool destroyOnCollect = true;

    [Header("Física")]
    public bool esperarParar = true;      // espera o item parar de cair
    public float velocidadeMinima = 0.1f; // considera parado abaixo disso

    private bool coletado = false;
    private bool podeColetar = false;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Se não precisa esperar, já libera a coleta
        if (!esperarParar) podeColetar = true;
    }

    private void Update()
    {
        // Libera coleta quando o item parar de se mover
        if (!podeColetar && rb != null && rb.linearVelocity.magnitude < velocidadeMinima)
            podeColetar = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (coletado) return;
        if (!podeColetar) return;
        if (!other.CompareTag("Player")) return;

        coletado = true;
        QuestManager.Instance.ReportCollect(itemID, amount);
        UiManager.Notify(itemName + " coletado!");

        if (destroyOnCollect) Destroy(gameObject);
    }
}