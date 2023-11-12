using Inventory.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UnityEditor.Progress;
using Unity.VisualScripting;
using System.Diagnostics;
using Inventory;
using UnityEngine.InputSystem;
using static Pathfinding.RaycastModifier;

public class CraftSysteme : MonoBehaviour
{
    private InputGame _input;
    public string SelectedItem;

    [Header("Gestion des inventaires")]
    [SerializeField] private InventoryController _controller;
    [SerializeField] private InventorySO mainInventory;
    [SerializeField] private InventorySO toolbarInventory;

    [Header("Gestion de l'affichage des items craftables")]
    [SerializeField] private Transform itemListPage;
    [SerializeField] private GameObject craftItemPrefab;
    public List<ItemCraft> listItemCraft = new List<ItemCraft>();

    [Header("Gestion de la description des items")]
    public TextMeshProUGUI nomItem;
    public TextMeshProUGUI descriptionItem;
    public Image imageItem;

    [Header("Gestion du tableau des ressources")]
    public Transform contentTable;
    [SerializeField] private GameObject contentItemTablePrefab;

    [Header("Gestion du Quantity Panel")]
    [SerializeField] private UI_CraftQuantityPanel quantityPanel;


    private void Awake()
    {
        _input = new InputGame();
        gameObject.SetActive(false);
    }


