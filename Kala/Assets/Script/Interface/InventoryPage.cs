using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryPage : MonoBehaviour
{
    [SerializeField] private InventoryItem itemPrefab;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private InventoryDescription itemDescription;
    [SerializeField] private MouseFollower mouseFollower;

    List<InventoryItem> items = new List<InventoryItem>();

    private int currentlyDraggedItemIndex = -1;

    public event Action<int>  OnDescriptionRequested, OnItemActionRequested, OnStartDragging;
    public event Action<int, int> OnSwapItems;
    private void Awake()
    {
        Hide();
        mouseFollower.Toggle(false);
        itemDescription.ResetDescription();
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

    private void HandleItemSelection(InventoryItem inventoryitemUI)
    {
        int index =  items.IndexOf(inventoryitemUI);
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
        if (index == -1) 
        {
            return;
        }
        OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
    }

    public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity)
    {
        if (items.Count > itemIndex)
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
        mouseFollower.Toggle(true);
        mouseFollower.SetData(sprite, quantity);
    }
    private void HandleShowItemActions(InventoryItem inventoryitemUI)
    {
        
    }

    public void Show()
    {
        gameObject.SetActive(true);
        ResetSelection();
    }

    private void ResetSelection()
    {
        itemDescription.ResetDescription();
        DeselectAllItems();
    }

    private void DeselectAllItems()
    {
        foreach (InventoryItem item in items)
        {
            item.Deselect();
        }
    }

    private void ResetDraggedItem()
    {
        currentlyDraggedItemIndex = -1;
        mouseFollower.Toggle(false);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        ResetDraggedItem();
    }
}
