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

    [Header("Alternate Shot")]
    [SerializeField] List<GameObject> _vfxsAlternateShot = new List<GameObject>();
    [SerializeField] float _vfxTimeAlternateShot;

    [Header("Scanner")]
    [SerializeField] List<GameObject> _vfxsScanner = new List<GameObject>();
    [SerializeField] float _vfxTimeScanner;

    [Header("Repair Decoy")]
    [SerializeField] GameObject _vfxRepairDecoy;
    [SerializeField] float _vfxTimeRepairDecoy;
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    // TO DO : mettre bool quand un vfx is playing pour que le joueur puisse pas end turn (sinon potentiels problemes)

    // Play VFX -> change position, enable, wait time anim, disable
    public void PlaySimpleHitVFX(Tile tile)
    {
        _vfxSimpleHit.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -1);
        _vfxSimpleHit.SetActive(true);

        StartCoroutine(DesactivateVFXAfterTime(_vfxSimpleHit, _vfxTimeSimpleHit));
    }

    public void PlayAlternateShotVFX(List<Tile> tiles)
    {
        AlternateShotDirection currentAlternateShotDirection;

        if (AbilityButtonsManager.instance.IsInRewind)
            currentAlternateShotDirection = AbilityButtonsManager.instance.GetRewindPlayerAlternateShotDirection();
        else
            currentAlternateShotDirection = AbilityButtonsManager.instance.GetCurrentPlayerAlternateShotDirection();

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
    }

    public void PlayRepairDecoyVFX(Tile tile)
    {
        _vfxRepairDecoy.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -1);
        _vfxRepairDecoy.SetActive(true);

        StartCoroutine(DesactivateVFXAfterTime(_vfxRepairDecoy, _vfxTimeRepairDecoy));
    }

    IEnumerator DesactivateVFXAfterTime(GameObject vfxToDesactivate, float timeToWait)
    {
        Debug.Log(vfxToDesactivate.transform.position);
        yield return new WaitForSeconds(timeToWait);
        Debug.Log(vfxToDesactivate.transform.position);
        vfxToDesactivate.SetActive(false);
    }
}
