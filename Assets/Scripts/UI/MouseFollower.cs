using Inventory.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MouseFollower : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private UIInventoryItem _item;
    [SerializeField] private InputGame _playerInput;

    public void Awake()
    {
        _playerInput = new InputGame();
        _canvas = transform.root.GetComponent<Canvas>();
        _mainCamera = Camera.main;
        _item = GetComponentInChildren<UIInventoryItem>();
    }

    public void SetData(Sprite sprite, int quantity)
    {
        _item.SetData(sprite, quantity);
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)_canvas.transform,Mouse.current.position.ReadValue() , _canvas.worldCamera, out position);
        transform.position = _canvas.transform.TransformPoint(position);
    }

    public void Toggle(bool val)
    {
        /*Debug.Log($"Item Toggled {val}");*/
        gameObject.SetActive(val);
    }


    private void OnEnable()
    {
        _playerInput.UI.Point.Enable();
    }

    private void OnDisable()
    {
        _playerInput.UI.Point.Disable();

    }
}
