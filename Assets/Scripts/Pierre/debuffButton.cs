using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class debuffButton : MonoBehaviour
{
    private Animator _animator;
    private GameObject _descriptionPage;

    [SerializeField] private scriptableDebuff _scriptable;
    [SerializeField] private Image _icon;

    private bool _enabled;
    private List<debuffButton> _debuffs = new List<debuffButton>();
    private List<powerButton> _abilitys = new List<powerButton>();


    public bool GetSelected() => _enabled;
    public void DisabledSelected() => UnSelected();

    private void Start()
    {
        _descriptionPage = GameObject.FindGameObjectWithTag("Description");
        _descriptionPage = _descriptionPage.transform.Find("contentDebuff").gameObject;

        _animator = GetComponent<Animator>();
        _animator.enabled = false;

        SetButton();
        AddList();
    }

    private void SetButton()
    {
        _icon.sprite = _scriptable.Icon;
    }

    #region ListButtons
    private void AddList()
    {
        GameObject[] debuffbuttons = GameObject.FindGameObjectsWithTag("Button");

        foreach (GameObject button in debuffbuttons)
        {
            if (button != gameObject)
            {
                powerButton powerButtonScript = button.GetComponent<powerButton>();
                debuffButton debuffButtonScript = button.GetComponent<debuffButton>();
                if (powerButtonScript != null)
                {
                    _abilitys.Add(powerButtonScript);
                }
                if(debuffButtonScript != null)
                {
                    _debuffs.Add(debuffButtonScript);
                }
            }
        }
    }

    private void CheckSelected()
    {
        foreach (debuffButton buttonSlected in _debuffs)
        {
            if (buttonSlected.GetSelected())
            {
                buttonSlected.DisabledSelected();
            }
        }
                foreach (powerButton buttonSlected in _abilitys)
        {
            if (buttonSlected.GetSelected())
            {
                 buttonSlected.DisabledSelected();
            }
        }
    }
    #endregion

    private void UnSelected()
    {
        if (_enabled)
        {
            CloseDescription();
            _enabled = false;
            _animator.SetBool("unselected", true);
        }
    }


    // SLECTION BUTTON
    public void Slected()
    {
        if (!_enabled) 
        {
            CheckSelected();
            _enabled = true;
            _animator.enabled = true;
            OpenDescription();

            _animator.SetBool("unselected", false);
            _animator.SetTrigger("selected");
        }else
        {
            UnSelected();
        }
    }

    // OPEN DESCRIPTION 
    public void OpenDescription()
    {
        _descriptionPage.SetActive(true);
        _descriptionPage.GetComponent<popUp>().OpenDescDebuff(_scriptable);
    }

    // CLOSE DESCRIPTION
    private void CloseDescription()
    {
        _descriptionPage.SetActive(false);
    }

}
