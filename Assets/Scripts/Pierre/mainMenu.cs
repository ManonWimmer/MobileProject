using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainMenu : MonoBehaviour
{
    private GameObject _menu;
    [SerializeField] private GameObject _settings;
    [SerializeField] private GameObject _credits;

    [SerializeField] private AudioClip[] _playlistFXUI;
    [SerializeField] private AudioSource _audioOpenClose;
    [SerializeField] private AudioSource _music;
    [SerializeField] private audioManager _audioManager;

    private bool _easterEggPlay;

    private void Start()
    {
        _menu = GetComponent<GameObject>();
        _easterEggPlay = false;
    }

    private void Update()
    {

        if (!_music.isPlaying )
        {
            _audioManager.PlayMusic();
            _easterEggPlay = false;
        }
    }

    public void StartDuel()
    {

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
        if(_easterEggPlay)
        {
            _audioManager.PlayMusic();
            _easterEggPlay = false;
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
        _easterEggPlay = true;
        _music.clip = _playlistFXUI[2];
        _music.Play();
    }
}
