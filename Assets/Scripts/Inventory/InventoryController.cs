using Inventory.Model;
using Inventory.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Inventory
{
    public class InventoryController : MonoBehaviour
    {
        // Dictionnaire pour stocker les inventaires actifs
        [HideInInspector] public Dictionary<string, InventorySO> Inventories = new Dictionary<string, InventorySO>();
/*        private Dictionary<string, InventorySO> activeInventories = new Dictionary<string, InventorySO>();*/

        // Dictionnaire pour stocker l'état de chaque inventaire actif
        private Dictionary<string, Dictionary<int, InventoryItem>> inventoryStates = new Dictionary<string, Dictionary<int, InventoryItem>>();
/*
        private string activeInventoryKey = "ToolbarInventory"; // Inventaire actif par défaut*/



        [SerializeField] private UIInventoryPage inventoryUI;
        public UIInventoryPage MainInventaire => inventoryUI;
        [SerializeField] private UIInventoryPage inventoryUIBar;
        [SerializeField] private InputGame _playerInput;

        [SerializeField] private InventorySO mainInventoryData;
        [SerializeField] private InventorySO toolbarInventoryData;
        private string lastOverhauledInventory;
        private int indexOfLastItemDragged;

        [HideInInspector] public bool inventoryUIIsOpen;
        public List<InventoryItem> initialItems = new List<InventoryItem>();
        public List<InventoryItem> initialBarItems = new List<InventoryItem>();

        private InventorySO initializeInventory;
        

        [HideInInspector] public Dictionary<InventorySO, UIInventoryPage> _inventoryUI = new Dictionary<InventorySO, UIInventoryPage>();
        [HideInInspector] public Dictionary<InventorySO, List<InventoryItem>> _initialItems = new Dictionary<InventorySO, List<InventoryItem>>();

        private string lastInventoryRecorded = "ToolBarInventory"; // A utiliser uniquement lorsque je ne peux pas accéder à l'inventaire survolé par ma souris.
        private string lastInventoryClickedOn;
        private int lastItemSelected;
        public int lastSelectedItemWithMouse;

        //Gestion des tags
/*        [SerializeField] private TagManager tagManager;*/
        

        // Gestion du drop
        [SerializeField] private GameObject droppedItemPrefab;

        //Gestion du combo de touche
        private bool shiftCombo;

        // Gestion du chest Inventory Panel
        private bool chestInventoryIsSet;
        public bool ChestInventoryIsSet => chestInventoryIsSet;
        public int chestCount;


        [Header("Gestion des différents panels")]
        public GameObject MainInventoryPanel;
        public GameObject ChestInventoryPanel;
        public GameObject ShopPanel;
        public GameObject RemoveQuantityPanel;
        private void Awake()
        {
            _playerInput = new InputGame();

        }

        private void Start()
        {
            chestInventoryIsSet = false;
            // Ajoutez les inventaires existants
            Inventories.Add("MainInventory", mainInventoryData);
            Inventories.Add("ToolbarInventory", toolbarInventoryData);

/*            // Ajoute l'inventaire actif
            activeInventories.Add("ToolbarInventory", toolbarInventoryData);*/

            // Associe à l'inventaire MainInventory son UI et ses items initiaux
            _inventoryUI.Add(mainInventoryData, inventoryUI);
            _initialItems.Add(mainInventoryData, initialItems);

            // Associe à l'inventaire ToolbarInventory son UI et ses items initiaux
            _inventoryUI.Add(toolbarInventoryData, inventoryUIBar);
            _initialItems.Add(toolbarInventoryData, initialBarItems);

/*            Debug.Log(GetItemsNumber("ToolbarInventory"));*/
            PrepareUI();
            PrepareInventoryData();

            string description = PrepareDescription(Inventories["ToolbarInventory"].GetItemAt(0));
            inventoryUIBar.Show();
            inventoryUIBar.UdpateClick(0, description);

            foreach (var inventoryEntry in Inventories)
            {
                string inventoryKey = inventoryEntry.Key;
                InventorySO inventoryData = inventoryEntry.Value;

                // Associer le tag du nom de l'inventaire à l'UI correspondante
                if (inventoryKey == "MainInventory" || inventoryKey == "ToolbarInventory")
                {
                    /*CreateTag(inventoryKey);*/
                    _inventoryUI[inventoryData].gameObject.GetComponent<TaggedObject>().tagDynamic = inventoryKey;

                }
            } // Je crée un tag pour chaque inventaire.

        }

        public void Update()
        {
            if (MainInventoryPanel.activeSelf || ShopPanel.activeSelf)
            {
                GetComponent<PlayerMovement>()._animator.SetFloat("Speed", 0);
                GetComponent<PlayerMovement>()._rigidbody.velocity = Vector2.zero;
                GetComponent<PlayerMovement>().enabled = false;
            }
            else if (!MainInventoryPanel.activeSelf && !ShopPanel.activeSelf)
            {
                GetComponent<PlayerMovement>().enabled = true;
            }

        }


        /*        private void PrepareInventoryData()
                {
                    foreach(var inventory in Inventories)
                    {
                        inventory.Value.Initialize();
                        initializeInventory = inventory.Value;
                        inventory.Value.OnInventoryUpdated += UpdateInventoryUI;
                        List<InventoryItem> InitialItems = _initialItems[inventory.Value];
                        foreach (InventoryItem item in InitialItems)
                        {
                            if (item.IsEmpty) continue;
                            inventory.Value.AddItem(item);
                        }

                    }
                }*/

        private void PrepareInventoryData()
        {
            foreach (var inventoryEntry in Inventories)
            {
                var inventoryKey = inventoryEntry.Key;
                var inventoryData = inventoryEntry.Value;

/*                Debug.Log(inventoryKey);*/
                inventoryData.Initialize();
                inventoryData.OnInventoryUpdated += (state) => UpdateInventoryUI(inventoryKey, state);
                

                var initialItems = _initialItems[inventoryData];
                foreach (InventoryItem item in initialItems)
                {
                    if (item.IsEmpty) continue;
                    inventoryData.AddItem(item);
                }
            }
        }
        public void PrepareInventoryDataChest(InventorySO inventoryEntry)
        {
            var inventoryKey = inventoryEntry.name;
            var inventoryData = inventoryEntry;

/*            Debug.Log(inventoryKey)*/;
            inventoryData.Initialize();
            inventoryData.OnInventoryUpdated += (state) => UpdateInventoryUI(inventoryKey, state);


            var initialItems = _initialItems[inventoryData];
            foreach (InventoryItem item in initialItems)
            {
                if (item.IsEmpty) continue;
                inventoryData.AddItem(item);
            }
        }

        public void RefreshInventoryUI(InventorySO inventoryData, UIInventoryPage uiInventory)
        {
            uiInventory.ResetAllItems(); // Réinitialise tous les éléments de l'UI de l'inventaire

            foreach (var item in inventoryData.GetCurrentInventoryState())
            {
                int itemIndex = item.Key;
                InventoryItem inventoryItem = item.Value;
                uiInventory.UdpateData(itemIndex, inventoryItem.item.lootSprite, inventoryItem.quantity);
            }
        }

        /*        private void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState)
                {
                    string InventaireSouris = GetActiveInventoryUnderMouseTag();
                    UIInventoryPage UIinventory = _inventoryUI[initializeInventory];
                    UIinventory.ResetAllItems();
                    foreach (var item in inventoryState)
                    {
                        UIinventory.UdpateData(item.Key, item.Value.item.lootSprite, item.Value.quantity);
                    }

                }*/

        private void UpdateInventoryUI(string inventoryKey, Dictionary<int, InventoryItem> inventoryState)
        {
            UIInventoryPage UIinventory = _inventoryUI[Inventories[inventoryKey]];
            UIinventory.ResetAllItems();
            foreach (var item in inventoryState)
            {
                UIinventory.UdpateData(item.Key, item.Value.item.lootSprite, item.Value.quantity);
            }
        }



        private void PrepareUI()
        {
            foreach (var inventory in Inventories)
            {
                UIInventoryPage UIinventory = _inventoryUI[inventory.Value];
       
                UIinventory.InitializeInventoryUI(inventory.Value.Size);
                
                UIinventory.OnItemClicked += HandleItemClick;
                UIinventory.OnStartDragging += HandleDragging;
                UIinventory.OnSwapItems += HandleSwapItems;
                UIinventory.OnItemActionRequested += HandleItemActionRequested;
            }

        }

        public void PrepareUIChest(InventorySO inventorySO, UIInventoryPage inventoryUI)
        {
            var inventoryKey = inventorySO.name;
            var inventoryData = inventorySO;
            UIInventoryPage UIinventory = _inventoryUI[inventorySO];
            if (!chestInventoryIsSet)
            {

                chestInventoryIsSet = true;
                UIinventory.InitializeInventoryUI(inventorySO.Size);
                UIinventory.OnItemClicked += HandleItemClick;
                UIinventory.OnStartDragging += HandleDragging;
                UIinventory.OnSwapItems += HandleSwapItems;
                UIinventory.OnItemActionRequested += HandleItemActionRequested;
            }

        }

        private void HandleItemClick(int itemIndex)
        {

            string InventaireSouris = GetActiveInventoryUnderMouseTag();
            lastInventoryClickedOn = InventaireSouris;
            InventoryItem inventoryItem = Inventories[InventaireSouris].GetItemAt(itemIndex);
            UIInventoryPage UIinventory = _inventoryUI[Inventories[InventaireSouris]]; // j'obtiens l'UI relié à mon inventaire
            if (inventoryItem.IsEmpty && inventoryUI.isActiveAndEnabled)
            {
                foreach(var inventory in Inventories)
                {
                    _inventoryUI[inventory.Value].ResetSelection();
                }
                return;
            }
            if (!inventoryItem.IsEmpty)
            {
                LootFortune item = inventoryItem.item;
                string description = PrepareDescription(inventoryItem);
                if (InventaireSouris == "ToolbarInventory") lastItemSelected = itemIndex;
                UIinventory.UdpateClick(itemIndex, description);
                foreach (var UI in _inventoryUI)
                {
                    if (UI.Value != UIinventory)
                    {
                        UI.Value.ResetSelection();
                    }
                }
                if (shiftCombo)
                {
                    if (InventaireSouris == "MainInventory" && inventoryUI.isActiveAndEnabled && !Inventories["ToolbarInventory"].IsInventoryFull() && !ChestInventoryPanel.activeSelf)
                    {
                        foreach (var itm in Inventories["ToolbarInventory"].inventoryItems.Select((value, i) => new { i, value }))
                        {
                            if (itm.value.item == item && itm.value.item.IsStackable)
                            {
                                AddItemsOfSameType(itemIndex, itm.i, Inventories[InventaireSouris], Inventories["ToolbarInventory"]);
                                return;
                                
                            }
                        }
                        Inventories["ToolbarInventory"].AddItemToFirstFreeSlot(item, inventoryItem.quantity);
                        Inventories["ToolbarInventory"].InformAboutChange();
                        Inventories["MainInventory"].RemoveItem(itemIndex, inventoryItem.quantity);

                    }
                    if (InventaireSouris == "MainInventory" && inventoryUI.isActiveAndEnabled && !Inventories[ChestInventoryPanel.GetComponent<TaggedObject>().tagDynamic].IsInventoryFull() && ChestInventoryPanel.activeSelf)
                    {
                        foreach (var itm in Inventories[ChestInventoryPanel.GetComponent<TaggedObject>().tagDynamic].inventoryItems.Select((value, i) => new { i, value }))
                        {
                            if (itm.value.item == item && itm.value.item.IsStackable)
                            {
                                AddItemsOfSameType(itemIndex, itm.i, Inventories[InventaireSouris], Inventories[ChestInventoryPanel.GetComponent<TaggedObject>().tagDynamic]);
                                return;

                            }
                        }
                        Inventories[ChestInventoryPanel.GetComponent<TaggedObject>().tagDynamic].AddItemToFirstFreeSlot(item, inventoryItem.quantity);
                        Inventories[ChestInventoryPanel.GetComponent<TaggedObject>().tagDynamic].InformAboutChange();
                        Inventories["MainInventory"].RemoveItem(itemIndex, inventoryItem.quantity);
                    }
                    else if (InventaireSouris != "MainInventory" && inventoryUI.isActiveAndEnabled && !Inventories["MainInventory"].IsInventoryFull())
                    {
                        foreach (var itm in Inventories["MainInventory"].inventoryItems.Select((value, i) => new { i, value }))
                        {
                            if (itm.value.item == item && itm.value.item.IsStackable)
                            {
                                AddItemsOfSameType(itemIndex, itm.i, Inventories[InventaireSouris], Inventories["MainInventory"]);
                                return;

                            }
                        }
                        Inventories["MainInventory"].AddItemToFirstFreeSlot(item, inventoryItem.quantity);
                        Inventories["MainInventory"].InformAboutChange();
                        Inventories[InventaireSouris].RemoveItem(itemIndex, inventoryItem.quantity);
                    }
                }
            }

        }


        private string PrepareDescription(InventoryItem inventoryItem)
        {
            StringBuilder sb = new StringBuilder();
            if (inventoryItem.item != null)
            {
                sb.Append(inventoryItem.item.lootName);
                sb.AppendLine();
                for (int i = 0; i < inventoryItem.itemState.Count; i++)
                {
                    sb.Append($"{inventoryItem.itemState[i].itemParameter.parameterName} : " +
                        $"{inventoryItem.itemState[i].value} /" +
                        $" {inventoryItem.item.DefaultParametersList[i].value}");
                    sb.AppendLine();
                }
                return sb.ToString();
            }
            return null;
        }

        private void HandleItemActionRequested(int itemIndex)
        {
            
            string InventaireSouris = GetActiveInventoryUnderMouseTag();
            InventoryItem inventoryItem = Inventories[lastInventoryRecorded].GetItemAt(itemIndex);
            UIInventoryPage UIinventory = _inventoryUI[Inventories[lastInventoryRecorded]]; // j'obtiens l'UI relié à mon inventaire
            if (inventoryItem.IsEmpty)
                return;

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                foreach (var inventory in Inventories)
                {
                    _inventoryUI[inventory.Value].ResetSelection();
                }
                UIinventory.listOfUIItems[itemIndex].Select();
                lastSelectedItemWithMouse = itemIndex;
                UIinventory.ShowItemActions(itemIndex);
                UIinventory.AddAction(itemAction.ActionName, () => PerformAction(itemIndex));
            }

            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (itemAction == null && destroyableItem != null)
            {
                foreach (var inventory in Inventories)
                {
                    _inventoryUI[inventory.Value].ResetSelection();
                }
                UIinventory.listOfUIItems[itemIndex].Select();
                lastSelectedItemWithMouse = itemIndex;
                UIinventory.ShowItemActions(itemIndex);
                UIinventory.AddAction("Drop", () => DropItem(itemIndex, inventoryItem.quantity));
            }
            if (itemAction != null && destroyableItem != null)
            {
                UIinventory.AddAction("Drop", () => DropItem(itemIndex, inventoryItem.quantity));
            }

        }

        private void DropItem(int itemIndex, int quantity)
        {
            /*string InventaireSouris = GetActiveInventoryUnderMouseTag();*/
            UIInventoryPage UIinventory = _inventoryUI[Inventories[lastInventoryRecorded]]; // j'obtiens l'UI relié à mon inventaire
            for (int i = 0; i<quantity; i++)
            {
                GameObject lootGameObject = Instantiate(droppedItemPrefab, new Vector2(transform.position.x + 1f, 
                    transform.position.y + 1f), Quaternion.identity);
                lootGameObject.GetComponent<Item>().InventoryItem = Inventories[lastInventoryRecorded].GetItemAt(itemIndex).item;
                lootGameObject.GetComponent<Loot>().hasNotBeDropped = false;
                lootGameObject.GetComponent<SpriteRenderer>().sprite = Inventories[lastInventoryRecorded].GetItemAt(itemIndex).item.lootSprite;
            }
            Inventories[lastInventoryRecorded].RemoveItem(itemIndex, quantity);
            UIinventory.ResetSelection();   
        }

        public void PerformAction(int itemIndex)
        {
/*            string InventaireSouris = GetActiveInventoryUnderMouseTag();*/
            InventoryItem inventoryItem = Inventories[lastInventoryRecorded].GetItemAt(itemIndex);
            UIInventoryPage UIinventory = _inventoryUI[Inventories[lastInventoryRecorded]]; // j'obtiens l'UI relié à mon inventaire

            if (inventoryItem.IsEmpty) return;

            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                Inventories[lastInventoryRecorded].RemoveItem(itemIndex, 1);
                UIinventory.ResetSelection();
            }

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                itemAction.PerformAction(gameObject, inventoryItem.itemState);
                if(inventoryItem.IsEmpty)
                {
                    UIinventory.ResetSelection();
                }
            }
        }

        private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
        {
            string InventaireSouris = GetActiveInventoryUnderMouseTag();
            if (InventaireSouris == lastOverhauledInventory)
            {
                if (AreItemsOfSameTypeAndAreStackable(itemIndex_1, Inventories[lastOverhauledInventory], itemIndex_2, Inventories[InventaireSouris]))
                {
                    AddItemsOfSameType(itemIndex_1, itemIndex_2, Inventories[lastOverhauledInventory], Inventories[lastOverhauledInventory]);
                }
                else
                {
                    Inventories[InventaireSouris].SwapItems(itemIndex_1, itemIndex_2);
                }
            }
            else
            {
/*                Debug.Log("itemIndex1 " + indexOfLastItemDragged + ", itemIndex2 " + itemIndex_2);
                Debug.Log("Swap Between inventories");
                Debug.Log("Inventaire 1 " + lastOverhauledInventory + ", Inventaire 2 " + InventaireSouris);*/
                if (AreItemsOfSameTypeAndAreStackable(indexOfLastItemDragged, Inventories[lastOverhauledInventory], itemIndex_2, Inventories[InventaireSouris]))
                {
                    AddItemsOfSameType(indexOfLastItemDragged, itemIndex_2, Inventories[lastOverhauledInventory], Inventories[InventaireSouris]);
                }
                else
                {
/*                    if (InventaireSouris == null)
                    {
                        DropItem(itemIndex_1, Inventories[InventaireSouris].GetItemAt(itemIndex_1).quantity);

                    }*/
/*                    else
                    {*/
                    List<InventoryItem> Inventories1Item = Inventories[lastOverhauledInventory].inventoryItems;
                    List<InventoryItem> Inventories2Item = Inventories[InventaireSouris].inventoryItems;
                    Debug.Log(InventaireSouris + " and " + lastOverhauledInventory);
                    Inventories[InventaireSouris].SwapItemsBetweenInventories(indexOfLastItemDragged, Inventories1Item,
                        itemIndex_2, Inventories2Item);
                    Inventories[lastOverhauledInventory].InformAboutChange();
                    //}
                }



            }
            /*inventoryData.SwapItems(itemIndex_1, itemIndex_2);*/
        }
        private void AddItemsOfSameType(int itemIndex_arrive, int itemIndex_depart, InventorySO inventory2, InventorySO inventory1)
        {
            if (inventory1.GetItemAt(itemIndex_depart).item == inventory2.GetItemAt(itemIndex_arrive).item)
            {
                /*Debug.Log("Les deux articles ont le même type, ajoutons-les ensemble.");*/
                // Les deux articles ont le même type, ajoutons-les ensemble.
                int totalQuantity = inventory1.inventoryItems[itemIndex_depart].quantity + inventory2.inventoryItems[itemIndex_arrive].quantity;
                if (totalQuantity <= inventory1.inventoryItems[itemIndex_depart].item.MaxStackSize)
                {
                    /*Debug.Log(totalQuantity);*/
                    // La quantité totale reste dans la limite de la pile.
                    inventory1.inventoryItems[itemIndex_depart] = inventory1.inventoryItems[itemIndex_depart].ChangeQuantity(totalQuantity);
                    inventory2.inventoryItems[itemIndex_arrive] = InventoryItem.GetEmptyItem();// Supprime l'autre article.
                    inventory1.InformAboutChange();
                    inventory2.InformAboutChange();


                }
                else
                {
                    // La quantité totale dépasse la limite de la pile.
                    inventory1.inventoryItems[itemIndex_depart] = inventory1.inventoryItems[itemIndex_depart].ChangeQuantity(inventory1.GetItemAt(itemIndex_depart).item.MaxStackSize);
                    inventory2.inventoryItems[itemIndex_arrive] = inventory2.inventoryItems[itemIndex_arrive].ChangeQuantity(totalQuantity - inventory2.GetItemAt(itemIndex_depart).item.MaxStackSize);
                    inventory1.InformAboutChange();
                    inventory2.InformAboutChange();
                }
            }
        }
        public bool AreItemsOfSameTypeAndAreStackable(int itemIndex1, InventorySO inventory1, int itemIndex2, InventorySO inventory2)
        {
            InventoryItem item1 = inventory1.inventoryItems[itemIndex1];
            InventoryItem item2 = inventory2.inventoryItems[itemIndex2];
            if (item1.IsEmpty || item2.IsEmpty) return false;
            if (item1.item.IsStackable && !item1.IsEmpty)
            {
                return item1.item == item2.item;
            }
            return item1.item.IsStackable;
        }

        private void HandleDragging(int itemIndex)
        {
            if (!inventoryUI.isActiveAndEnabled) return;
            string InventaireSouris = GetActiveInventoryUnderMouseTag();
            lastOverhauledInventory = InventaireSouris;
            InventoryItem inventoryItem = Inventories[InventaireSouris].GetItemAt(itemIndex);
            indexOfLastItemDragged = itemIndex;
            UIInventoryPage UIinventory = _inventoryUI[Inventories[InventaireSouris]]; // j'obtiens l'UI relié à mon inventaire
            if (inventoryItem.IsEmpty) return;
            UIinventory.CreateDraggedItem(inventoryItem.item.lootSprite, inventoryItem.quantity);

/*            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
              if (inventoryItem.IsEmpty)
                  return;
              inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);*/
        }
        public void Shift(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                shiftCombo = true;
            }
            if (context.canceled)
            {
                shiftCombo = false;
            }
        }
        public void RemoveQuantityItem(InventorySO inventory, int itemIndex, int removedQuantity)
        {
            LootFortune item = inventory.inventoryItems[itemIndex].item;
            int newQuantity = inventory.inventoryItems[itemIndex].quantity - removedQuantity;
            if (!inventory.IsInventoryFull())
            {
                inventory.inventoryItems[itemIndex] = inventory.inventoryItems[itemIndex].ChangeQuantity(newQuantity);
                inventory.AddItemToFirstFreeSlot(item, removedQuantity);
                inventory.InformAboutChange();
            }
            else if (inventory != Inventories["MainInventory"] && !Inventories["MainInventory"].IsInventoryFull())
            {
                inventory.inventoryItems[itemIndex] = inventory.inventoryItems[itemIndex].ChangeQuantity(newQuantity);
                Inventories["MainInventory"].AddItemToFirstFreeSlot(item, removedQuantity);
                inventory.InformAboutChange();
                Inventories["MainInventory"].InformAboutChange();
            }
            else if (inventory != Inventories["ToolbarInventory"] && !Inventories["ToolbarInventory"].IsInventoryFull())
            {
                inventory.inventoryItems[itemIndex] = inventory.inventoryItems[itemIndex].ChangeQuantity(newQuantity);
                Inventories["ToolbarInventory"].AddItemToFirstFreeSlot(item, removedQuantity);
                inventory.InformAboutChange();
                Inventories["ToolbarInventory"].InformAboutChange();
            }
            else if (ChestInventoryPanel.activeSelf && inventory != Inventories[lastInventoryClickedOn] && !Inventories[lastInventoryClickedOn].IsInventoryFull())
            {
                inventory.inventoryItems[itemIndex] = inventory.inventoryItems[itemIndex].ChangeQuantity(newQuantity);
                Inventories[lastInventoryClickedOn].AddItemToFirstFreeSlot(item, removedQuantity);
                inventory.InformAboutChange();
                Inventories[lastInventoryClickedOn].InformAboutChange();
            }

        }
        public void Split(InputAction.CallbackContext context)
        {
            if (context.performed && MainInventoryPanel.activeSelf && Inventories[lastInventoryClickedOn].inventoryItems[lastSelectedItemWithMouse].item.IsStackable && Inventories[lastInventoryClickedOn].inventoryItems[lastSelectedItemWithMouse].quantity > 2 && !shiftCombo)
            {
                if (ChestInventoryPanel.activeSelf && !Inventories[lastInventoryClickedOn].IsInventoryFull())
                {
                    RemoveQuantityPanel.transform.GetChild(0).GetComponent<UI_QuantityPanel>().item = Inventories[lastInventoryClickedOn].inventoryItems[lastSelectedItemWithMouse];
                    RemoveQuantityPanel.transform.GetChild(0).GetComponent<UI_QuantityPanel>().inventoryItemSplit = Inventories[lastInventoryClickedOn];
                    RemoveQuantityPanel.transform.GetChild(0).GetComponent<UI_QuantityPanel>().itemIndex = lastSelectedItemWithMouse;
                    RemoveQuantityPanel.transform.GetChild(0).GetComponent<UI_QuantityPanel>().slider.maxValue = Inventories[lastInventoryClickedOn].inventoryItems[lastSelectedItemWithMouse].quantity;
                    RemoveQuantityPanel.SetActive(true);
                }
                else if (Inventories[lastInventoryClickedOn] != Inventories["MainInventory"] && !Inventories["MainInventory"].IsInventoryFull())
                {
                    RemoveQuantityPanel.transform.GetChild(0).GetComponent<UI_QuantityPanel>().item = Inventories[lastInventoryClickedOn].inventoryItems[lastSelectedItemWithMouse];
                    RemoveQuantityPanel.transform.GetChild(0).GetComponent<UI_QuantityPanel>().inventoryItemSplit = Inventories[lastInventoryClickedOn];
                    RemoveQuantityPanel.transform.GetChild(0).GetComponent<UI_QuantityPanel>().itemIndex = lastSelectedItemWithMouse;
                    RemoveQuantityPanel.transform.GetChild(0).GetComponent<UI_QuantityPanel>().slider.maxValue = Inventories[lastInventoryClickedOn].inventoryItems[lastSelectedItemWithMouse].quantity;
                    RemoveQuantityPanel.SetActive(true);
                }
                else if (Inventories[lastInventoryClickedOn] != Inventories["ToolbarInventory"] && !Inventories["ToolbarInventory"].IsInventoryFull())
                {
                    RemoveQuantityPanel.transform.GetChild(0).GetComponent<UI_QuantityPanel>().item = Inventories[lastInventoryClickedOn].inventoryItems[lastSelectedItemWithMouse];
                    RemoveQuantityPanel.transform.GetChild(0).GetComponent<UI_QuantityPanel>().inventoryItemSplit = Inventories[lastInventoryClickedOn];
                    RemoveQuantityPanel.transform.GetChild(0).GetComponent<UI_QuantityPanel>().itemIndex = lastSelectedItemWithMouse;
                    RemoveQuantityPanel.transform.GetChild(0).GetComponent<UI_QuantityPanel>().slider.maxValue = Inventories[lastInventoryClickedOn].inventoryItems[lastSelectedItemWithMouse].quantity;
                    RemoveQuantityPanel.SetActive(true);
                }

            }
            else if (context.performed && MainInventoryPanel.activeSelf && Inventories[lastInventoryClickedOn].inventoryItems[lastSelectedItemWithMouse].item.IsStackable && Inventories[lastInventoryClickedOn].inventoryItems[lastSelectedItemWithMouse].quantity == 2)
            {
                RemoveQuantityItem(Inventories[lastInventoryClickedOn], lastSelectedItemWithMouse, 1);
            }
            else if (context.performed && MainInventoryPanel.activeSelf && Inventories[lastInventoryClickedOn].inventoryItems[lastSelectedItemWithMouse].item.IsStackable && Inventories[lastInventoryClickedOn].inventoryItems[lastSelectedItemWithMouse].quantity > 2 && shiftCombo)
            {
                if (Inventories[lastInventoryClickedOn].inventoryItems[lastSelectedItemWithMouse].quantity % 2 == 0)
                {
                    RemoveQuantityItem(Inventories[lastInventoryClickedOn], lastSelectedItemWithMouse, Inventories[lastInventoryClickedOn].inventoryItems[lastSelectedItemWithMouse].quantity/2);
                }
                else
                {
                    int quantity1 = Inventories[lastInventoryClickedOn].inventoryItems[lastSelectedItemWithMouse].quantity / 2;
                    RemoveQuantityItem(Inventories[lastInventoryClickedOn], lastSelectedItemWithMouse, Inventories[lastInventoryClickedOn].inventoryItems[lastSelectedItemWithMouse].quantity - quantity1);

                }
            }


        }
        public void inventory(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InventoryShow();
            }
            else if (context.canceled)
            {
                /*Debug.Log("Je relache");*/
            }
        }

        public void InventoryShow()
        {
            if (!ShopPanel.activeSelf)
            {
                foreach (var inventory in Inventories)
                {
                    _inventoryUI[inventory.Value].ResetSelection();
                }
                /*Debug.Log("J'appuie");*/
                if (!inventoryUI.isActiveAndEnabled)
                {
                    /*                    activeInventories.Add("MainInventory", mainInventoryData);*/
                    inventoryUI.Show();
                    inventoryUIIsOpen = true;
                    foreach (var item in mainInventoryData.GetCurrentInventoryState())
                    {
                        inventoryUI.UdpateData(item.Key, item.Value.item.lootSprite, item.Value.quantity);
                    }
                }
                else if (!ChestInventoryPanel.activeSelf && inventoryUI.isActiveAndEnabled)
                {
                    /*                    activeInventories.Remove("MainInventory");*/
                    inventoryUI.Hide();
                    if (!Inventories["ToolbarInventory"].GetItemAt(lastItemSelected).IsEmpty)
                    {
                        string description = PrepareDescription(Inventories["ToolbarInventory"].GetItemAt(lastItemSelected));
                        inventoryUIBar.UdpateClick(lastItemSelected, description);
                    }
                    else
                    {
                        string description = PrepareDescription(Inventories["ToolbarInventory"].GetItemAt(0));
                        inventoryUIBar.UdpateClick(0, description);
                    }

                    inventoryUIIsOpen = false;
                    /*lastItemSelected = 0;*/
                }

            }
        }

