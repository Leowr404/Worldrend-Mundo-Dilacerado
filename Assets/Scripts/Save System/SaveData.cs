using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    // === Posição e câmera ===
    public Vector3 playerPosition;
    public Quaternion cameraRotation;

    // === Vida ===
    public int playerHealth;

    // === Tempo de jogo ===
    public int playTimeSeconds;

    // === Data do save ===
    public string saveDate;
    public string lastSaveDate;

    // === Moeda ===
    public int coins;

    // === Stats do player ===
    public int level;
    public int currentXP;
    public int xpToNextLevel;
    public int statPoints;
    public int strength;
    public int defense;
    public int vitality;
    public int endurance;
    public int lucky;

    // === Horário do mundo (Skyboxspin) ===
    public int worldHours;
    public int worldMinutes;
    public int worldDays;

    // === Quests ===
    public List<string> completedQuestNames = new List<string>();
    public List<QuestProgress> activeQuestProgress = new List<QuestProgress>();

    [Serializable]
    public class QuestProgress
    {
        public string questName;
        public int currentAmount;
        public bool isReadyToDeliver;
    }

    // === Equipamento equipado ===
    public List<EquippedItem> equipment = new List<EquippedItem>();

    [Serializable]
    public class EquippedItem
    {
        public int slotType;   // (int)EquipSlotType
        public int itemId;
        public int rolledDefense;
        public int rolledAttack;
    }

    // === Inventário ===
    public List<InventoryItem> inventory = new List<InventoryItem>();

    [Serializable]
    public class InventoryItem
    {
        public int slotIndex;
        public int itemId;
        public string itemName;
        public int amount;
        public int rolledDefense;
        public int rolledAttack;

        public InventoryItem() { }

        public InventoryItem(int slotIndex, int itemId, string itemName, int amount)
        {
            this.slotIndex = slotIndex;
            this.itemId = itemId;
            this.itemName = itemName;
            this.amount = amount;
        }
    }
}
