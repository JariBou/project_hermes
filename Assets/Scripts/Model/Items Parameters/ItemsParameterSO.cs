using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu(menuName = "ScriptableObject/ItemParameter")]
    public class ItemsParameterSO : ScriptableObject
    {
        [field: SerializeField]
        public string parameterName {  get; private set; }
    }
}