using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static VFXManager instance;

    [Header("Simple Hit")]
    [SerializeField] GameObject _vfxSimpleHit; // list avec 4 si meme effet pour simple hit et simple hit x2
    [SerializeField] float _vfxTimeSimpleHit;

    [Header("Simple Hit X2")]
    [SerializeField] List<GameObject> _vfxsSimpleHitX2 = new List<GameObject>();
    [SerializeField] float _vfxTimeSimpleHitX2;

    [Header("Alternate Shot")]
    [SerializeField] List<GameObject> _vfxsAlternateShot = new List<GameObject>();
    [SerializeField] float _vfxTimeAlternateShot;

    [Header("Scanner")]
    [SerializeField] List<GameObject> _vfxsScanner = new List<GameObject>();
    [SerializeField] float _vfxTimeScanner;

    [Header("Repair Decoy")]
    [SerializeField] GameObject _vfxRepairDecoy;
    [SerializeField] float _vfxTimeRepairDecoy;

    [Header("Decoy Creation")]
    [SerializeField] GameObject _vfxDecoyCreation;
    [SerializeField] float _vfxTimeDecoyCreation;

    [Header("EMP")]
    [SerializeField] GameObject _vfxEMPCenter;
    [SerializeField] List<GameObject> _vfxsEMPAround = new List<GameObject>();
    [SerializeField] float _vfxTimeEMP;

    [Header("Probe")]
    [SerializeField] List<GameObject> _vfxsProbe = new List<GameObject>();
    [SerializeField] float _vfxTimeProbe;

    [Header("Random Probe")]
    [SerializeField] List<GameObject> _vfxsRandomProbe = new List<GameObject>();
    [SerializeField] float _vfxTimeRandomProbe;

    [Header("Upgrade Shot")]
    [SerializeField] GameObject _vfxUpgradeShot1;
    [SerializeField] GameObject _vfxUpgradeShot2;
    [SerializeField] List<GameObject> _vfxsUpgradeShot3And4 = new List<GameObject>();
    [SerializeField] float _vfxTimeUpgradeShot;
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    // TO DO : mettre bool quand un vfx is playing pour que le joueur puisse pas end turn ni use une autre compétence (sinon potentiels problemes)

    // Play VFX -> change position, enable, wait time anim, disable
    public void PlaySimpleHitVFX(Tile tile)
    {
        _vfxSimpleHit.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -1);
        _vfxSimpleHit.SetActive(true);

        StartCoroutine(DesactivateVFXAfterTime(_vfxSimpleHit, _vfxTimeSimpleHit));

        if (audioManager.instance != null)
            audioManager.instance.PlaySoundGunNormal();
    }

    public void PlaySimpleHitX2VFX(List<Tile> tiles)
    {
        int i = 0;
        foreach (Tile tile in tiles)
        {
            _vfxsSimpleHitX2[i].transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -1);
            _vfxsSimpleHitX2[i].SetActive(true);

            StartCoroutine(DesactivateVFXAfterTime(_vfxsSimpleHitX2[i], _vfxTimeSimpleHitX2));
            i++;
        }

        if (audioManager.instance != null)
            audioManager.instance.PlaySoundExplosion();
    }

    public void PlayAlternateShotVFX(List<Tile> tiles)
    {
        AlternateShotDirection currentAlternateShotDirection;

        if (AbilityButtonsManager.instance.IsInRewind)
        {
            currentAlternateShotDirection = AbilityButtonsManager.instance.GetRewindPlayerAlternateShotDirection();

            if (audioManager.instance != null)
                audioManager.instance.PlaySoundAlternate1();
        }
        else
        {
            currentAlternateShotDirection = AbilityButtonsManager.instance.GetCurrentPlayerAlternateShotDirection();

            if (audioManager.instance != null)
                audioManager.instance.PlaySoundAlternate2();
        }
           
        int i = 0;
        foreach (Tile tile in tiles)
        {
            // rotate vfx ? 
            if (currentAlternateShotDirection == AlternateShotDirection.Horizontal)
                _vfxsAlternateShot[i].transform.eulerAngles = new Vector3(0, 0, 0);
            else
                _vfxsAlternateShot[i].transform.eulerAngles = new Vector3(0, 0, 90);

            _vfxsAlternateShot[i].transform.position = new Vector3(tiles[i].transform.position.x, tiles[i].transform.position.y, -1);
            _vfxsAlternateShot[i].SetActive(true);

            StartCoroutine(DesactivateVFXAfterTime(_vfxsAlternateShot[i], _vfxTimeScanner));
            i++;
        }
    }

    public void PlayScannerVFX(List<Tile> tiles)
    {
        int i = 0;
        foreach(Tile tile in tiles)
        {
            _vfxsScanner[i].transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -1);
            _vfxsScanner[i].SetActive(true);

            StartCoroutine(DesactivateVFXAfterTime(_vfxsScanner[i], _vfxTimeScanner));
            i++;
        }

        if (audioManager.instance != null)
            audioManager.instance.PlaySoundSonar();
    }

    public void PlayRepairDecoyVFX(Tile tile)
    {
        _vfxRepairDecoy.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -1);
        _vfxRepairDecoy.SetActive(true);

        StartCoroutine(DesactivateVFXAfterTime(_vfxRepairDecoy, _vfxTimeRepairDecoy));
    }

    public void PlayDecoyCreationVFX(Tile tile)
    {
        _vfxDecoyCreation.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -1);
        _vfxDecoyCreation.SetActive(true);

        StartCoroutine(DesactivateVFXAfterTime(_vfxDecoyCreation, _vfxTimeDecoyCreation));
    }

    public void PlayEMPVFX(Tile centerTile, List<Tile> aroundTile)
    {
        _vfxEMPCenter.transform.position = new Vector3(centerTile.transform.position.x, centerTile.transform.position.y, -1);
        _vfxEMPCenter.SetActive(true);
        StartCoroutine(DesactivateVFXAfterTime(_vfxEMPCenter, _vfxTimeEMP));

        int i = 0;
        foreach (Tile tile in aroundTile)
        {
            Debug.Log(tile.name);
            _vfxsEMPAround[i].transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -1);
            _vfxsEMPAround[i].SetActive(true);

            StartCoroutine(DesactivateVFXAfterTime(_vfxsEMPAround[i], _vfxTimeEMP));
            i++;
        }

        if (audioManager.instance != null)
            audioManager.instance.PlaySoundLaser();
    }

    IEnumerator DesactivateVFXAfterTime(GameObject vfxToDesactivate, float timeToWait)
    {
        Debug.Log(vfxToDesactivate.transform.position);
        yield return new WaitForSeconds(timeToWait);
        Debug.Log(vfxToDesactivate.transform.position);
        vfxToDesactivate.SetActive(false);

        UIManager.instance.CheckIfShowEndTurnButton();
    }

    public void PlayProbeVFX(Tile tile)
    {
        foreach (GameObject _vfxProbe in _vfxsProbe)
        {
            Debug.Log(tile.name);

            if (!_vfxProbe.activeSelf)
            {
                _vfxProbe.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -1);
                _vfxProbe.SetActive(true);

                if (audioManager.instance != null)
                    audioManager.instance.PlaySoundProbe();

                StartCoroutine(DesactivateVFXAfterTime(_vfxProbe, _vfxTimeProbe));
                return;
            }
        }  
    }

    public void PlayRandomProbeVFX(Tile tile)
    {
        foreach (GameObject _vfxRandomProbe in _vfxsRandomProbe)
        {
            Debug.Log(tile.name);

            if (!_vfxRandomProbe.activeSelf)
            {
                _vfxRandomProbe.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -1);
                _vfxRandomProbe.SetActive(true);

                if (audioManager.instance != null)
                    audioManager.instance.PlaySoundProbe();

                StartCoroutine(DesactivateVFXAfterTime(_vfxRandomProbe, _vfxTimeProbe));
                return;
            }
        }
    }

    public void PlayUpgradeShotVFX(List<Tile> tiles)
    {
        UpgradeShotStep currentUpgradeShotStep;

        if (AbilityButtonsManager.instance.IsInRewind)
            currentUpgradeShotStep = AbilityButtonsManager.instance.GetRewindPlayerUpgradeShotStep();
        else
            currentUpgradeShotStep = AbilityButtonsManager.instance.GetCurrentPlayerUpgradeShotStep();

        int i = 0;
        foreach (Tile tile in tiles)
        {
            switch (currentUpgradeShotStep)
            {
                case (UpgradeShotStep.RevealOneTile):
                    _vfxUpgradeShot1.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -1);
                    _vfxUpgradeShot1.SetActive(true);
                    StartCoroutine(DesactivateVFXAfterTime(_vfxUpgradeShot1, _vfxTimeUpgradeShot));
                    break;
                case (UpgradeShotStep.DestroyOneTile):
                    _vfxUpgradeShot2.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -1);
                    _vfxUpgradeShot2.SetActive(true);
                    StartCoroutine(DesactivateVFXAfterTime(_vfxUpgradeShot1, _vfxTimeUpgradeShot));
                    break;
                case (UpgradeShotStep.DestroyThreeTilesInDiagonal):
                case (UpgradeShotStep.DestroyFiveTilesInCross):
                    _vfxsUpgradeShot3And4[i].transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -1);
                    _vfxsUpgradeShot3And4[i].SetActive(true);
                    StartCoroutine(DesactivateVFXAfterTime(_vfxsUpgradeShot3And4[i], _vfxTimeUpgradeShot));
                    break;
            } 

            i++;
        }

        if (audioManager.instance != null)
            audioManager.instance.PlaySoundUpgradeShot();
    }

    public float GetAnimationTime(string actionName)
    {
        switch (actionName)
        {
            case ("Simple Hit"):
                return _vfxTimeSimpleHit;
            case ("Simple Hit X2"):
                return _vfxTimeSimpleHitX2;
            case ("Alternate Shot"):
                return _vfxTimeAlternateShot;
            case ("Scanner"):
                return _vfxTimeScanner;
            case ("Repair Decoy"):
                return _vfxTimeRepairDecoy;
            case ("EMP"):
                return _vfxTimeEMP;
            case ("Probe"):
                return _vfxTimeProbe;
        }

        return 2;
    }
}
