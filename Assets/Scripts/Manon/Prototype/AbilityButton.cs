using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class AbilityButton : MonoBehaviour
{
    // ----- FIELDS ----- //
    [SerializeField] scriptablePower _ability;
    [SerializeField] TMP_Text _cooldownTxt;
    private Image _image;
    private Button _button;
    private Button.ButtonClickedEvent _onClickOnline;
    private bool _isSelected;

    public bool IsOffline = false;
    // ----- FIELDS ----- //

    private void Start()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
        _onClickOnline = _button.onClick;
        _cooldownTxt.gameObject.SetActive(false);
    }

    public scriptablePower GetAbility()
    {
        return _ability;
    }

    public void SetCooldown()
    {
        int cooldown = GameManager.instance.GetCurrentCooldown(_ability);

        if (cooldown > 0)
        {
            //_image.color = Color.gray;

            if (_button != null)
            {
                Destroy(_button);
            }

            _cooldownTxt.gameObject.SetActive(true);
            _cooldownTxt.text = "Cooldown " + cooldown.ToString();
        }
        else
        {
            _cooldownTxt.gameObject.SetActive(false);
        }
    }

    public void SetOffline()
    {
        _image.color = Color.red;

        if (_button != null)
        {
            Destroy(_button);
        }
        IsOffline = true;

        _cooldownTxt.gameObject.SetActive(false);
    }

    public void SetOnline()
    {
        Debug.Log("set online " + _ability.name);
        int cooldown = GameManager.instance.GetCurrentCooldown(_ability);

        if (cooldown > 0)
        {
            //_image.color = Color.gray;

            if (_button != null)
            {
                Destroy(_button);
            }

            _cooldownTxt.gameObject.SetActive(true);
            _cooldownTxt.text = "Cooldown " + cooldown.ToString();
        }
        else
        {
            //_image.color = Color.white;
            if (GetComponent<Button>() == null)
            {
                _button = gameObject.AddComponent<Button>();
                _button.onClick = _onClickOnline;
            }
            IsOffline = false;

            _cooldownTxt.gameObject.SetActive(false);
        }   
    }
}
