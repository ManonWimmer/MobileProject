using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    private GameObject _menu;
    [SerializeField] private GameObject _settings;
    [SerializeField] private GameObject _credits;
    [SerializeField] private RectTransform _creditsSroll;

    private AudioClip[] _playlistFXUI;
    private AudioSource _audioOpenClose;
    private AudioSource _music;
    private audioManager _audioManager;
    [SerializeField] private GameObject _conffeti; 

    private bool _easterEggPlay;
    private Vector2 _poseCredits;

    private void Start()
    {
        _menu = GetComponent<GameObject>();
        _audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<audioManager>();
        _music = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
        _audioOpenClose = GameObject.FindGameObjectWithTag("Sound").GetComponent<AudioSource>();
        _playlistFXUI = _audioManager.GetPlaylistFX();

        _poseCredits = _creditsSroll.anchoredPosition;
        _easterEggPlay = false;
    }

    private void Update()
    {

        if (!_music.isPlaying )
        {
            _conffeti.GetComponent<ParticleSystem>().Stop();
            _easterEggPlay = false;
        }
    }

    public void StartDuel()
    {
        SceneManager.LoadScene("Prototype_Manon");
    }

    public void StartTuto()
    {

    }

    public void OpenSettings()
    {
        _audioOpenClose.clip = _playlistFXUI[0];
        _audioOpenClose.Play();

        _settings.SetActive(true);
    }

    public void CloseSettings()
    {
        _audioOpenClose.clip = _playlistFXUI[1];
        _audioOpenClose.Play();

        _settings.SetActive(false);
    }

    public void CloseCredits()
    {
        _creditsSroll.anchoredPosition = _poseCredits;

        if (_easterEggPlay)
        {
            _easterEggPlay = false;
            _audioManager.PlayMusic();
            _music.Play();
        }

        _audioOpenClose.clip = _playlistFXUI[1];
        _audioOpenClose.Play();

        _credits.SetActive(false);
    }

    public void OpenCredits()
    {
        _audioOpenClose.clip = _playlistFXUI[0];
        _audioOpenClose.Play();

        _credits.SetActive(true);
    }

    public void Leave()
    {
        Application.Quit();
    }

    public void EasterEgg()
    {
        _conffeti.GetComponent<ParticleSystem>().Play();

        _easterEggPlay = true;
        _music.clip = _playlistFXUI[2];
        _music.Play();
    }
}
