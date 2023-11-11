using Inventory.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPickUpSystem : MonoBehaviour
{
    [SerializeField] public InventorySO mainInventoryData;
    [SerializeField] public InventorySO toolbarInventoryData;

    public void HasSpaceToStoreItem(GameObject obj)
    {
        Item item = obj.GetComponent<Item>();
        foreach(InventoryItem itm in mainInventoryData.inventoryItems)
        {
            if (item.InventoryItem == itm.item && (item.Quantity + itm.quantity) <= itm.item.MaxStackSize)
            {
                item.hasSpaceToBePickUp = true;
                return;
            }
        }
        if (mainInventoryData.IsInventoryFull())
        {
            item.hasSpaceToBePickUp = false;
        }
        else
        {
            item.hasSpaceToBePickUp = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetType().ToString() == "UnityEngine.CircleCollider2D")
        {
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                AddItem(item);
            }
        }
    }

    public void AddItem(Item item)
    {
        Debug.Log("Ajout");
        int reminder;
        int reminder2;
        reminder = toolbarInventoryData.AddItem(item.InventoryItem, item.Quantity);
        if (reminder == 0)
        {
            StartCoroutine(item.AnimateItemPickup());
        }
        else if (reminder > 0)
        {
            reminder2 = mainInventoryData.AddItem(item.InventoryItem, reminder);
            if (reminder2 == 0)
            {
                StartCoroutine(item.AnimateItemPickup());
            }
            else
            {
                item.Quantity = reminder2;
            }
        }
        

        

                
        
        

    }

    public int AddItemFromShop(LootFortune item, int quantity, InventorySO inventory1, InventorySO inventory2)
    {
        /*Debug.Log("Ajout");*/
        int reminder;
        reminder = inventory1.AddItem(item, quantity);
        if (reminder > 0)
        {
            quantity = reminder;
            reminder = inventory2.AddItem(item, quantity);
        }
        return reminder;
    }

    public int RemoveItemQuantityFromInventory(LootFortune item, int quantity, InventorySO inventory1)
    {
        for (int i = 0; i < inventory1.Size; i++)
        {
            if (inventory1.inventoryItems[i].item == item)
            {
                if (inventory1.inventoryItems[i].quantity - quantity > 0)
                {
                    inventory1.inventoryItems[i] = inventory1.inventoryItems[i].ChangeQuantity(inventory1.inventoryItems[i].quantity - quantity);
                    inventory1.InformAboutChange();
                    return 0;
                }
                else if (inventory1.inventoryItems[i].quantity - quantity == 0)
                {
                    inventory1.inventoryItems[i] = InventoryItem.GetEmptyItem();
                    inventory1.InformAboutChange();
                    return 0;

                }
                else
                {
                    quantity -= inventory1.inventoryItems[i].quantity;
                    inventory1.inventoryItems[i] = InventoryItem.GetEmptyItem();
                    inventory1.InformAboutChange();
                }
            }
        }
        return quantity;

        
    }

}
