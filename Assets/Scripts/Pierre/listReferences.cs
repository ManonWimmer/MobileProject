using System.Collections;
using System.Collections.Generic;
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

    public xmlReader GetXmlReader() => _xmlReader;

    private void Start()
    {
        _xmlReader = GameObject.FindGameObjectWithTag("Translate").GetComponent <xmlReader>();
        _audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<audioManager>();

        _sliderMusic.value = _audioManager.GetVolumeMusic();
        _sliderSounds.value = _audioManager.GetVolumeSound();

        _dropdownLanguage.value = _xmlReader.GetLanguage();
        _xmlReader.SetTextTranslate(_listTextTranslate);
        _xmlReader.SetDropdown(_dropdownLanguage);
    }

    public void ChangeTranslate()
    {
        _xmlReader.OnLanguageChange();
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
