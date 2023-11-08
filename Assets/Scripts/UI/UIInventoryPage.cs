using Inventory.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
/*using static UnityEditor.Progress;*/

namespace Inventory.UI
{
    public class UIInventoryPage : MonoBehaviour
    {
        [SerializeField] private UIInventoryItem itemPrefab;
        [SerializeField] private RectTransform contentPanel;
        public List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();

        [SerializeField] List<LootFortune> listOfObjects = new List<LootFortune>();

        [SerializeField] private MouseFollower mouseFolower;

        private int currentlyDraggedItemIndex = -1;

        public event Action<int> OnItemActionRequested, OnStartDragging, OnItemClicked;
        public event Action<int, int> OnSwapItems;

        [SerializeField]
        private ItemActionPanel actionPanel;
        private void Awake()
        {
            Hide();
            mouseFolower.Toggle(false);

        }


        public void InitializeInventoryUI(int inventorysize)
        {
            for (int i = 0; i < inventorysize; i++)
            {
                UIInventoryItem uiItem =
                    Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                uiItem.transform.SetParent(contentPanel);
                listOfUIItems.Add(uiItem);
                uiItem.OnItemClicked += HandleItemSelection;
                uiItem.OnItemBeginDrag += HandleBeginDrag;
                uiItem.OnItemDroppedOn += HandleSwap;
                uiItem.OnItemEndDrag += HandleEndDrag;
                uiItem.OnRightMouseBtnClick += HandleShowItemActions;
            }
        }
        public void UdpateData(int itemIndex, Sprite itemImage, int itemQuantity)
        {
            if (listOfUIItems.Count > itemIndex)
            {
                listOfUIItems[itemIndex].SetData(itemImage, itemQuantity);
            }
        }

        private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
        {
            
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1)
            {
                return;
            }
            OnItemActionRequested?.Invoke(index);
        }

        private void HandleSwap(UIInventoryItem inventoryItemUI)
        {
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1)
            {
                return;
            }
            OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
            HandleItemSelection(inventoryItemUI);

        }

        private void ResetDraggedItem()
        {
            mouseFolower.Toggle(false);
            currentlyDraggedItemIndex = -1;
        }

        public void ResetSelection()
        {
            DeselectAllItems();
        }
        public void AddAction(string actionName, Action performAction)
        {
            actionPanel.AddButon(actionName, performAction);
        }

        public void ShowItemActions(int itemIndex)
        {
            actionPanel.Toggle(true);
            actionPanel.transform.position = listOfUIItems[itemIndex].transform.position;
        }

        private void DeselectAllItems()
        {
            foreach (UIInventoryItem item in listOfUIItems)
            {
                item.Deselect();
            }
            actionPanel.Toggle(false);
        }

        private void HandleEndDrag(UIInventoryItem inventoryItemUI)
        {
            ResetDraggedItem();
        }

        private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
        {
            /*Debug.Log("Swap5");*/
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1) return;
            currentlyDraggedItemIndex = index;
            HandleItemSelection(inventoryItemUI);
            OnStartDragging?.Invoke(index);


        }

        public void CreateDraggedItem(Sprite sprite, int Quantity)
        {
            mouseFolower.Toggle(true);
            mouseFolower.SetData(sprite, Quantity);
        }

        private void HandleItemSelection(UIInventoryItem inventoryItemUI)
        {
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1) return;
            OnItemClicked?.Invoke(index);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            ResetSelection();

        }


        public void Hide()
        {
            actionPanel.Toggle(false);
            gameObject.SetActive(false);
            ResetDraggedItem();
        }

        internal void UdpateClick(int itemIndex, string description)
        {
            DeselectAllItems();
            listOfUIItems[itemIndex].Select();
            GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryController>().lastSelectedItemWithMouse = itemIndex;
            if (description != null)
            {
                Debug.Log(description);
            }
           
        }

        internal void ResetAllItems()
        {
            foreach (var item in listOfUIItems)
            {
                item.ResetData();
                item.Deselect();
            }
        }
    }
}