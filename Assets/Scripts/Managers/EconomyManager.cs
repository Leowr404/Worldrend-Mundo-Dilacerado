using TMPro;
using UnityEngine;
using System;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;

    public Action OnMoneyChanged; // Evento

    [Header("Configuração")]
    public int startingMoney = 100;

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

    public void Sell(int value)
    {
        currentMoney += value;
        UpdateMoneyUI();
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"{currentMoney}";

        OnMoneyChanged?.Invoke(); // Avisar todo mundo que o dinheiro mudou
    }

    public int GetMoney() => currentMoney;
}
