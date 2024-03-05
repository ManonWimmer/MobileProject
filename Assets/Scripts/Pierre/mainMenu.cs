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

    private void Start()
    {
        _menu = GetComponent<GameObject>();
    }

    public void StartDuel()
    {

    }

    public void StartTuto()
    {

    }

    public void OpenSettings()
    {
        _settings.SetActive(true);

        _audioOpenClose.clip = _playlistFXUI[0];
        _audioOpenClose.Play();
    }

    public void CloseSettings()
    {
        _settings.SetActive(false);

        _audioOpenClose.clip = _playlistFXUI[1];
        _audioOpenClose.Play();
    }

    public void CloseCredits()
    {
        _credits.SetActive(false);

        _audioOpenClose.clip = _playlistFXUI[1];
        _audioOpenClose.Play();
    }

    public void OpenCredits()
    {
        _credits.SetActive(true);

        _audioOpenClose.clip = _playlistFXUI[0];
        _audioOpenClose.Play();
    }

    public void Leave()
    {
        Application.Quit();
    }
}
