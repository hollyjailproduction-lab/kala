using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class InventorySO : ScriptableObject
{
    [SerializeField] private List<InventoryItemData> inventoryItems;
    [field: SerializeField] public int Size { get; private set; } = 10;

    public void Initialize()
    {
        inventoryItems = new List<InventoryItemData>();
        for (int i = 0; i < Size; i++)
        {
            inventoryItems.Add(InventoryItemData.GetEmptyItem());
        }

    }

    public void AddItem(ItemSO item, int quantity)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].IsEmpty)
            {
                inventoryItems[i] = new InventoryItemData
                {
                    item = item,
                    quantity = quantity
                };
            }
        }
    }

    public Dictionary<int, InventoryItemData> GetCurrentInventoryState()
    {
        Dictionary<int, InventoryItemData> returnValue =new Dictionary<int, InventoryItemData>();
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].IsEmpty) continue;
            returnValue[i] = inventoryItems[i];
        }
        return returnValue;
    }
    
}

[Serializable]
public struct InventoryItemData
{
    public int quantity;
    public ItemSO item;
    public bool IsEmpty => item == null;

    public InventoryItemData ChangeQuantity(int newQuantity)
    {
        return new InventoryItemData
        {
            item = this.item,
            quantity = newQuantity,
        };

    }

    public static InventoryItemData GetEmptyItem() => new InventoryItemData
    {
        item = null,
        quantity = 0,
    };
}
