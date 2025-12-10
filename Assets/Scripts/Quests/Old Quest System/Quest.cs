using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "RPG/Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    [TextArea(2, 4)] public string description;
    public bool isCompleted;
    public int requiredItems = 3;
    public int collectedItems = 0;
    public int rewardMoney;
    public int XpReward;
    public bool money;

    public void ResetProgress()
    {
        isCompleted = false;
        collectedItems = 0;
    }

    public void AddProgress(int amount = 1)
    {
        collectedItems += amount;
        if (collectedItems >= requiredItems)
        {
            collectedItems = requiredItems;
            isCompleted = true;
        }
    }
    public void GiveRewards(PlayerStats player)
    {
        Debug.Log($"🎁 Recompensa entregue pela quest: {questName}");
        player.AddXP(XpReward);
        player.AddXP(XpReward);
        player.AddXP(XpReward);
        player.AddXP(XpReward);
        player.AddXP(XpReward);
        player.AddXP(XpReward);
        player.AddXP(XpReward);
        player.AddXP(XpReward);
        player.AddXP(XpReward);
        player.AddXP(XpReward);
        player.AddXP(XpReward);
        if (money == true ) 
        {
            EconomyManager.Instance.AddMoney(rewardMoney);
            AudioManager.instancia.PlaySFX(AudioManager.instancia.rewardMoney, false);
        }


    }
}