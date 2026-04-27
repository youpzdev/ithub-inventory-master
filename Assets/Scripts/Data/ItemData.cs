using UnityEngine;
[CreateAssetMenu(fileName = "ItemData", menuName = "youpzdev/inventory item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [Range(1, 100)] public int damage;
    [Range(1, 100)] public int armor;
    [Range(1, 5)] public int rarity;
    public float weight;
}