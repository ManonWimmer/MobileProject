using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class popUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _powerNeed;
    [SerializeField] private Image _icon;

    [SerializeField] private bool _popupVisible;
    [SerializeField] private Animator _animator;

    public void CheckPopupIsOpen() => CheckOpen();

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void CheckOpen()
    {
        if (_popupVisible)
        {
            Close();
        }
    }

    #region Description Open
    public void OpenDescAbility(scriptablePower scriptable)
    {
        _popupVisible = true;
        _animator.SetTrigger("selected");
        _animator.SetBool("disabled", false);

        _description.text = scriptable.Description;
        _name.text = scriptable.AbilityName;
        _powerNeed.text = scriptable.ActionPointsNeeded.ToString();
        _icon.sprite = scriptable.Icon;
    }

    /*public void OpenDescDebuff(scriptableDebuff scriptable)
    {
        _popupVisible = true;
        _animator.SetTrigger("selected");
        _animator.SetBool("disabled", false);

        _description.text = scriptable.Description;
        _name.text = scriptable.DebuffName;
        _icon.sprite = scriptable.Icon;
    }*/
    #endregion

    public void Close()
    {
        _animator.SetBool("disabled", true);
        if(!_popupVisible)
        {
            gameObject.SetActive(false);
        }
    }
}
