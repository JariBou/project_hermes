using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHeartBar : MonoBehaviour
{
    [SerializeField] GameObject heartPrefab;
    [SerializeField] PlayerHealth playerHealth;
    List<HealthHeart> hearts = new List<HealthHeart>();
    // Start is called before the first frame update
    private void OnEnable()
    {
        PlayerHealth.OnPlayerDamage += DrawHearts;
    }
    private void OnDisable()
    {
        PlayerHealth.OnPlayerDamage -= DrawHearts;
    }


    private void Start()
    {
        DrawHearts();
    }

    public void DrawHearts()
    {
        ClearHearths();
        float maxHealthRemainder = playerHealth.maxHealth % 2;
        int heartsToMake = (int)((playerHealth.maxHealth / 2) + maxHealthRemainder);
        for (int i = 0; i < heartsToMake; i++)
        {
            CreateEmptyHeart();
        }
        for (int i = 0; i< hearts.Count; i++)
        {
            int hearStatusRamainder = (int)Mathf.Clamp(playerHealth.health - (i * 2), 0, 2);
            hearts[i].SetHeartImage((HeartStatus)hearStatusRamainder);
        }

    }
    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab);
        newHeart.transform.SetParent(transform);
        HealthHeart heartComponent = newHeart.GetComponent<HealthHeart>();
        heartComponent.SetHeartImage(HeartStatus.EMPTY);
        hearts.Add(heartComponent);
    }
    public void ClearHearths()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        hearts =  new List<HealthHeart> ();
    }
}
