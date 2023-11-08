using Inventory;
using Inventory.Model;
using Inventory.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChestInventoryController : MonoBehaviour
{
    [SerializeField] private InventorySO inventoryModel;
    [SerializeField] private InventorySO inventory;
    [SerializeField] private UIInventoryPage inventoryUI;
    public List<InventoryItem> initialItems = new List<InventoryItem>();
    private InventoryController inventoryController;
    private InputGame _playerInput;
    private string gameObjectName;
    public string ID;
    [SerializeField] private bool isFirstChest;
    public bool IsFirstChest => isFirstChest;
    private bool firstFrame;
    private void Awake()
    {
        inventoryController = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryController>();
        inventoryUI = inventoryController.ChestInventoryPanel.GetComponent<UIInventoryPage>();
        _playerInput = new InputGame();
        if (ID == "")
        {
            ID = GetPersitantUniqueID(transform.position.sqrMagnitude, 1);
        }

    }
    void Start()
    {
        firstFrame = true;
        inventoryUI.gameObject.SetActive(false);
        gameObjectName = "Coffre - " + ID;
        gameObject.name = gameObjectName;
        CreateInventory();
        inventoryController.chestCount += 1;
        if (isFirstChest)
        {
            AddInventoryToInventoryController();
        }
/*        inventoryController.CreateTag(inventory.name);*/

    }

    
    void Update()
    {
        if (firstFrame && !isFirstChest)
        {
            firstFrame = false;
            AddInventoryToInventoryController();
        }
    }

    #region CreateInventory/DeleteInventory
    void CreateInventory()
    {
        string path = "Assets/ScriptableObject/ChestInventories/" + "Inventory-" + ID + ".asset";
/*        if (File.Exists(path))
        {
            inventory = AssetDatabase.LoadAssetAtPath<InventorySO>(path);
            Debug.LogWarning("Le Scriptable Object existe déjà sous le nom" + " Inventory-" + ID + ".");
            return; // Ne crée pas de nouveau SO s'il existe déjà.
        }*/
        inventory = Instantiate(inventoryModel);

        // Modifiez le nom de la nouvelle instance
        inventory.name = "Inventory-" + ID;
        inventory.Size = 36;

        // Assurez-vous que les modifications sont enregistrées
/*        AssetDatabase.CreateAsset(inventory, path);*/

        Debug.Log("Nouvel inventaire créé : " + inventory.name);
    }

/*    public void DeleteInventory()
    {
        string path = "Assets/ScriptableObject/ChestInventories/" + "Inventory-" + ID + ".asset";
        // Supprimez le Scriptable Object.
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.Refresh();
        Debug.Log("InventorySO supprimé avec succès : " + path);
    }*/
    #endregion
    public void AddInventoryToInventoryController()
    {
        inventoryController.Inventories.Add(inventory.name, inventory);
        inventoryController._inventoryUI.Add(inventory, inventoryUI);
        inventoryController._initialItems.Add(inventory, initialItems);
        inventoryController.PrepareInventoryDataChest(inventory);
        if (!inventoryController.ChestInventoryIsSet)
        {
            inventoryController.PrepareUIChest(inventory, inventoryUI);
        }
    }

    #region CreateUniquePersistantID
    private string GetPersitantUniqueID(float sqrMagnitude, int startNumber)
    {
        string sqrMagnitudeSTR = sqrMagnitude.ToString();
        string ID = "";
        foreach (char chara in sqrMagnitudeSTR)
        {
            if (chara != ',')
            {
                ID += chara;
            }
        }
        if (!DoesIDAlreadyExist(ID))
        {
            return ID;
        }
        else if (DoesIDAlreadyExist(ID))
        {
            int randomNumber = startNumber;
            string newID = ID + randomNumber.ToString();
            if (DoesIDAlreadyExist(newID))
            {
                GetPersitantUniqueID(sqrMagnitude,startNumber+1);
                if (!DoesIDAlreadyExist(ID + (startNumber + 1).ToString())) return ID + (startNumber + 1).ToString();

            }
            else if (!DoesIDAlreadyExist(newID))
            {
                ID = newID;
                return ID;
            }
        }
        return ID + startNumber;

    }

    bool DoesIDAlreadyExist(string ID)
    {
        List<string> IDs = new List<string>();
        foreach(ChestInventoryController chestInventoryController in GameObject.FindObjectsOfType<ChestInventoryController>())
        {
            
            if (chestInventoryController.ID !="")
            {
                IDs.Add(chestInventoryController.ID);
            }
        }
        foreach(string id in IDs)
        {
            if (id == ID)
            {
                return true;
            }
        }
        return false;

    }

    #endregion
    #region Show/Hide
    public void Show()
    {
        if (!inventoryController.MainInventoryPanel.activeSelf)
        {
            if (!inventoryUI.gameObject.activeSelf)
            {
                inventoryController.InventoryShow();
                if (inventoryController.MainInventoryPanel.activeSelf)
                {
                    inventoryUI.gameObject.GetComponent<TaggedObject>().tagDynamic = inventory.name;
                    inventoryController.RefreshInventoryUI(inventoryController.Inventories[inventory.name], inventoryController._inventoryUI[inventoryController.Inventories[inventory.name]]);
                    inventoryUI.gameObject.SetActive(true);
                    if (!inventoryUI.gameObject.activeSelf)
                    {
                        inventoryUI.gameObject.GetComponent<TaggedObject>().tagDynamic = inventory.name;
                        inventoryUI.gameObject.SetActive(true); // Il y'a un bug qui fait qu'au start je dois l'activer 2 fois. Solution temporraire !!!
                    }
                }
            }
            else
            {
                inventoryController.InventoryShow();
                if (!inventoryController.MainInventoryPanel.activeSelf)
                {
                    inventoryUI.gameObject.SetActive(false);
                }
            }

        }
    }

    public void Hide()
    {
        if (inventoryController.MainInventoryPanel.activeSelf)
        {
            inventoryController.InventoryShow();
            inventoryUI.gameObject.SetActive(false);
        }
    }
    public void HideInventory(InputAction.CallbackContext context)
    {
        if (context.performed && inventoryUI.gameObject.activeSelf)
        {
            Hide();
        }
    }
    #endregion

    #region OnEnable/OnDisable
    private void OnEnable()
    {
        _playerInput.Player.Escape.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Escape.Disable();
    }
    #endregion


}
