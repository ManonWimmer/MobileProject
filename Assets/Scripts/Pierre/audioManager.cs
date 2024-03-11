using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class audioManager : MonoBehaviour
{
    [SerializeField] private float _minVolume;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _playlistFight;
    [SerializeField] private AudioClip[] _playlistMenu;
    [SerializeField] public AudioClip[] _playlistFX;
    [SerializeField] public AudioClip[] _playlistWin;
    [SerializeField] private AudioMixer _mixer;

    private GameManager _gameManager;
    private bool _combatMode;

    private int _index = 0;
    private int _lastIndex = 0;
    private AudioClip[] _actualClip;

    public void PlayMusic() => PlayNextSound();
    public AudioClip[] GetPlaylistFX() => _playlistFX;

    void Start()
    {
        _actualClip = _playlistMenu;
        PlayNextSound();
    }

    void Update()
    {
        if (!_audioSource.isPlaying)
        {
            PlayNextSound();
        }

        float currentVolume;
        bool result = _mixer.GetFloat("volume", out currentVolume);

        float currentVolumeMusique;
        bool resultMusique = _mixer.GetFloat("musique", out currentVolumeMusique);

        if (result && currentVolume == _minVolume)
        {
            _mixer.SetFloat("volume", -80f);
        }
        if (resultMusique && currentVolumeMusique == _minVolume)
        {
            _mixer.SetFloat("musique", -80f);
        }
    }

    public void ChangeMode()
    {
        if (_gameManager.GetCurrentMode() == Mode.Combat)
        {
            _combatMode = true;
        }
        else
        {
            _combatMode = false;
        }

        if (_combatMode)
        {
            _actualClip = _playlistFight;
            PlayNextSound();
        } else
        {
            _actualClip = _playlistMenu;
            PlayNextSound();
        }
    }

    private void PlayNextSound()
    {
        if (_playlistMenu.Length == 0)
        {
            Debug.LogWarning("La playlist est vide.");
            return;
        }

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, _playlistMenu.Length);
        } while (randomIndex == _lastIndex);

        _lastIndex = randomIndex;
        _index = randomIndex;

        _audioSource.clip = _playlistMenu[_index];
        _audioSource.Play();
    }

    public void SetVolume(float volume)
    {
        _mixer.SetFloat("volume", volume);
    }

    public void SetMusiqueVolume(float volume)
    {
        _mixer.SetFloat("musique", volume);
    }
}
