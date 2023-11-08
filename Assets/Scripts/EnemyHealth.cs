using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;

public class EnemyHealth : MonoBehaviour
{
    // Start is called before the first frame update*
    [SerializeField] float health = 1;
    [SerializeField] float maxHealth;
    public bool isDead;
    [SerializeField] Animator animator;
    [SerializeField] Color damageColor;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] int coinValue;
    [SerializeField] GameObject Coin1;
    [SerializeField] GameObject Coin5;
    void Start()
    {
/*        Debug.Log(coinValue / 5);
        Debug.Log(coinValue % 5);*/
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator TakeDamage(float damage)
    {
        health -= damage;
        if (!isDead)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = new Color(255, 255, 255);
        }

        if (health <= 0 && !isDead)
        {
            isDead = true;
            animator.SetTrigger("Dead");
            if (coinValue > 5)
            {
                for (int i = 0; i < coinValue/5; i++)
                {
                    Instantiate(Coin5, gameObject.transform.position, Quaternion.identity);
                }
                for (int i = 0; i < coinValue%5; i++)
                {
                    Instantiate(Coin1, gameObject.transform.position, Quaternion.identity);
                }
            }
            else if (coinValue < 5)
            {
                for (int i = 0; i < coinValue; i++)
                {
                    Instantiate(Coin1, gameObject.transform.position, Quaternion.identity);
                }
            }

            Destroy(gameObject, 15f);
        }
    }

    private void OnValidate()
    {
        if (health > maxHealth)
        {
               maxHealth = health;
        }

        if (health < 0)
        {
            health = Mathf.Abs(health);
        }

        if(maxHealth < 0)
        {
            maxHealth = Mathf.Abs(maxHealth); 
        }

        if(coinValue < 0)
        {
            coinValue = 0;
        }
    }
}
