using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TableManager : MonoBehaviour
{
    public TextMeshProUGUI quantityRequired;
    public TextMeshProUGUI quantitPossed;
    public TextMeshProUGUI requiredItemName;
    public TextMeshProUGUI totalQuantityRequired;



    void Start()
    {
        quantityRequired.text = "";
        quantitPossed.text = "";
        requiredItemName.text = "";
        totalQuantityRequired.text = "";
    }

    public void ChangeTextColor(Color color)
    {
        quantityRequired.color = color;
        quantitPossed.color = color;
        requiredItemName.color = color;
        totalQuantityRequired.color = color;
    }


}
