using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ShopItem : MonoBehaviour
{

    public Image imageItem;
    public TextMeshProUGUI nameItem;
    public TextMeshProUGUI prixItem;
    [HideInInspector]
    public int prix;
    public int index;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => GameObject.FindGameObjectWithTag("ShopPanel").GetComponent<UI_Shop>().BuyItem(this));

    }

}
