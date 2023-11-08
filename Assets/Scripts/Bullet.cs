using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int _damage;
    [SerializeField] float Speed;
    [SerializeField] Animator animator;
    [SerializeField] GameObject DestroyPrefab;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = transform.right * Speed;
        animator.SetBool("Touch", true);
        /*Vector3 direction = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
        float rot = Mathf.Atan2(-direction.y, -direction.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,rot+90); */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(_damage);


        }
        animator.SetBool("Touch", true);
        Instantiate(DestroyPrefab, gameObject.transform.position, gameObject.transform.rotation);
        Destroy(gameObject);
    }
}
