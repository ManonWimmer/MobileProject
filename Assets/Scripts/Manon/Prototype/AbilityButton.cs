using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    // ----- FIELDS ----- //
    [SerializeField] scriptablePower _ability;
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
    }

    public scriptablePower GetAbility()
    {
        return _ability;
    }

    public void SetOffline()
    {
        _image.color = Color.red;

        if (_button != null)
        {
            Destroy(_button);
        }
        IsOffline = true;
    }

    public void SetOnline()
    {
        _image.color = Color.white;
        if (GetComponent<Button>() == null)
        {
            _button = gameObject.AddComponent<Button>();
            _button.onClick = _onClickOnline;
        }
        IsOffline = false;
    }
}
