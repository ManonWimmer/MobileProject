using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static VFXManager instance;

    [Header("Simple Hit")]
    [SerializeField] GameObject _vfxSimpleHit;
    [SerializeField] float _vfxTimeSimpleHit;

    [Header("Alternate Shot")]
    [SerializeField] List<GameObject> _vfxsAlternateShot = new List<GameObject>();
    [SerializeField] float _vfxTimeAlternateShot;

    [Header("Scanner")]
    [SerializeField] List<GameObject> _vfxsScanner = new List<GameObject>();

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
        int i = 0;
        foreach(GameObject vfxAlternateShot in _vfxsAlternateShot)
        {
            // rotate vfx ? 
            if (AbilityButtonsManager.instance.GetCurrentPlayerAlternateShotDirection() == AlternateShotDirection.Horizontal)
                vfxAlternateShot.transform.eulerAngles = new Vector3(0, 0, 0);
            else
                vfxAlternateShot.transform.eulerAngles = new Vector3(0, 0, 90);

            vfxAlternateShot.transform.position = new Vector3(tiles[i].transform.position.x, tiles[i].transform.position.y, -1);
            Debug.Log(vfxAlternateShot.transform.position);
            vfxAlternateShot.SetActive(true);

            i++;
            Debug.Log(vfxAlternateShot.transform.position);
            StartCoroutine(DesactivateVFXAfterTime(vfxAlternateShot, _vfxTimeAlternateShot));
        }    
    }

    IEnumerator DesactivateVFXAfterTime(GameObject vfxToDesactivate, float timeToWait)
    {
        Debug.Log(vfxToDesactivate.transform.position);
        yield return new WaitForSeconds(timeToWait);
        Debug.Log(vfxToDesactivate.transform.position);
        vfxToDesactivate.SetActive(false);
    }
}
