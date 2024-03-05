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
    [SerializeField] Image _cooldownIcon;
    [SerializeField] GameObject _selectedBackground;
    [SerializeField] Image _abilityIcon;
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
        _cooldownTxt.enabled = false;
        _cooldownIcon.enabled = false;
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

    public void SelectedButtonUI()
    {
        _selectedBackground.SetActive(true);
        _image.color = Color.white;
        _cooldownTxt.color = new Color(0.094f, 0.09f, 0.15f, 1f); // Black
        _cooldownIcon.color = new Color(0.094f, 0.09f, 0.15f, 1f); // Black
        _abilityIcon.color = new Color(0.094f, 0.09f, 0.15f, 1f); // Black
    }

    public void OnlineAndCanBeUsedButtonUI()
    {
        _selectedBackground.SetActive(false);
        _image.color = Color.white;
        _cooldownTxt.color = new Color(0.34f, 0.54f, 0.76f, 1f); // Blue
        _cooldownIcon.color = new Color(0.34f, 0.54f, 0.76f, 1f); // Blue
        _abilityIcon.color = new Color(0.34f, 0.54f, 0.76f, 1f); // Blue
    }

    public void OnlineAndCantBeUsedButtonUI()
    {
        _selectedBackground.SetActive(false);
        _image.color = Color.gray;
        _cooldownTxt.color = new Color(0.34f, 0.54f, 0.76f, 1f); // Blue
        _cooldownIcon.color = new Color(0.34f, 0.54f, 0.76f, 1f); // Blue
        _abilityIcon.color = new Color(0.34f, 0.54f, 0.76f, 1f); // Blue
    }

    public void OfflineButtonUI()
    {
        _selectedBackground.SetActive(false);
        _image.color = Color.red;
        _cooldownTxt.color = new Color(0.094f, 0.09f, 0.15f, 1f); // Black
        _cooldownIcon.color = new Color(0.094f, 0.09f, 0.15f, 1f); // Black
        _abilityIcon.color = new Color(0.094f, 0.09f, 0.15f, 1f); // Black
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

            _cooldownTxt.enabled = true;
            _cooldownIcon.enabled = true;
            _cooldownTxt.text = cooldown.ToString();
        }
        else
        {
            if (GetComponent<Button>() == null)
            {
                _button = gameObject.AddComponent<Button>();
                _button.onClick = _onClickOnline;
            }

            _cooldownTxt.enabled = false;
            _cooldownIcon.enabled = false;
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

        _cooldownTxt.enabled = false;
        _cooldownIcon.enabled = false;
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

            _cooldownTxt.enabled = true;
            _cooldownIcon.enabled = true;
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

            _cooldownTxt.enabled = false;
            _cooldownIcon.enabled = false;
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
