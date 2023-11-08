using Inventory;
using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UI_QuantityPanel : MonoBehaviour
{
    [SerializeField] private int quantity;
    [SerializeField] public UnityEngine.UI.Slider slider;
    [SerializeField] private TMP_InputField inputField;

    private InputGame _playerInput;
    public InventoryItem item;
    public InventorySO inventoryItemSplit;
    public int itemIndex;

    private void Awake()
    {
        _playerInput = new InputGame();
    }

    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateQuantityByInput()
    {
        if (int.Parse(inputField.text) >= slider.minValue && int.Parse(inputField.text) <= slider.maxValue)
        {
            quantity = int.Parse(inputField.text);
            slider.value = quantity;
        }
        else
        {
            Debug.Log("Valeur d'entrée incorect");
        }
    }
    public void UpdateQuantityBySlider()
    {
        quantity = (int)slider.value;
        inputField.text = quantity.ToString();
    }
    public void QuantityUp()
    {
        if (quantity +1 <= slider.maxValue)
        {
            quantity += 1;
            slider.value = quantity;
            inputField.text = quantity.ToString();
        }
        else
        {
            Debug.Log("Quantité Max Atteinte");
        }
        
    }
    public void QuantityDown()
    {
        if (quantity -1 >= slider.minValue)
        {
            quantity -= 1;
            slider.value = quantity;
            inputField.text = quantity.ToString();
        }
        else
        {
            Debug.Log("Quantité Min Atteinte");
        }

    }

    public void Escape(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            transform.parent.gameObject.SetActive(false);
        }
    }

    public void Submit(InputAction.CallbackContext context)
    {
        if (context.performed && transform.parent.gameObject.activeSelf)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryController>().RemoveQuantityItem(inventoryItemSplit, itemIndex, quantity);
            transform.parent.gameObject.SetActive(false);
        }
    }


    private void OnEnable()
    {
        quantity = 1;
        _playerInput.Player.Escape.Enable();
        _playerInput.Player.Submit.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Escape.Disable();
        _playerInput.Player.Submit.Disable();
    }
}
