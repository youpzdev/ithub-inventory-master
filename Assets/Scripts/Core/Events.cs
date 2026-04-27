public struct OnInventoryChanged { }

public struct OnItemDropped
{
    public int FromSlot;
    public int ToSlot;
}

public struct OnLootboxOpened
{
    public LootboxType Type;
}

