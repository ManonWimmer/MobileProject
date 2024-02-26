using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class audioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _playlist;
    [SerializeField] private AudioMixer _mixer;

    private int _index = 0;

    void Start()
    {
        _audioSource.clip = _playlist[0];
        _audioSource.Play();
    }

    void Update()
    {
        if (!_audioSource.isPlaying)
        {
            PlayNextSound();
        }
    }

    private void PlayNextSound()
    {
        _index = (_index + 1) % _playlist.Length;
        _audioSource.clip = _playlist[_index];
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
