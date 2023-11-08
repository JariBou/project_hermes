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
        bool canStack = false;
        if (item.InventoryItem.IsStackable)
        {
            foreach (InventoryItem itm in toolbarInventoryData.inventoryItems)
            {
                if (item.InventoryItem == itm.item && (item.Quantity + itm.quantity) <= itm.item.MaxStackSize)
                {
                    canStack = true;
                }
            }
            if (!toolbarInventoryData.IsInventoryFull() && canStack)
            {
                reminder = toolbarInventoryData.AddItem(item.InventoryItem, item.Quantity);
                if (reminder == 0)
                {
                    StartCoroutine(item.AnimateItemPickup());
                }
                else
                {
                    item.Quantity = reminder;
                }
            }
            else
            {
                canStack = false;
                foreach (InventoryItem itm in mainInventoryData.inventoryItems)
                {
                    if (item.InventoryItem == itm.item && (item.Quantity + itm.quantity) <= itm.item.MaxStackSize)
                    {
                        canStack = true;
                    }
                }
                if (canStack)
                {
                    reminder = mainInventoryData.AddItem(item.InventoryItem, item.Quantity);
                    if (reminder == 0)
                    {
                        StartCoroutine(item.AnimateItemPickup());
                    }
                    else
                    {
                        item.Quantity = reminder;
                    }
                }
            }
        }
        else
        {
            if (!toolbarInventoryData.IsInventoryFull())
            {
                reminder = toolbarInventoryData.AddItem(item.InventoryItem, item.Quantity);
                if (reminder == 0)
                {
                    StartCoroutine(item.AnimateItemPickup());
                }
                else
                {
                    item.Quantity = reminder;
                }
            }
            else
            {

                reminder = mainInventoryData.AddItem(item.InventoryItem, item.Quantity);
                if (reminder == 0)
                {
                    StartCoroutine(item.AnimateItemPickup());
                }
                else
                {
                    item.Quantity = reminder;
                }
                
            }
        }

    }

    public int AddItemFromShop(Item item)
    {
        /*Debug.Log("Ajout");*/
        int reminder;
        reminder = toolbarInventoryData.AddItem(item.InventoryItem, item.Quantity);
        if (reminder > 0)
        {
            item.Quantity = reminder;
            reminder = mainInventoryData.AddItem(item.InventoryItem, item.Quantity);
        }
        return reminder;
    }
}
