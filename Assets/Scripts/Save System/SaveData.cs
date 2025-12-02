using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    // === Dados de mundo / jogador ===
    public Vector3 playerPosition;
    public Quaternion cameraRotation;

    // Tempo de jogo
    public int playTimeSeconds;

    // Data e hora do save
    public string saveDate;
    public string lastSaveDate;

    // Lista de itens do inventário
    public List<InventoryItem> inventory = new List<InventoryItem>();


    // -------------------------
    // Tipo usado para salvar um item do inventário
    // -------------------------
    [Serializable]
    public class InventoryItem
    {
        public int slotIndex;
        public int itemId;     // ID ÚNICO DO ITEM
        public string itemName;   // Nome (apenas para debug)
        public int amount;

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
