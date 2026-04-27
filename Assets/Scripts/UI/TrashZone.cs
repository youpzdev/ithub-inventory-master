using UnityEngine;
using UnityEngine.EventSystems;
public class TrashZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggable = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (draggable == null) return;
        InventoryGrid.Instance.RemoveItem(draggable.SlotIndex);
        Destroy(draggable.gameObject);
    }
}