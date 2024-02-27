using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debuffButton : MonoBehaviour
{
    private Animator _animator;
    private bool _enabled;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.enabled = false;
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

    private void AddDebuff()
    {

    }
}
