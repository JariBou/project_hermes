using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthHeart : MonoBehaviour
{
    [SerializeField] Sprite fullHeart, halfHeart, emptyHeart;
    Image heartImage;

    private void Awake()
    {
        heartImage = GetComponent<Image>();
    }
    
    public void SetHeartImage(HeartStatus heartStatus)
    {
        switch (heartStatus)
        {
            case HeartStatus.EMPTY:
                heartImage.sprite = emptyHeart;
                transform.Rotate(0, 0f, 0);
                break;
            case HeartStatus.HALF:
                heartImage.sprite = halfHeart;
                transform.Rotate(0, 180f, 0);
                break;
            case HeartStatus.FULL:
                heartImage.sprite = fullHeart;
                transform.Rotate(0, 0f, 0);
                break;
        }
    }
}

public enum HeartStatus
{
    EMPTY = 0,
    HALF = 1,
    FULL = 2,
}