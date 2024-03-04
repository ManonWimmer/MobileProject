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
    [SerializeField] private GameObject _cooldownGameobject;
    [SerializeField] private GameObject _energyGameobject;

    [SerializeField] bool _haveCooldown;
    private GameObject _description;

    //Cooldown
    [SerializeField] private GameObject _reloadButton;
    private TextMeshProUGUI _timerCooldown;
    private int _turnCooldown;
    private int _cooldown;

    private bool _selected;
    private List<powerButton> _abilitys = new List<powerButton>();
    private List<debuffButton> _debuffs = new List<debuffButton>();

    private Animator _animator;

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

        _icon.sprite = _scriptable.Icon;
        _powerNeed.text = _scriptable.ActionPointsNeeded.ToString();

        _turnCooldown = _scriptable.Cooldown;

        if (_haveCooldown)
        {
            _cooldownGameobject.SetActive(true);
            _energyGameobject.SetActive(false);
        }else
        {
            _cooldownGameobject.SetActive(false);
            _energyGameobject.SetActive(true);
        }
    }

    #region ListButtons
    private void AddList()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");

        foreach (GameObject button in buttons)
        {
            if (button != gameObject)
            {
                powerButton powerButtonScript = button.GetComponent<powerButton>();
                debuffButton debuffButtonScript = button.GetComponent<debuffButton>();
                if (powerButtonScript != null)
                {
                    _abilitys.Add(powerButtonScript);
                }
                if (debuffButtonScript != null)
                {
                    _debuffs.Add(debuffButtonScript);
                }
            }
        }
    }

    private void CheckSelected()
    {
        foreach (powerButton buttonSlected in _abilitys)
        {
            if (buttonSlected.GetSelected())
            {
                 buttonSlected.DisabledSelected();
            }
        }
        foreach (debuffButton buttonSlected in _debuffs)
        {
            if (buttonSlected.GetSelected())
            {
                buttonSlected.DisabledSelected();
            }
        }
    }
    #endregion

    // SELECTED
    public void Action()
    {
        if (_cooldown <= 1 && !_selected)
        {
            CheckSelected();
            _selected = true;
            _animator.enabled = true;

            _animator.SetBool("UnSelected", false);
            _animator.SetTrigger("Selected");

            //Attack
            //Cooldown();
        } else
        {
            UnSelected();
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


    // OPEN DESCRIPTION 
    public void OpenDescription()
    {
        _description.SetActive(true);
        _description.GetComponent<popUp>().OpenDescAbility(_scriptable);
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
