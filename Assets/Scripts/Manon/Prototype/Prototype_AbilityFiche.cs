using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prototype_AbilityFiche : MonoBehaviour
{
    // ----- FIELDS ----- //
    [SerializeField] Prototype_AbilityButton _abilityButton;
    // ----- FIELDS ----- //

    public void ShowAbilityFiche()
    {
        Prototype_ManagerUI.instance.ShowFicheAbility(_abilityButton.GetAbility());
    }
}
