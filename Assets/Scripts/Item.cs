using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [field: SerializeField] public LootFortune InventoryItem { get; set;}
    [field: SerializeField] public int Quantity { get; set; } = 1;
    /*    [SerializeField] AudioSource audioSource;*/
    [SerializeField] private float duration = 0.15f;

    public bool hasSpaceToBePickUp;

    


    public IEnumerator AnimateItemPickup()
    {
        /*audioSource.Play();*/ //Si jamais je veux ajouter un son de pickup
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, currentTime/duration);
            yield return null;
        }
        Destroy(gameObject);
    }

   

}
