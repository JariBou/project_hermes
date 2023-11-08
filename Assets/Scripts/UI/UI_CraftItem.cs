using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Inventory.UI;
using System;
using Unity.VisualScripting;

public class UI_CraftItem : MonoBehaviour, IPointerClickHandler
{

    public Image imageItem;
    public string nameItem;
    public Image borderImage;
    [HideInInspector]
    public int index;

    private CraftSysteme craftSystem;
    private UI_CraftQuantityPanel quantityPanel;

    public event Action<UIInventoryItem> OnItemClicked;

    private void Awake()
    {
        craftSystem = GameObject.FindGameObjectWithTag("CraftPanel").GetComponent<CraftSysteme>();
        quantityPanel = GameObject.FindGameObjectWithTag("QuantitySelector").GetComponent<UI_CraftQuantityPanel>();
        ResetData();
        Deselect();
    }
    private void Start()
    {
       
    }

    public void SetData(Sprite sprite, string name)
    {
        imageItem.gameObject.SetActive(true);
        imageItem.sprite = sprite;
        nameItem = name;
    }
    public void ResetData()
    {
        imageItem.gameObject.SetActive(false);
    }
    public void Deselect()
    {
        borderImage.enabled = false;
    }
    public void OnPointerClick(PointerEventData pointerData)
    {
        if (nameItem != craftSystem.SelectedItem)
        {
            Debug.Log("Changement d'Item");
            craftSystem.SelectedItem = nameItem;
            for (int i = 0; i < gameObject.transform.parent.childCount; i++)
            {
                gameObject.transform.parent.GetChild(i).GetComponent<UI_CraftItem>().Deselect();
            }
            Select();
        }
    }

    public void Select()
    {
        quantityPanel.ChangeQuantity(1);
        borderImage.enabled = true;
        gameObject.transform.parent.transform.parent.GetComponent<CraftSysteme>().SetData(nameItem);
        craftSystem.SelectedItem = nameItem;

    }
}
