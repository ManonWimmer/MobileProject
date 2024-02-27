using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class debuffButton : MonoBehaviour
{
    private Animator _animator;
    private bool _enabled;
    [SerializeField] private scriptableDebuff _scriptable;

    [SerializeField] private Image _icon;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.enabled = false;
        SetButton();
    }

    private void SetButton()
    {
        _icon.sprite = _scriptable._icon;
    }

    public void Slected()
    {
        if (!_enabled) 
        {
            _enabled = true;
            _animator.enabled = true;

            _animator.SetBool("unselected", false);
            _animator.SetTrigger("selected");
        }else
        {
            _enabled = false;
            _animator.SetBool("unselected", true);
        }
    }
}
