using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFiche : MonoBehaviour
{
    // ----- FIELDS ----- //
    [SerializeField] AbilityButton _abilityButton;
    // ----- FIELDS ----- //

    public void ToggleAbilityFiche()
    {
        if (CameraController.instance.IsMoving)
            return;

        if (!UIManager.instance.IsFicheAbilityOpened())
        {
            if (UIManager.instance.IsFicheAbilityWithSameAbility(_abilityButton.GetAbility()))
            {
                UIManager.instance.HideFicheAbility();
            }
            else
            {
                UIManager.instance.UpdateFicheAbility(_abilityButton.GetAbility());
            }
        }
        else
        {
            UIManager.instance.ShowFicheAbility(_abilityButton.GetAbility());
        }

    }
}
