using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;
    bool isDead;
    private bool _canBeAttacked;
    public bool CanBeAttacked => _canBeAttacked;
    public static event Action OnPlayerDamage;
    [SerializeField] private Animator _animator;
    [HideInInspector] public Vector2 _revivePoint;
    [SerializeField, Range(0,1)] private float _transparenceRevive;
    [SerializeField] private float _dureeInvacibilite;
    float dureeAvantRevive;
    void Start()
    {
        _animator = GetComponent<Animator>();
        _canBeAttacked = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            Revive();
        }
    }

    public void TakeHeal(int heal)
    {
        if (health < maxHealth && !isDead)
        {
            if (health + heal > maxHealth)
            {
                health = maxHealth;
                OnPlayerDamage?.Invoke();
            }
            else
            {
                health += heal;
                OnPlayerDamage?.Invoke();
            }
        
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        OnPlayerDamage?.Invoke();
        if (health <= 0 && !isDead)
        {
            dureeAvantRevive = 2;
            isDead = true;
            _canBeAttacked = false;
            _animator.SetBool("isDead", isDead);
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<PlayerAttack>().enabled = false;
        }
    }

    public void Revive()
    {
        dureeAvantRevive -= Time.deltaTime;
        if(dureeAvantRevive <= 0)
        {
            isDead = false;
            health = maxHealth;
            OnPlayerDamage?.Invoke();
            _animator.SetBool("isDead", isDead);
            transform.position = _revivePoint;
            StartCoroutine(Clignotement(_dureeInvacibilite, _transparenceRevive));
            GetComponent<PlayerMovement>().enabled = true;
            GetComponent<PlayerAttack>().enabled = true;

        }
    }

    IEnumerator Clignotement(float duree, float tranparence)
    {
        bool clignotement = true;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        while (clignotement)
        {   
            duree -= 1;
            spriteRenderer.color = new Color(1, 1, 1, tranparence);
            yield return new WaitForSeconds(0.5f);
            spriteRenderer.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(0.5f);
            if (duree <= 0)
            {
                clignotement = false;
                _canBeAttacked = true;
                break;
            }
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

        if (maxHealth < 0)
        {
            maxHealth = Mathf.Abs(maxHealth);
        }
        /*OnPlayerDamage?.Invoke();*/
    }
}
