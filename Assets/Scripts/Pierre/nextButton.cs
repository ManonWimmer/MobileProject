using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class nextButton : MonoBehaviour
{

    [SerializeField] private Image _step1;
    [SerializeField] private Image _step2;
    [SerializeField] private Image _step3;

    private bool _step1Pass;
    private bool _step2Pass;
    private bool _step3Pass;

    private bool _canPass;

    private Color _color = new Color(77.0f / 255.0f, 136.0f / 255.0f, 204.0f / 255.0f, 1.0f);
    private Color _colorNotpass = new Color(66.0f / 255.0f, 103.0f / 255.0f, 145.0f / 255.0f, 1.0f);


    void Start()
    {
        SetColorStep();
        _canPass = true;
    }

    private void SetColorStep()
    {
        _step1.color = _colorNotpass;
        _step2.color = _colorNotpass;
        _step3.color = _colorNotpass;
    }

    public void PassSteps()
    {
        if (!_step1Pass && _canPass) 
        {
            _step1.color = _color;
            _step1Pass = true;
        }
        else if (!_step2Pass && _canPass) 
        {
            _step2.color = _color;
            _step2Pass = true;
        }
        else if ( !_step3Pass && _canPass)
        {
            _step3.color = _color;
            _step3Pass = true;
        }
    }
}
