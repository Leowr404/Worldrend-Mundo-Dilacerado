using TMPro;
using UnityEngine;
using System;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;

    public Action OnMoneyChanged; // Evento chamado quando o dinheiro muda

    [Header("Configuração")]
    public int startingMoney = 1000;

    [Header("UI")]
    public TMP_Text moneyText;

    private int currentMoney;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentMoney = startingMoney;
        UpdateMoneyUI();
    }

    // ===============================
    //         MÉTODOS PRINCIPAIS
    // ===============================

    /// <summary>
    /// Tenta comprar algo. Retorna true se conseguiu.
    /// </summary>
    public bool TryBuy(int value)
    {
        if (currentMoney >= value)
        {
            currentMoney -= value;
            UpdateMoneyUI();
            return true;
        }

        Debug.Log("⚠️ Dinheiro insuficiente!");
        return false;
    }

    /// <summary>
    /// Adiciona dinheiro (ex: venda, recompensa, drop).
    /// </summary>
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
    }

    /// <summary>
    /// Remove dinheiro diretamente (opcional).
    /// </summary>
    public void RemoveMoney(int amount)
    {
        currentMoney -= amount;

        if (currentMoney < 0)
            currentMoney = 0;

        UpdateMoneyUI();
    }

    /// <summary>
    /// Usado quando o player vende itens.
    /// </summary>
    public void Sell(int value)
    {
        AddMoney(value);
    }

    // ===============================
    //     ATUALIZAÇÃO DE UI + EVENTO
    // ===============================

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"{currentMoney}";

        // Avisar botões e sistemas que dependem do dinheiro
        OnMoneyChanged?.Invoke();
    }

    public int GetMoney() => currentMoney;
}
