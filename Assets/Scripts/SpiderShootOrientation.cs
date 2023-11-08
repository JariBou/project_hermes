using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderShootOrientation : MonoBehaviour
{
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = new Vector2(GameObject.FindGameObjectWithTag("Player").transform.position.x - transform.position.x, GameObject.FindGameObjectWithTag("Player").transform.position.y - transform.position.y);
        transform.up = direction;
    }
}
