using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/CharacterStatsHealthModifier")]
public class CharacterStatsHealthModifierSO : CharacterStatsModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        PlayerHealth health = character.GetComponent<PlayerHealth>();
        health?.TakeHeal((int)val);
    }
}
