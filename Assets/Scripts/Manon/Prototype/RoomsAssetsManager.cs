using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomsAssetsManager : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static RoomsAssetsManager instance;

    [Header("Vital Room")]
    [SerializeField] Sprite _vitalRevealed;
    [SerializeField] Sprite _vitalDestroyed;

    [Header("Alternate Shot")]
    [SerializeField] Sprite _alternateShotRevealed;
    [SerializeField] Sprite _alternateShotDestroyed;

    [Header("Capacitor")]
    [SerializeField] Sprite _capacitorRevealed;
    [SerializeField] Sprite _capacitorDestroyed;

    [Header("EMP")]
    [SerializeField] Sprite _empRevealed;
    [SerializeField] Sprite _empDestroyed;

    [Header("Probe")]
    [SerializeField] Sprite _probeRevealed;
    [SerializeField] Sprite _probeDestroyed;

    [Header("Scanner")]
    [SerializeField] Sprite _scannerRevealed;
    [SerializeField] Sprite _scannerDestroyed;

    [Header("Time Accelerator")]
    [SerializeField] Sprite _timeAcceleratorRevealed;
    [SerializeField] Sprite _timeAcceleratorDestroyed;

    [Header("Upgrade Shot")]
    [SerializeField] Sprite _upgradeShotRevealed;
    [SerializeField] Sprite _upgradeShotDestroyed;

    [Header("Bomb")]
    [SerializeField] Sprite _bombRevealed;
    [SerializeField] Sprite _bombDestroyed;

    [Header("Shield")]
    [SerializeField] Sprite _shieldRevealed;
    [SerializeField] Sprite _shieldDestroyed;

    [Header("Random Reveal")]
    [SerializeField] Sprite _randomRevealRevealed;
    [SerializeField] Sprite _randomRevealDestroyed;

    [Header("Energy Decoy")]
    [SerializeField] Sprite _energyDecoyRevealed;
    [SerializeField] Sprite _energyDecoyDestroyed;
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    public void SetTileRoomAsset(scriptablePower ability, SpriteRenderer spriteRenderer, bool isDestroyed)
    {
        Debug.Log("set tile room asset " + ability.name);

        switch (ability.AbilityName)
        {
            case ("Simple Hit"):
                if (!isDestroyed)
                    spriteRenderer.sprite = _vitalRevealed;
                else
                    spriteRenderer.sprite = _vitalDestroyed;
                break;
            case ("Alternate Shot"):
                if (!isDestroyed)
                    spriteRenderer.sprite = _alternateShotRevealed;
                else
                    spriteRenderer.sprite = _alternateShotDestroyed;
                break;
            case ("Capacitor"):
                if (!isDestroyed)
                    spriteRenderer.sprite = _capacitorRevealed;
                else
                    spriteRenderer.sprite = _capacitorDestroyed;
                break;
            
            case ("EMP"):
                if (!isDestroyed)
                    spriteRenderer.sprite = _empRevealed;
                else
                    spriteRenderer.sprite = _empDestroyed;
                break;
            case ("Probe"):
                if (!isDestroyed)
                    spriteRenderer.sprite = _probeRevealed;
                else
                    spriteRenderer.sprite = _probeDestroyed;
                break;
            case ("Scanner"):
                if (!isDestroyed)
                    spriteRenderer.sprite = _scannerRevealed;
                else
                    spriteRenderer.sprite = _scannerDestroyed;
                break;
            case ("Time Accelerator"):
                if (!isDestroyed)
                    spriteRenderer.sprite = _timeAcceleratorRevealed;
                else
                    spriteRenderer.sprite = _timeAcceleratorDestroyed;
                break;
            case ("Upgrade Shot"):
                if (!isDestroyed)
                    spriteRenderer.sprite = _upgradeShotRevealed;
                else
                    spriteRenderer.sprite = _upgradeShotDestroyed;
                break;
            case ("Bomb"):
                if (!isDestroyed)
                    spriteRenderer.sprite = _bombRevealed;
                else
                    spriteRenderer.sprite = _bombDestroyed;
                break;
            case ("Shield"):
                if (!isDestroyed)
                    spriteRenderer.sprite = _shieldRevealed;
                else
                    spriteRenderer.sprite = _shieldDestroyed;
                break;
            case ("Random Reveal"):
                if (!isDestroyed)
                    spriteRenderer.sprite = _randomRevealRevealed;
                else
                    spriteRenderer.sprite = _randomRevealDestroyed;
                break;
            case ("Energy Decoy"):
                if (!isDestroyed)
                    spriteRenderer.sprite = _energyDecoyRevealed;
                else
                    spriteRenderer.sprite = _energyDecoyDestroyed;
                break;
        }
    }
}
