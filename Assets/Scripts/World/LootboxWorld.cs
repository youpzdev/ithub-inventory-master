using UnityEngine;

public class LootboxWorld : MonoBehaviour, IInteractable
{
    [SerializeField] private bool canInteract = true;
    [SerializeField] private string promptText = "";
    [SerializeField] private bool requiresHold = true;
    [SerializeField] private float holdDuration = 3f;
    [SerializeField] private LootboxType type;
    [SerializeField] private ItemData[] possibleItems;
    [SerializeField] private GameObject visual;

    public void Open()
    {
        ApplyEffect();
        Destroy(visual);
        Destroy(gameObject);
    }

    public void OnInteract() { }
    public void OnHoldInteract() => Open();
    public bool CanInteract => canInteract;
    public float HoldDuration => holdDuration;
    public bool RequiresHold => requiresHold;
    public string GetPromptText() => promptText;

    private void ApplyEffect()
    {
        switch (type)
        {
            case LootboxType.Common: GiveItems(); break;
            case LootboxType.NiggaWtf: InventoryGrid.Instance.ClearAll(); InventorySaveSystem.DeleteSave(); break;
            case LootboxType.Great: InventoryGrid.Instance.RemoveRandom(0.10f); break;
            case LootboxType.Cool: InventoryGrid.Instance.RemoveRandom(0.30f); break;
            case LootboxType.ExtraNigger: InventoryGrid.Instance.RemoveRandom(0.50f); break;
        }
    }

    private void GiveItems()
    {
        if (possibleItems == null || possibleItems.Length == 0) return;

        int count = Random.Range(1, 4);
        for (int i = 0; i < count; i++)
        {
            ItemData item = possibleItems[Random.Range(0, possibleItems.Length)];
            if (item == null) continue;
            InventoryGrid.Instance.TryAddItem(item);
        }
    }
}