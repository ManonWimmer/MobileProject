using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pause : MonoBehaviour
{
    [SerializeField] private GameObject _pausePage;
    [SerializeField] private listReferences _listRef;
    private AudioSource _audioSource;
    private audioManager _audioManager;

    private bool _paused;

    private void Start()
    {
        _audioSource = GameObject.FindGameObjectWithTag("Sound").GetComponent<AudioSource>();
        _audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<audioManager>();
    }

    public void OpenPage()
    {
        if (!_paused)
        {
            _pausePage.SetActive(true);
            _audioSource.clip = _audioManager._playlistFX[0];
            _audioSource.Play();

            _paused = true;
        } else
        {
            ClosePage();
        }
    }

    public void ClosePage()
    {
        _audioSource.clip = _audioManager._playlistFX[1];
        _audioSource.Play();
        _pausePage.SetActive(false);

        _paused = false;
    }
}