/*        public void Left(InputAction.CallbackContext context)
        {
            shiftCombo = context.performed;
        }*/

        public void HideInventory(InputAction.CallbackContext context)
        {
            if (context.performed && inventoryUI.isActiveAndEnabled && !RemoveQuantityPanel.activeSelf)
            {
                inventoryUI.Hide();
            }
        }

        public void OnScroll(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (!inventoryUI.isActiveAndEnabled)
                {
                    if (context.ReadValue<float>() > 0f)
                    {
                        /*Debug.Log("Scroll Up");*/
                        lastItemSelected += 1;
                    }
                    if (context.ReadValue<float>() < 0f)
                    {
                        /*Debug.Log("Scroll Down");*/
                        lastItemSelected -= 1;
                    }
                    if (lastItemSelected < 0)
                    {
                        lastItemSelected = Inventories["ToolbarInventory"].inventoryItems.Count - 1;
                    }
                    else if (lastItemSelected > Inventories["ToolbarInventory"].inventoryItems.Count - 1)
                    {
                        lastItemSelected = 0;
                    }
                    string description = PrepareDescription(Inventories["ToolbarInventory"].GetItemAt(lastItemSelected));
                    inventoryUIBar.UdpateClick(lastItemSelected, description);

                }

            }
        }
        public string GetActiveInventoryUnderMouseTag() // Renvoie le nom de l'inventaire au dessus duquel je passe ma souris
        {
            Vector2 mousePosition = Input.mousePosition;
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            pointerData.position = mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            foreach (var result in results)
            {
                foreach (var inventory in Inventories)
                {
                    if (result.gameObject.GetComponent<TaggedObject>() != null && result.gameObject.GetComponent<TaggedObject>().tagDynamic == inventory.Key)
                    {
                        // Vérifiez si le GameObject possède un composant Image avec Raycast Target activé
                        Image inventoryImage = result.gameObject.GetComponent<Image>();

                        if (inventoryImage != null && inventoryImage.raycastTarget)
                        {
                            if (result.gameObject.GetComponent<UIInventoryPage>() != null)
                            {
                                lastInventoryRecorded = result.gameObject.GetComponent<TaggedObject>().tagDynamic;
                                return result.gameObject.GetComponent<TaggedObject>().tagDynamic;
                            }
                            GameObject parent = result.gameObject.transform.parent.gameObject;
                            while (parent != null && parent.GetComponent<UIInventoryPage>() == null)
                            {
                                parent = parent.transform.parent.gameObject;
                            }
                            if (parent != null) lastInventoryRecorded = parent.gameObject.GetComponent<TaggedObject>().tagDynamic;
                            return parent.gameObject.GetComponent<TaggedObject>().tagDynamic; // La souris est au-dessus de cet inventaire
                        }
                    }
                }          
            }

            return null; // Aucun inventaire actif sous la souris
        }


        private int GetItemsNumber(string nameInventory)
        {
            int numberItems = 0;
            for (int i = 0; i < Inventories[nameInventory].inventoryItems.Count - 1; i++)
            {
                if (!Inventories[nameInventory].inventoryItems[i].IsEmpty)
                {
                    numberItems++;
                }
            }
            return numberItems;
        }
        private void OnEnable()
        {
            _playerInput.Player.Inventory.Enable();
            _playerInput.Player.MouseScrollY.Enable();
            _playerInput.Player.Escape.Enable();
            _playerInput.Player.Sprint.Enable();
        }

        private void OnDisable()
        {
            _playerInput.Player.Inventory.Disable();
            _playerInput.Player.MouseScrollY.Disable();
            _playerInput.Player.Escape.Disable();
            _playerInput.Player.Sprint.Disable();

        }

/*        public void CreateTag(string tagName)
        {
            tagManager.CreateTag(tagName);
        }*/

        /*        public void CreateTag(string tagName)
                {
                    SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                    SerializedProperty tagsProp = tagManager.FindProperty("tags");

                    // Vérifiez si le tag existe déjà
                    bool tagExists = false;
                    for (int i = 0; i < tagsProp.arraySize; i++)
                    {
                        SerializedProperty tag = tagsProp.GetArrayElementAtIndex(i);
                        if (tag.stringValue == tagName)
                        {
                            tagExists = true;
                            break;
                        }
                    }

                    if (!tagExists)
                    {
                        // Ajoutez un nouveau tag
                        int newIndex = tagsProp.arraySize;
                        tagsProp.InsertArrayElementAtIndex(newIndex);
                        SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(newIndex);
                        newTag.stringValue = tagName;

                        tagManager.ApplyModifiedProperties();
                    }
                }
        */
    }
}