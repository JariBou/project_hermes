using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Voyager : MonoBehaviour
{
    public string destination;

    public void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().usedTP)
        {
            GameObject.FindGameObjectWithTag("Player").transform.position = new Vector2(transform.position.x,transform.position.y - 1.5f);
        }
        
    }
    public void voyager()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().usedTP = true;
        if (destination != null && destination == "SpiderJour2")
        {
            Debug.Log("Réveillez-vous !");
            StartCoroutine(voyageDans(1f));
        }
        else if (destination != null && destination == "DreamJour1")
        {
            Debug.Log("Bonne Nuit !");
            StartCoroutine(voyageDans(1f));
        }
        else if(destination != null)
        {
            Debug.Log("Bon Voyage !");
            StartCoroutine(voyageDans(1f));
        }

    }

    IEnumerator voyageDans(float secondes)
    {
        yield return new WaitForSeconds(secondes);
        SceneManager.LoadScene(destination);
        
    }
}
    
