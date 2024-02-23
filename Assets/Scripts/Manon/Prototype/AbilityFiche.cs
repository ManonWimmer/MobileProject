using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFiche : MonoBehaviour
{
    // ----- FIELDS ----- //
    [SerializeField] AbilityButton _abilityButton;
    // ----- FIELDS ----- //

    public void ShowAbilityFiche()
    {
        UIManager.instance.ShowFicheAbility(_abilityButton.GetAbility());
    }
}
