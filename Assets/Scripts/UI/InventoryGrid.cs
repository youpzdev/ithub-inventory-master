// Inventory/InventoryGrid.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class InventoryGrid : MonoBehaviour
{
    public static InventoryGrid Instance { get; private set; }
    public const int Size = 25;
    private InventoryItem[] slots = new InventoryItem[Size];
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }
    private void Start()
    {
        InventorySaveSystem.Load(slots);
        EventBus<OnInventoryChanged>.Raise(new OnInventoryChanged());
    }
    public bool TryAddItem(ItemData data)
    {
        int free = GetFreeSlot();
        if (free == -1) return false;
        slots[free] = new InventoryItem(data, free);
        Save();
        return true;
    }
    public void MoveItem(int from, int to)
    {
        if (slots[from] == null) return;
        if (slots[to] != null) return;
        slots[from].slotIndex = to;
        slots[to] = slots[from];
        slots[from] = null;
        Save();
    }
    public void RemoveItem(int slotIndex)
    {
        if (slots[slotIndex] == null) return;
        slots[slotIndex] = null;
        Save();
    }
    public void ClearAll()
    {
        for (int i = 0; i < Size; i++)
            slots[i] = null;
        Save();
    }
    public void RemoveRandom(float percent)
    {
        var occupied = slots.Select((item, idx) => (item, idx)).Where(x => x.item != null).ToList();

        int count = Mathf.Max(1, Mathf.RoundToInt(occupied.Count * percent));
        var toRemove = occupied.OrderBy(_ => Random.value).Take(count);
        foreach (var (_, idx) in toRemove) slots[idx] = null;

        Save();
    }
    public InventoryItem GetItem(int slotIndex) => slots[slotIndex];
    public InventoryItem[] GetAll() => slots;
    private int GetFreeSlot()
    {
        for (int i = 0; i < Size; i++) if (slots[i] == null) return i;
        return -1;
    }
    private void Save()
    {
        InventorySaveSystem.Save(slots);
        EventBus<OnInventoryChanged>.Raise(new OnInventoryChanged());
    }
}