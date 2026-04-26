using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "RPG/Quest")]
public class Quest : ScriptableObject
{
    [Header("Identificação")]
    public string questName;
    [TextArea(2, 4)] public string description;

    [Header("Objetivo")]
    public QuestObjective objective;

    [Header("Entrega")]
    public string deliveryNPCID; // ID do NPC que entrega a recompensa

    [Header("Recompensas")]
    public int xpReward;
    public int moneyReward;
    public bool givesMoney;

    // Runtime — não persiste no asset
    [System.NonSerialized] public bool isCompleted;
    [System.NonSerialized] public bool isReadyToDeliver; // objetivo feito, falta falar com NPC

    public void ResetProgress()
    {
        isCompleted = false;
        isReadyToDeliver = false;
        objective.currentAmount = 0;
    }

    // Retorna true se o objetivo acabou de ser completado
    public bool TryCompleteObjective()
    {
        if (isReadyToDeliver || isCompleted) return false;
        if (!objective.IsComplete()) return false;
        isReadyToDeliver = true;
        return true;
    }

    // Chamado quando player volta ao NPC e entrega
    public void DeliverAndReward(PlayerStats player)
    {
        if (!isReadyToDeliver) return;
        isCompleted = true;
        isReadyToDeliver = false;

        player.AddXP(xpReward);
        if (givesMoney)
        {
            EconomyManager.Instance.AddMoney(moneyReward);
            AudioManager.instancia.PlaySFX(AudioManager.instancia.rewardMoney, false);
        }
    }
}