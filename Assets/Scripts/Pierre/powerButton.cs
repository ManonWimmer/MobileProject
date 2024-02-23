using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class powerButton : MonoBehaviour
{
    [SerializeField] private scriptablePower _scriptable;
    private GameObject _description;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _powerNeed;

    [SerializeField] private GameObject _reloadButton;
    private int _turnCooldown = 1;
    private int _cooldown;
    private TextMeshProUGUI _timerCooldown;

    public void GetEndTrun() => EndTrun();

    private void Start()
    {
        _description = GameObject.FindGameObjectWithTag("Description");
        _description = _description.transform.Find("content").gameObject;

        _icon.sprite = _scriptable._icon;
        _powerNeed.text = _scriptable._powerNeed.ToString();

        _turnCooldown = _scriptable._cooldown;
    }

    public void Action()
    {
        if (_cooldown <= 1)
        {
            //Attack
            Cooldown();
        }
    }

    public void OpenDescription()
    {
        _description.SetActive(true);
        _description.GetComponent<popUp>().OpenDesc(_scriptable);
    }

    private void EndTrun()
    {
        if (_cooldown > 1)
        {
            _cooldown -= 1;
            _timerCooldown.text = _cooldown.ToString();
        } else
        {
            _cooldown = 0;
            _reloadButton.SetActive(false);
        }
    }

    private void Cooldown()
    {
        if(_turnCooldown > 1)
        {
            _reloadButton.SetActive(true);
            _timerCooldown = _reloadButton.GetComponentInChildren<TextMeshProUGUI>();
            _cooldown = _turnCooldown;
            _timerCooldown.text = _turnCooldown.ToString();
        }
    }
}