    void Start()
    {
        InitializeData();
        StartCoroutine(Wait1FrameBeforeDo());
        _controller =  GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryController>();  
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    IEnumerator Wait1FrameBeforeDo() // Solution dégueulasse pour régler un problème qui fesait que le tableau ne s'affichait pas à la première ouverture du panel craft
    {
        yield return new WaitForEndOfFrame();
        SetData(listItemCraft[0].name);
    }

    void InitializeData()
    {
        // Initialisation de la table avant d'ajouter des éléments
        InitializeTable();

        for (int i = 0; i < itemListPage.childCount; i++)
        {
            Destroy(itemListPage.GetChild(i).gameObject);
        }
        foreach (ItemCraft craftableItem in listItemCraft)
        {
            GameObject item = Instantiate(craftItemPrefab, Vector3.zero, Quaternion.identity);
            item.transform.SetParent(itemListPage);
            item.GetComponent<UI_CraftItem>().SetData(craftableItem.item.lootSprite, craftableItem.name);
        }
        itemListPage.GetChild(1).gameObject.GetComponent<UI_CraftItem>().Select();

    }
    public void SetData(string itemName)
    {
        if (FindIndexWithItemName(itemName) != -1)
        {
            SetTable(itemName);
            ItemCraft craft = listItemCraft[FindIndexWithItemName(itemName)];
            nomItem.text = craft.name;
            descriptionItem.text = craft.description;
            imageItem.sprite = craft.item.lootSprite;
            
        }
    }

    private void InitializeTable()
    {
        for (int i = 0; i < contentTable.childCount; i++)
        {
            Destroy(contentTable.GetChild(i).gameObject);
        }
        for (int i = 0; i<4; i++)
        {
            GameObject tableLine = Instantiate(contentItemTablePrefab, Vector3.zero, Quaternion.identity);
            tableLine.transform.SetParent(contentTable);
        }
    }
    public void SetTable(string itemName)
    {
        if (FindIndexWithItemName(itemName) == -1) return;
        ItemCraft item = listItemCraft[FindIndexWithItemName(itemName)];
        if (item.ItemRequirements.Count <= 4)
        {
            for (int i = 0; i < item.ItemRequirements.Count; i++)
            {
                GameObject line = contentTable.transform.GetChild(i).gameObject;
                TableManager tableManager = line.GetComponent<TableManager>();
                tableManager.quantityRequired.text = item.ItemRequirements[i].quantityRequired.ToString();
                tableManager.requiredItemName.text = item.ItemRequirements[i].itemRequired.name;
                tableManager.totalQuantityRequired.text = item.ItemRequirements[i].quantityRequired.ToString();
                tableManager.quantitPossed.text = FindQuantityPossed(item.ItemRequirements[i].itemRequired).ToString();
                if (item.ItemRequirements[i].quantityRequired > FindQuantityPossed(item.ItemRequirements[i].itemRequired))
                {
                    tableManager.ChangeTextColor(Color.red);
                    quantityPanel.inputField.textComponent.color = Color.red;
                }
            }
        }
    }

    public void UpdateTotalQuantity(int quantity)
    {
        if (FindIndexWithItemName(SelectedItem) != -1)
        {
            ItemCraft item = listItemCraft[FindIndexWithItemName(SelectedItem)];
            bool canBeWhite = true;
            for (int i = 0; i < item.ItemRequirements.Count; i++)
            {
                GameObject line = contentTable.transform.GetChild(i).gameObject;
                TableManager tableManager = line.GetComponent<TableManager>();
                tableManager.totalQuantityRequired.text = (item.ItemRequirements[i].quantityRequired * quantity).ToString();
                if (item.ItemRequirements[i].quantityRequired * quantity > FindQuantityPossed(item.ItemRequirements[i].itemRequired))
                {
                    if (!tableManager.isRed)
                    {
                        tableManager.ChangeTextColor(Color.red);
                        quantityPanel.inputField.textComponent.color = Color.red;
                    }
                    canBeWhite = false;

                }
                else if (item.ItemRequirements[i].quantityRequired * quantity <= FindQuantityPossed(item.ItemRequirements[i].itemRequired) && tableManager.isRed)
                {
                    tableManager.ChangeTextColor(Color.white);
                }
            }

            if (canBeWhite)
            {
                quantityPanel.inputField.textComponent.color = Color.white;
            }

        }
    }

    public void ChangeSelectedItem()
    {
        ItemCraft item = listItemCraft[FindIndexWithItemName(SelectedItem)];
        for (int i = 0; i < item.ItemRequirements.Count; i++)
        {
            GameObject line = contentTable.transform.GetChild(i).gameObject;
            TableManager tableManager = line.GetComponent<TableManager>();
            tableManager.ChangeTextColor(Color.white);

        }
        quantityPanel.inputField.textComponent.color = Color.white;
    }

    public int FindIndexWithItemName(string itemName)
    {
        for(int i = 0; i < listItemCraft.Count; i++)
        {
            if (itemName == listItemCraft[i].name)
            {
                return i;
            }
        }
        return -1;
    }

    private int FindQuantityPossed(LootFortune item)
    {
        int quantity = 0;
        foreach (InventoryItem itemInventory in mainInventory.inventoryItems)
        {
            if (item == itemInventory.item)
            {
                quantity += itemInventory.quantity;
            }
        }
        foreach (InventoryItem itemInventory in toolbarInventory.inventoryItems)
        {
            if (item == itemInventory.item)
            {
                quantity += itemInventory.quantity;
            }
        }
        return quantity;
    }

    public void Craft()
    {
        ItemCraft item = listItemCraft[FindIndexWithItemName(SelectedItem)];
        bool canCraft = true;
        foreach (ItemCraftRequirements craftRequirement in item.ItemRequirements)
        {
            if (craftRequirement.quantityRequired * quantityPanel.quantity > FindQuantityPossed(craftRequirement.itemRequired)) canCraft = false;
        }
        if (canCraft)
        {
                InventoryPickUpSystem inventoryPickUp = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryPickUpSystem>();
                int quantitySelector = GameObject.FindGameObjectWithTag("QuantitySelector").GetComponent<UI_CraftQuantityPanel>().quantity;
                inventoryPickUp.AddItemFromShop(item.item, item.quantity * quantitySelector, mainInventory, toolbarInventory);
                foreach (ItemCraftRequirements requirements in item.ItemRequirements)
                {
                    int reminder = inventoryPickUp.RemoveItemQuantityFromInventory(requirements.itemRequired, requirements.quantityRequired * quantitySelector, mainInventory);
                    if (reminder > 0)
                    {
                        inventoryPickUp.RemoveItemQuantityFromInventory(requirements.itemRequired, reminder, toolbarInventory);
                    }
                }
                SetTable(SelectedItem);
        }

    }

    #region Show/Hide/Escape
    public void Show()
    {
        
        for (int i = 0; i < itemListPage.transform.childCount; i++)
        {
            itemListPage.GetChild(i).gameObject.GetComponent<UI_CraftItem>().Deselect();
        }
        itemListPage.GetChild(0).gameObject.GetComponent<UI_CraftItem>().Select();
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        else if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
           
        GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryController>().InventoryShow();
    }


    private void Hide()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryController>().InventoryShow();
        gameObject.SetActive(false);
    }

    public void Escape(InputAction.CallbackContext context)
    {
        if (context.performed && gameObject.activeSelf)
        {
            Hide();
        }
    }

    #endregion

    #region struct
    [Serializable]
    public struct ItemCraft
    {
        public LootFortune item;
        public string name;
        [TextArea(1,5)]
        public string description;
        public int quantity;
        public List<ItemCraftRequirements> ItemRequirements;
    }
    [Serializable]
    public struct ItemCraftRequirements
    {
        public LootFortune itemRequired;
        public int quantityRequired;
    }
    #endregion
}
