using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    [Header("Lista de todos os itens do jogo")]
    public List<Objects> items = new List<Objects>();

    private void Awake()
    {
        // Singleton básico
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // opcional: DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Retorna um item pelo nome (para o sistema de save/load).
    /// </summary>
    public Objects GetItemByName(string itemName)
    {
        foreach (var item in items)
        {
            if (item != null && item.itemName == itemName)
                return item;
        }

        return null;
    }

    /// <summary>
    /// Alternativa por id (se preferir salvar por id em vez de nome).
    /// </summary>
    public Objects GetItemById(int id)
    {
        foreach (var item in items)
        {
            if (item != null && item.itemId == id)
                return item;
        }

        return null;
    }
}
