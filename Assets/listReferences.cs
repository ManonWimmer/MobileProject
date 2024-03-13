using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class listReferences : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> _listTextTranslate;
    [SerializeField] private Slider _sliderMusic;
    [SerializeField] private Slider _sliderSounds;
    [SerializeField] private TMP_Dropdown _dowpdownLanguage;

    private AudioParameterSync _audioParameterSync;
    private xmlReader _xmlReader;

    public AudioParameterSync GetRefSync() => _audioParameterSync;
    public xmlReader GetXmlReader() => _xmlReader;

    private void Start()
    {
        _audioParameterSync = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioParameterSync>();
        _xmlReader = GameObject.FindGameObjectWithTag("Translate").GetComponent <xmlReader>();

        _sliderMusic.value = _audioParameterSync.GetMusic();
        _sliderSounds.value = _audioParameterSync.GetSounds();
        _dowpdownLanguage.value = _xmlReader.GetLanguage();
        _xmlReader.SetTextTranslate(_listTextTranslate);
        _xmlReader.SetDropdown(_dowpdownLanguage);
    }

    public void ChangeTranslate()
    {
        _xmlReader.OnLanguageChange();
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("UI");
    }
}
