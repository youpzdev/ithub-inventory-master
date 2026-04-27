using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image highlight;
    public int SlotIndex { get; private set; }
    public void Init(int index)
    {
        SlotIndex = index;
    }
    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggable = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (draggable == null) return;
        InventoryGrid.Instance.MoveItem(draggable.SlotIndex, SlotIndex);
    }
    public void SetHighlight(bool active)
    {
        if (highlight != null) highlight.enabled = active;
    }
}