using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
/*using UnityEditor.PackageManager;*/
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] float ZoneInterraction;
    public float zoneInterraction=>ZoneInterraction;

    [SerializeField] GameObject toucheE;
    [SerializeField] Vector2 positionToucheE;
    bool toucheEIsPressed;
    GameObject toucheEInstance;
    /*[SerializeField]*/ private bool isUsed;
    /*[SerializeField]*/ private bool hasBeenUsed;
    [SerializeField] private bool isASingleUseObject;
    [SerializeField] UnityEvent Event;
    [SerializeField] PlayerInput _playerInput;
    InputAction _toucheE;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }
    // Start is called before the first frame update
    void Start()
    {
        toucheEInstance = Instantiate(toucheE);
    }

    // Update is called once per frame
    void Update()
    {

        toucheEInstance.transform.position = new Vector2(transform.position.x + positionToucheE.x, transform.position.y + positionToucheE.y);
        toucheEInstance.SetActive(false);
        Collider2D[] zoneInterraction = Physics2D.OverlapCircleAll(transform.position, ZoneInterraction);
        foreach (Collider2D collision in zoneInterraction)
        {
            if (collision.gameObject.tag == "Player" && collision.isTrigger)
            {
                if (isASingleUseObject && !hasBeenUsed)
                {
                    toucheEInstance.SetActive(true);
                }
                else if (!isASingleUseObject)
                {
                    toucheEInstance.SetActive(true);
                }
                
            }
            if (isUsed)
            {
                if (!toucheEIsPressed) isUsed = false;
            }
        }
    }

    private void OnEnable()
    {
        _toucheE = _playerInput.Player.Interract;
        _toucheE.Enable();
        _toucheE.performed += Interract;
    }

    private void OnDisable()
    {
        _toucheE.Disable();
        _toucheE.canceled += Interract;
    }

    private void Interract(InputAction.CallbackContext context)
    {
        if (toucheEInstance.activeSelf)
        {
            if (context.performed)
            {
                toucheEIsPressed = true;
                isUsed = true;
                hasBeenUsed = true;
                this.Event.Invoke();
            }
            else if (context.canceled)
            {
                toucheEIsPressed = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, ZoneInterraction);
    }
}
