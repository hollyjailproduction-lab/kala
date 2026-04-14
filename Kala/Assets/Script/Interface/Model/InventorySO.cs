using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Inventory;

namespace  Inventory.Model
{
    [CreateAssetMenu]
    public class InventorySO : ScriptableObject
    {
        [SerializeField] private List<InventoryItemData> inventoryItems;
        [field: SerializeField] public int Size { get; private set; } = 10;

        public event Action<Dictionary<int, InventoryItemData>> OnInventoryUpdated;
        public void Initialize()
        {
            inventoryItems = new List<InventoryItemData>();
            for (int i = 0; i < Size; i++)
            {
                inventoryItems.Add(InventoryItemData.GetEmptyItem());
            }

        }

        public int AddItem(ItemSO item, int quantity)
        {
            if(item.IsStackable == false)
            {
                for (int i = 0; i < inventoryItems.Count; i++)
                {
                    while (quantity > 0 && IsInventoryFull() == false)
                    {
                        quantity -= AddItemToFirstFreeSlot(item, 1);
                    }
                    InformAboutChange();
                    return quantity;
                }
            }
            quantity = AddStackableItem(item, quantity);
            InformAboutChange();
            return quantity;
        }

        private int AddItemToFirstFreeSlot(ItemSO item, int v)
        {
            InventoryItemData newItem = new InventoryItemData
            {
                item = item,
                quantity = v
            };

            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                {
                    inventoryItems[i] = newItem;
                    return 1;
                }
            }
            return 0;
        }
        private bool IsInventoryFull() => inventoryItems.Where(item => item.IsEmpty).Any() == false; 
        
        private int  AddStackableItem(ItemSO item,int quantity)
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                continue;
                if (inventoryItems[i].item.ID ==  item.ID)
                {
                    int amountPossibleToTake = inventoryItems[i].item.MaxStackSize - inventoryItems[i].quantity;

                    if (quantity > amountPossibleToTake)
                    {
                        inventoryItems[i] = inventoryItems[i].ChangeQuantity(inventoryItems[i].item.MaxStackSize);
                        quantity -= amountPossibleToTake;
                    }
                    else
                    {
                        inventoryItems[i] = inventoryItems[i].ChangeQuantity(inventoryItems[i].quantity + quantity);
                        InformAboutChange();
                        return 0;
                    }
                }
            }
            while(quantity > 0 && IsInventoryFull() == false)
            {
                int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
                quantity -= newQuantity;
                AddItemToFirstFreeSlot(item, newQuantity);
            }
            return quantity;

        }

        public void AddItem(InventoryItemData item)
        {
            AddItem(item.item, item.quantity);
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

        public InventoryItemData GetItemAt(int itemIndex)
        {
            return inventoryItems[itemIndex];
        }

        public void SwapItems(int itemIndex_1, int itemIndex_2)
        {
            InventoryItemData temp = inventoryItems[itemIndex_1];
            inventoryItems[itemIndex_1] = inventoryItems[itemIndex_2];
            inventoryItems[itemIndex_2] = temp;
            InformAboutChange();
        }

        private void InformAboutChange()
        {
            OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
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

}

