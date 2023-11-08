using Inventory;
using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UI_CraftQuantityPanel : MonoBehaviour
{
    public int quantity;
    [SerializeField] private TMP_InputField inputField;

    private InputGame _playerInput;
    [SerializeField] private int maxValue = 100;
    CraftSysteme craftSysteme;

    private void Awake()
    {
        _playerInput = new InputGame();
        
    }

    private void Start()
    {
        craftSysteme = GameObject.FindGameObjectWithTag("CraftPanel").GetComponent<CraftSysteme>();
    }

    public void UpdateQuantityByInput()
    {
        if (int.Parse(inputField.text) >= 1 && int.Parse(inputField.text) <= maxValue)
        {
            quantity = int.Parse(inputField.text);
            craftSysteme.UpdateTotalQuantity(quantity);
            
        }
        else
        {
            Debug.Log("Valeur d'entrée incorect");
        }
    }
    public void QuantityUp()
    {
        if (quantity + 1 <= maxValue)
        {
            quantity += 1;
            inputField.text = quantity.ToString();
            craftSysteme.UpdateTotalQuantity(quantity);
        }
        else
        {
            Debug.Log("Quantité Max Atteinte");
        }

    }
    public void QuantityDown()
    {
        if (quantity - 1 >= 1)
        {
            quantity -= 1;
            inputField.text = quantity.ToString();
            craftSysteme.UpdateTotalQuantity(quantity);
        }
        else
        {
            Debug.Log("Quantité Min Atteinte");
        }

    }

    public void ChangeQuantity(int newQuantity)
    {
        quantity = newQuantity;
        inputField.text = newQuantity.ToString();
    }
    public void ChangeColor(Color color)
    {
        inputField.caretColor = color;

    }
    public void Submit(InputAction.CallbackContext context)
    {
        if (context.performed && transform.parent.gameObject.activeSelf)
        {
           
        }
    }

/*    private void OnEnable()
    {
        quantity = 1;
        _playerInput.Player.Submit.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Submit.Disable();
    }*/
}
