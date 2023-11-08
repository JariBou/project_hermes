using Inventory;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] InputGame _playerInput;
    [SerializeField] Animator _animator;
    [SerializeField] Transform _attackPoint;
    [SerializeField] float _attackRange;
    [SerializeField] LayerMask _enemyLayers;
    // Update is called once per frame
    void Awake()
    {
        _playerInput = new InputGame();
    }
    void Update()
    {
        TournerY();
    }
    void Attack() 
    {
        _animator.SetTrigger("Attack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRange, _enemyLayers);
        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit "+ enemy.gameObject.name);
            if (enemy.gameObject.layer == 9 )
            {
                StartCoroutine(enemy.gameObject.GetComponent<EnemyHealth>().TakeDamage(1));
            }
        }
    }

    public void LeftCLick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
/*            Debug.Log(GetComponent<InventoryController>().GetActiveInventoryUnderMouseTag());*/
            /*Debug.Log("J'appuie");*/
            if (!GetComponent<InventoryController>().MainInventaire.isActiveAndEnabled && GetComponent<InventoryController>().GetActiveInventoryUnderMouseTag() != "ToolbarInventory")
            {
                Attack();
            }
            
        }
        else if (context.canceled)
        {
            /*Debug.Log("Je relache");*/
        }
    }

    private void OnEnable()
    {
        _playerInput.Player.Fire.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Fire.Disable();
        
    }

    void TournerY()
    {
        if (GetComponent<PlayerMovement>().HorizontalIdle > 0) //Vers la droite
        {
            _attackPoint.position = new Vector2(transform.position.x + 0.95f, transform.position.y + 0.25f);
        }
        else if (GetComponent<PlayerMovement>().HorizontalIdle < 0) // Vers la gauche
        {
            _attackPoint.position = new Vector2(transform.position.x - 0.95f, transform.position.y + 0.25f);
        }

        if (GetComponent<PlayerMovement>().VerticalIdle > 0) // Vers le haut
        {
            _attackPoint.position = new Vector2(transform.position.x, transform.position.y+0.9f);
        }
        else if (GetComponent<PlayerMovement>().VerticalIdle < 0) // Vers le bas
        {
            _attackPoint.position = new Vector2(transform.position.x, transform.position.y+  - 0.15f);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (_attackPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);
    }


}

