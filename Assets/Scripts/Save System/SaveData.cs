using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public List<ItemData> inventoryItems = new List<ItemData>();
}

[Serializable]
public class ItemData
{
    public string itemName;
    public int itemCount;
}
