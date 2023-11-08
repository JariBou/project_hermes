using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float Gold = 0;
    [SerializeField] TextMeshProUGUI coinSTR;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (coinSTR != null)
        {
            coinSTR.text = Gold.ToString();
        }
            
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetType().ToString() == "UnityEngine.CircleCollider2D") // car les pièces ont plusieurs colliders.
        {
            if (collision.CompareTag("Coin1"))
            {
                Destroy(collision.gameObject);
                Gold += 1f;
            }
            if (collision.CompareTag("Coin5"))
            {
                Destroy(collision.gameObject);
                Gold += 5f;
            }
        }
    }
}
