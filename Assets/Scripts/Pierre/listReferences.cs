using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class listReferences : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> _listTextTranslate;
    [SerializeField] private Slider _sliderMusic;
    [SerializeField] private Slider _sliderSounds;
    [SerializeField] private TMP_Dropdown _dropdownLanguage;

    private xmlReader _xmlReader;
    private audioManager _audioManager;
    private AudioSource _audioSourceSound;

    public AudioSource GetaudioSource() => _audioSourceSound;
    public audioManager GetaudioManager() => _audioManager;

    
    private void Awake()
    {
        _audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<audioManager>();
        _audioSourceSound = GameObject.FindGameObjectWithTag("Sound").GetComponent<AudioSource>();
        _xmlReader = GameObject.FindGameObjectWithTag("Translate").GetComponent<xmlReader>();

        _xmlReader.SetTextTranslate(_listTextTranslate);
        _xmlReader.SetDropdown(_dropdownLanguage);
        _dropdownLanguage.value = xmlReader.instance.GetLanguage();

        _sliderMusic.value = _audioManager.GetVolumeMusic();
        _sliderSounds.value = _audioManager.GetVolumeSound();
    }

    public void ChangeTranslate()
    {
        if (_xmlReader != null)
        {
            xmlReader.instance.OnLanguageChange();
        }
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("UI");
    }

    public void ChangeSound(float volume)
    {
        _audioManager.ChangeValueSound(volume);
    }

    public void ChangeMusic(float volume)
    {
        _audioManager.ChangeValueMusic(volume);
    }
}
