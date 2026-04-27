using UnityEngine;
using System;
[System.Serializable]
public class InventoryItem
{
    public string itemId;
    public int slotIndex;
    public int damage;
    public int armor;
    public int rarity;
    public float weight;
    public InventoryItem(ItemData data, int slot)
    {
        itemId = data.name;
        slotIndex = slot;
        damage = data.damage;
        armor = data.armor;
        rarity = data.rarity;
        weight = data.weight;
    }
}