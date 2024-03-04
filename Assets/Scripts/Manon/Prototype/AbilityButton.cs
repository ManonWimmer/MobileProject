using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;

public class AbilityButton : MonoBehaviour
{
    // ----- FIELDS ----- //
    [SerializeField] scriptablePower _ability;
    [SerializeField] TMP_Text _cooldownTxt;
    private Image _image;
    private Button _button;
    private Button.ButtonClickedEvent _onClickOnline;
    private Tile _target;

    private bool _isSelected;

    public bool IsOffline = false;

    public bool IsSelected { get => _isSelected; set => _isSelected = value; }

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

    public void SelectButton()
    {
        Debug.Log("select ability button");
        _isSelected = true;
    }

    public void DeselectButton()
    {
        Debug.Log("deselect ability button");
        _isSelected = false;
    }

    public void SetCooldown()
    {
        GameManager.instance.SetAbilityCooldown(_ability);
        UpdateCooldown();
    }

    public void UpdateCooldown()
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
            _cooldownTxt.text = cooldown.ToString();
        }
        else
        {
            if (GetComponent<Button>() == null)
            {
                _button = gameObject.AddComponent<Button>();
                _button.onClick = _onClickOnline;
            }

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
            _image.color = Color.gray;

            if (_button != null)
            {
                Destroy(_button);
            }

            _cooldownTxt.gameObject.SetActive(true);
            _cooldownTxt.text = cooldown.ToString();
        }
        else
        {
            _image.color = Color.white;
            if (GetComponent<Button>() == null)
            {
                _button = gameObject.AddComponent<Button>();
                _button.onClick = _onClickOnline;
            }
            IsOffline = false;

            _cooldownTxt.gameObject.SetActive(false);
        }   
    }

    public void SelectOrDeselectAbility()
    {
        Debug.Log("select / deselect ability");
        if (AbilityButtonsManager.instance.IsProbeStarted())
        {
            return;
        }
        
        if (GameManager.instance.TargetOnTile == null)
        {
            GameManager.instance.SetRoundTargetPos();
        }

        _target = GameManager.instance.TargetOnTile;

        if (!IsSelected)
        {
            if (GameManager.instance.CanUseAbility(_ability))
            {
                AbilityButtonsManager.instance.SelectAbilityButton(this);
            }
        }
        else
        {
            AbilityButtonsManager.instance.DeselectAbilityButton(this);
        }
    }
}
