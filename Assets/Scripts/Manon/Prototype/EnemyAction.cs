using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAction : MonoBehaviour
{
    // ----- FIELDS ----- //
    [SerializeField] Image _abilityIcon;
    private scriptablePower _ability;
    // ----- FIELDS ----- //

    public void InitEnemyAction(scriptablePower ability)
    {
        Debug.Log("init enemy action");
        _ability = ability;

        GetComponent<Image>().enabled = true;
        _abilityIcon.enabled = true;

        _abilityIcon.sprite = _ability.Icon;
    }

    public void HideEnemyAction()
    {
        Debug.Log("hide enemy action");
        GetComponent<Image>().enabled = false;
        _abilityIcon.enabled = false;
    }

    public void EnemyActionToggleFiche()
    {
        if (CameraController.instance.IsMoving)
            return;

        if (UIManager.instance.IsFicheAbilityOpened())
        {
            if (UIManager.instance.IsFicheAbilityWithSameAbility(_ability))
            {
                UIManager.instance.HideFicheAbility();
            }
            else
            {
                UIManager.instance.UpdateFicheAbility(_ability);
            }
        }
        else
        {
            UIManager.instance.ShowFicheAbility(_ability);
        }

    }
}
