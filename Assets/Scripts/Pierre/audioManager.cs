using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class audioManager : MonoBehaviour
{
    public static audioManager instance;

    [SerializeField] private float _minVolume;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _playlistFight;
    [SerializeField] private AudioClip[] _playlistMenu;
    [SerializeField] public AudioClip[] _playlistFX;

    [Header("SFX")]
    public AudioClip[] _playlistRooms;
    public AudioClip[] _playlistFightMode;
    public AudioClip[] _playlistDestroy;

    [Header("Voice playList")]
    [Header("Voice Win")]
    public AudioClip[] _playlistWinNerd;
    public AudioClip[] _playlistWinCow;
    public AudioClip[] _playlistWinPizza;
    [Header("Voice Hit")]
    public AudioClip[] _playlistHitNerd;
    public AudioClip[] _playlistHitCow;
    public AudioClip[] _playlistHitPizza;
    [Header("Voice Attack")]
    public AudioClip[] _playlistAttackNerd;
    public AudioClip[] _playlistAttackCow;
    public AudioClip[] _playlistAttackPizza;

    [SerializeField] private AudioMixer _mixer;

    private GameManager _gameManager;
    private bool _combatMode;

    private int _index = 0;
    private int _lastIndex = 0;
    private AudioClip[] _actualClip;
    private float _musicV;
    private float _soundV;


    public void PlayMusic() => PlayNextSound();
    public AudioClip[] GetPlaylistFX() => _playlistFX;

    public void PlaySoundDropRoom() => PlaySound(_playlistFX[2]);
    public void PlaySoundMoveRoom() => PlaySound(_playlistFX[3]);
    public void PlaySoundSelectRoom() => PlaySound(_playlistFX[4]);

    public void PlaySoundAlternate1() => PlaySound(_playlistRooms[0]);
    public void PlaySoundAlternate2() => PlaySound(_playlistRooms[1]);
    public void PlaySoundGunNormal() => PlaySound(_playlistRooms[2]);
    public void PlaySoundLaser() => PlaySound(_playlistRooms[3]);
    public void PlaySoundProbe() => PlaySound(_playlistRooms[4]);
    public void PlaySoundSonar() => PlaySound(_playlistRooms[5]);
    public void PlaySoundTimeAccelerator() => PlaySound(_playlistRooms[6]);
    public void PlaySoundUpgradeShot() => PlaySound(_playlistRooms[7]);
    public void PlaySoundDecoyReveal() => PlaySound(_playlistRooms[8]);
    public void PlaySoundExplosion() => PlaySound(_playlistDestroy[1]);


    public AudioClip[] GetPlayListDialogueWin() => FindAudioClipDialogueWin();
    public AudioClip[] GetPlayListDialogueHit() => FindAudioClipDialogueHit();
    public AudioClip[] GetPlayListDialogueAttack() => FindAudioClipDialogueAttack();

    public void ChangeValueMusic(float volume) => SetMusiqueVolume(volume);
    public void ChangeValueSound(float volume) => SetVolume(volume);
    public float GetVolumeMusic() => _musicV;
    public float GetVolumeSound() => _soundV;

    private AudioSource _audioSourceSound;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        _audioSourceSound = GameObject.FindGameObjectWithTag("Sound").GetComponent<AudioSource>();
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
    #region Audio Dialogue
    private AudioClip[] FindAudioClipDialogueWin()
    {
        AudioClip[] clip;

        if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. COWBOY")
        {
            clip = _playlistWinCow;
            return clip;
        }
        else if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. NERD")
        {
            clip = _playlistWinNerd;
            return clip;
        }
        else if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. RAVIOLI")
        {
            clip = _playlistWinPizza;
            return clip;
        } else
        {
            Debug.Log("Pas de clip choisi");
            return clip = null;
        }
    }

    private AudioClip[] FindAudioClipDialogueHit()
    {
        AudioClip[] clip;

        if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. COWBOY")
        {
            clip = _playlistHitCow;
            return clip;
        }
        else if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. NERD")
        {
            clip = _playlistHitNerd;
            return clip;
        }
        else if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. RAVIOLI")
        {
            clip = _playlistHitPizza;
            return clip;
        }
        else
        {
            Debug.Log("Pas de clip choisi");
            return clip = null;
        }
    }

    private AudioClip[] FindAudioClipDialogueAttack()
    {
        AudioClip[] clip;

        if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. COWBOY")
        {
            clip = _playlistAttackCow;
            return clip;
        }
        else if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. NERD")
        {
            clip = _playlistAttackNerd;
            return clip;
        }
        else if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. RAVIOLI")
        {
            clip = _playlistAttackPizza;
            return clip;
        }
        else
        {
            Debug.Log("Pas de clip choisi");
            return clip = null;
        }
    }
    #endregion

    private void PlaySound(AudioClip playlist)
    {
        _audioSourceSound.clip = playlist;
        _audioSourceSound.Play();
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
        _soundV = volume;
        _mixer.SetFloat("volume", volume);
    }

    public void SetMusiqueVolume(float volume)
    {
        _musicV = volume;
        _mixer.SetFloat("musique", volume);
    }
}
