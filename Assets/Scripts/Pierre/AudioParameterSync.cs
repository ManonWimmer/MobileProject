using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioParameterSync : MonoBehaviour
{
    [SerializeField] private Slider _sliderSound1;

    [SerializeField] private Slider _sliderMusic1;

    private float _tempValueSound;
    private float _tempValueMusic;

    public float GetSounds() => _tempValueSound;
    public float GetMusic() => _tempValueMusic;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

        if (_sliderSound1 != null)
        {
            _sliderSound1.onValueChanged.AddListener(OnSliderValueChangedSounds);
        }

        if(_sliderMusic1 != null)
        {
            _sliderMusic1.onValueChanged.AddListener(OnSliderValueChangedMusic);
        }
    }

    private void OnSliderValueChangedSounds(float value)
    {
        if (_sliderSound1 != null && _sliderSound1.IsActive())
        {
            _tempValueSound = value;
        }
    }

    private void OnSliderValueChangedMusic(float value)
    {
        if (_sliderMusic1 != null && _sliderMusic1.IsActive())
        {
            _tempValueMusic = value;
        }
    }
}
