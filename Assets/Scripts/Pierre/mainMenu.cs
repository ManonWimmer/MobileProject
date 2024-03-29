using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    private GameObject _menu;
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
            _conffeti.GetComponent<ParticleSystem>().Pause();
            _easterEggPlay = false;
        }
    }

    public void StartDuel()
    {
        _audioOpenClose.clip = _playlistFXUI[0];
        _audioOpenClose.Play();
        SceneManager.LoadScene("Prototype_Manon");
    }

    public void Open(GameObject obj)
    {
        _audioOpenClose.clip = _playlistFXUI[0];
        _audioOpenClose.Play();

        obj.SetActive(true);

        if ( obj.name == "tuto")
        {
            Debug.Log("StartTuto");
            obj.GetComponent<tuto>().StartDialogueTuto();
        }
    }

    public void Close(GameObject obj)
    {
        _audioOpenClose.clip = _playlistFXUI[1];
        _audioOpenClose.Play();
        obj.SetActive(false);
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

    public void Leave()
    {
        Application.Quit();
    }

    public void EasterEgg()
    {
        _conffeti.GetComponent<ParticleSystem>().Play();

        _easterEggPlay = true;
        _music.clip = _playlistFXUI[6];
        _music.Play();
    }
}
