using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] objects;
    public static DontDestroyOnLoad instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            foreach (var element in objects)
            {
                Destroy(element);
            }
        }
        else
        {
            instance = this;
            foreach (var element in objects)
            {

                DontDestroyOnLoad(element);
            }
        }

    }

}