using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Canvas rootCanvas;

    public int SlotIndex { get; private set; }

    private void Awake() => rootCanvas = GetComponentInParent<Canvas>();

    public void Init(int slotIndex, Sprite sprite)
    {
        SlotIndex = slotIndex;
        icon.sprite = sprite;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        rootCanvas = GetComponentInParent<Canvas>();
        originalParent = transform.parent;
        transform.SetParent(rootCanvas.transform);
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) => transform.position = eventData.position;

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        if (transform.parent == rootCanvas.transform)
        {
            DestroyImmediate(gameObject);
            EventBus<OnInventoryChanged>.Raise(new OnInventoryChanged());
        }
    }
}