using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class powerButton : MonoBehaviour
{
    [SerializeField] private scriptablePower _scriptable;
    [SerializeField] private TextMeshProUGUI _powerNeed;
    [SerializeField] private Image _icon;
    private GameObject _description;

    //Cooldown
    [SerializeField] private GameObject _reloadButton;
    private TextMeshProUGUI _timerCooldown;
    private int _turnCooldown;
    private int _cooldown;

    private Animator _animator;
    private bool _selected;
    private List<powerButton> _selectedObjects = new List<powerButton>();

    public void GetEndTrun() => EndTrun();

    public bool GetSelected() => _selected;
    public void DisabledSelected() => UnSelected();

    private void Start()
    {
        _description = GameObject.FindGameObjectWithTag("Description");
        _description = _description.transform.Find("content").gameObject;

        _animator = GetComponent<Animator>();
        _animator.enabled = false;
        AddList();

        _icon.sprite = _scriptable._icon;
        _powerNeed.text = _scriptable._powerNeed.ToString();

        _turnCooldown = _scriptable._cooldown;
    }

    private void AddList()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");

        foreach (GameObject button in buttons)
        {
            if (button != gameObject)
            {
                powerButton powerButtonScript = button.GetComponent<powerButton>();
                if (powerButtonScript != null)
                {
                    _selectedObjects.Add(powerButtonScript);
                }
            }
        }
    }

    private void CheckSelected()
    {
        foreach (powerButton buttonSlected in _selectedObjects)
        {
            if (buttonSlected.GetSelected())
            {
                 buttonSlected.DisabledSelected();
            }
        }
    }

    // SELECTED
    public void Action()
    {
        if (_cooldown <= 1)
        {
            CheckSelected();
            _selected = true;
            _animator.enabled = true;

            _animator.SetBool("UnSelected", false);
            _animator.SetTrigger("Selected");

            //Attack
            //Cooldown();
        }
    }

    private void UnSelected()
    {
        if (_selected) 
        {
            _selected = false;
            _animator.SetBool("UnSelected", true);
        }
    }


    // DESCRIPTION
    public void OpenDescription()
    {
        _description.SetActive(true);
        _description.GetComponent<popUp>().OpenDesc(_scriptable);
    }


    // END TRUN
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


    // COOLDOWN
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
