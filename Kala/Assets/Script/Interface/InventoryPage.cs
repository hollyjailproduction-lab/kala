using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Inventory.Model; 

namespace Inventory.UI
{
    public class InventoryPage : MonoBehaviour
    {
        [SerializeField] private InventoryItem itemPrefab;
        [SerializeField] private RectTransform contentPanel;
        [SerializeField] private InventoryDescription itemDescription;
        [SerializeField] private MouseFollower mouseFollower;

        List<InventoryItem> items = new List<InventoryItem>();
        private int currentlyDraggedItemIndex = -1;

        public event Action<int> OnDescriptionRequested, OnItemActionRequested, OnStartDragging;
        public event Action<int, int> OnSwapItems;

        private void Awake()
        {
            //gameObject.SetActive(false);
            if (mouseFollower != null) mouseFollower.Toggle(false);
            if (itemDescription != null) itemDescription.ResetDescription();
        }

        public void initializeInventoryUI(int inventorysize)
        {
            for (int i = 0; i < inventorysize; i++)
            {
                InventoryItem item = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                item.transform.SetParent(contentPanel);
                items.Add(item);
                item.OnItemClicked += HandleItemSelection;
                item.OnItemDroppedOn += HandleSwap;
                item.OnItemBeginDrag += HandleBeginDrag;
                item.OnItemEndDrag += HandleEndDrag;
                item.OnRightMouseBtnClick += HandleShowItemActions;
            }
        }

        internal void ResetAllItems()
        {
            foreach (var item in items)
            {
                item.ResetData();
                item.Deselect();
            }
        }

        internal void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
        {
            if (itemDescription != null)
                itemDescription.SetDescription(itemImage, name, description);
            DeselectAllItems();
            if (items.Count > itemIndex && items[itemIndex] != null)
                items[itemIndex].Select();
        }

        private void HandleItemSelection(InventoryItem inventoryitemUI)
        {
            int index = items.IndexOf(inventoryitemUI);
            if (index == -1) return;
            OnDescriptionRequested?.Invoke(index);
        }

        private void HandleEndDrag(InventoryItem inventoryitemUI)
        {
            ResetDraggedItem();
        }

        private void HandleSwap(InventoryItem inventoryitemUI)
        {
            int index = items.IndexOf(inventoryitemUI);
            if (index == -1) return;
            OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
            HandleItemSelection(inventoryitemUI);
        }

        public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity)
        {
            if (items.Count > itemIndex && items[itemIndex] != null)
            {
                items[itemIndex].SetData(itemImage, itemQuantity);
            }
        }

        private void HandleBeginDrag(InventoryItem inventoryitemUI)
        {
            
            int index = items.IndexOf(inventoryitemUI);
            if (index == -1) return;
            currentlyDraggedItemIndex = index;
            HandleItemSelection(inventoryitemUI);
            OnStartDragging?.Invoke(index);
        }

        public void CreateDraggedItem(Sprite sprite, int quantity)
        {
            if (mouseFollower != null)
            {
                mouseFollower.Toggle(true);
                mouseFollower.SetData(sprite, quantity);
            }
        }

        private void HandleShowItemActions(InventoryItem inventoryitemUI) { }

        public void Show()
        {
            gameObject.SetActive(true);
            ResetSelection();
        }

        public void ResetSelection()
        {
            if (itemDescription != null) itemDescription.ResetDescription();
            DeselectAllItems();
        }

        private void DeselectAllItems()
        {
            foreach (InventoryItem item in items)
            {
                if (item != null) item.Deselect();
            }
        }

        private void ResetDraggedItem()
        {
            currentlyDraggedItemIndex = -1;
            if (mouseFollower != null) mouseFollower.Toggle(false);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            ResetDraggedItem();
        }
    }
}