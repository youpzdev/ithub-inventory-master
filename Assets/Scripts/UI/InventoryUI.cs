// UI/InventoryUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventorySlot[] slots;
    [SerializeField] private DraggableItem itemPrefab;
    [SerializeField] private ItemData[] itemDatabase;
    [SerializeField] private GameObject inventoryPanel;

    private void Awake()
    {
        for (int i = 0; i < slots.Length; i++) slots[i].Init(i);
    }
    private void OnEnable()
    {
        EventBus<OnInventoryChanged>.Subscribe(Refresh, this);
    }
    private void OnDisable()
    {
        EventBus<OnInventoryChanged>.Unsubscribe(Refresh);
    }
    private void Start()
    {
        Refresh(new OnInventoryChanged());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) TriggerInventory();
    }

    public void TriggerInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        if (inventoryPanel.activeSelf) Refresh(new OnInventoryChanged());
    }

    private void Refresh(OnInventoryChanged _)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            foreach (Transform child in slots[i].transform) DestroyImmediate(child.gameObject);

            InventoryItem item = InventoryGrid.Instance.GetItem(i);
            if (item == null) continue;

            ItemData data = FindData(item.itemId);
            if (data == null) continue;

            DraggableItem draggable = Instantiate(itemPrefab, slots[i].transform);
            draggable.Init(i, data.icon);
        }
    }

    private ItemData FindData(string id)
    {
        foreach (var d in itemDatabase) if (d.name == id) return d;

        return null;
    }
}