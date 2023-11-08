using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] float ZoneInterraction;
    [SerializeField] bool isInteractable;
    private bool isInteractableTMP;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] zoneInterraction = Physics2D.OverlapCircleAll(transform.position, ZoneInterraction);
        foreach (Collider2D collision in zoneInterraction)
        {
            if (collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<PlayerHealth>()._revivePoint = new Vector2(transform.position.x, transform.position.y - 1.5f);

            }
        }
    }



    private void OnDrawGizmos()
    {
        if(GetComponent<InteractableObject>() == null || ZoneInterraction != GetComponent<InteractableObject>().zoneInterraction)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ZoneInterraction);
        }
    }

    private void OnValidate()
    {
        if (isInteractable && isInteractable!=isInteractableTMP)
        {
            if (GetComponent<InteractableObject>() != null)
            {
                ZoneInterraction = GetComponent<InteractableObject>().zoneInterraction;
            }
            else
            {
                Debug.Log("Il manque le script 'InteractableObject' pour pouvoir activer cette option");
                isInteractable = false;
            }
        }
        isInteractableTMP = isInteractable;
    }
}
