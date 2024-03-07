using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pause : MonoBehaviour
{
    [SerializeField] private GameObject _pausePage;
    [SerializeField] private AudioSource _audioOpenClose;
    [SerializeField] private audioManager _audioManager;

    private bool _paused;

    public void OpenPage()
    {
        if (!_paused)
        {
            _pausePage.SetActive(true);
            _audioOpenClose.clip = _audioManager._playlistFX[0];
            _audioOpenClose.Play();

            _paused = true;
        } else
        {
            ClosePage();
        }
    }

    public void ClosePage()
    {
        _audioOpenClose.clip = _audioManager._playlistFX[1];
        _audioOpenClose.Play();
        _pausePage.SetActive(false);

        _paused = false;
    }
}
