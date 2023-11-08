using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu(menuName = "ScriptableObject/EdibleItem")]
    public class EdibleItemSO : LootFortune, IDestroyableItem, IItemAction
    {
        [SerializeField]
        private List<ModifierData> modifierData = new List<ModifierData>();

        public string ActionName => "Consommer";

        public bool PerformAction(GameObject character, List<ItemParameter> itemState = null)
        {
            foreach (ModifierData data in modifierData)
            {
                data.statModifier.AffectCharacter(character, data.value);
            }
            return true;
        }
    }

    public interface IDestroyableItem
    {

    }

    public interface IItemAction
    {
        public string ActionName { get; }
        /*public AudioClip actionSFX { get; }*/
        bool PerformAction(GameObject character, List<ItemParameter> itemState);
    }

    [Serializable]
    public class ModifierData
    {
        public CharacterStatsModifierSO statModifier;
        public float value;
    }
}