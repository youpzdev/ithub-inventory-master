using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class InventorySaveSystem
{
    private static readonly string Path = System.IO.Path.Combine(Application.persistentDataPath, "inventory.json");

    [System.Serializable]
    private class SaveData { public List<InventoryItem> items = new(); }

    public static void Save(InventoryItem[] slots)
    {
        var data = new SaveData();
        foreach (var item in slots) if (item != null) data.items.Add(item);

        File.WriteAllText(Path, JsonUtility.ToJson(data, true));
        Debug.Log($"[Save] {Path}");
    }
    public static void Load(InventoryItem[] slots)
    {
        if (!File.Exists(Path)) return;

        var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(Path));
        foreach (var item in data.items)
        {
            if (item.slotIndex < 0 || item.slotIndex >= InventoryGrid.Size) continue;
            slots[item.slotIndex] = item;
        }
    }
    public static void DeleteSave()
    {
        if (File.Exists(Path)) File.Delete(Path);
    }
}