using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioParameterSync : MonoBehaviour
{
    [SerializeField] private Slider _sliderSound1;
    [SerializeField] private Slider _sliderSound2;

    [SerializeField] private Slider _sliderMusic1;
    [SerializeField] private Slider _sliderMusic2;

    private void Start()
    {

        if (_sliderSound1 != null && _sliderSound2 != null)
        {
            _sliderSound1.onValueChanged.AddListener(OnSliderValueChangedSounds);
            _sliderSound2.onValueChanged.AddListener(OnSliderValueChangedSounds);
        }

        if(_sliderMusic1 != null && _sliderMusic2 != null)
        {
            _sliderMusic1.onValueChanged.AddListener(OnSliderValueChangedMusic);
            _sliderMusic2.onValueChanged.AddListener(OnSliderValueChangedMusic);
        }
    }

    private void OnSliderValueChangedSounds(float value)
    {
        if (_sliderSound1 != null && _sliderSound1.IsActive())
        {
            _sliderSound2.value = value;
        }
        else if (_sliderSound2 != null && _sliderSound2.IsActive())
        {
            _sliderSound1.value = value;
        }
    }

    private void OnSliderValueChangedMusic(float value)
    {
        if (_sliderMusic1 != null && _sliderMusic1.IsActive())
        {
            _sliderMusic2.value = value;
        }
        else if (_sliderMusic2 != null && _sliderMusic2.IsActive())
        {
            _sliderMusic1.value = value;
        }
    }
}
