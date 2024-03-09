using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomsAssetsManager : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static RoomsAssetsManager instance;

    [Header("Vital Room")]
    [SerializeField] Sprite _vital;
    [SerializeField] Sprite _vitalRevealed;
    [SerializeField] Sprite _vitalDestroyed;

    [Header("Alternate Shot")]
    [SerializeField] Sprite _alternateShot;
    [SerializeField] Sprite _alternateShotRevealed;
    [SerializeField] Sprite _alternateShotDestroyed;

    [Header("Capacitor")]
    [SerializeField] Sprite _capacitor;
    [SerializeField] Sprite _capacitorRevealed;
    [SerializeField] Sprite _capacitorDestroyed;

    [Header("EMP")]
    [SerializeField] Sprite _emp;
    [SerializeField] Sprite _empRevealed;
    [SerializeField] Sprite _empDestroyed;

    [Header("Probe")]
    [SerializeField] Sprite _probe;
    [SerializeField] Sprite _probeRevealed;
    [SerializeField] Sprite _probeDestroyed;

    [Header("Scanner")]
    [SerializeField] Sprite _scanner;
    [SerializeField] Sprite _scannerRevealed;
    [SerializeField] Sprite _scannerDestroyed;

    [Header("Time Accelerator")]
    [SerializeField] Sprite _timeAccelerator;
    [SerializeField] Sprite _timeAcceleratorRevealed;
    [SerializeField] Sprite _timeAcceleratorDestroyed;

    [Header("Upgrade Shot")]
    [SerializeField] Sprite _upgradeShot;
    [SerializeField] Sprite _upgradeShotRevealed;
    [SerializeField] Sprite _upgradeShotDestroyed;

    [Header("Bomb")]
    [SerializeField] Sprite _bomb;
    [SerializeField] Sprite _bombRevealed;
    [SerializeField] Sprite _bombDestroyed;

    [Header("Shield")]
    [SerializeField] Sprite _shield;
    [SerializeField] Sprite _shieldRevealed;
    [SerializeField] Sprite _shieldDestroyed;

    [Header("Random Reveal")]
    [SerializeField] Sprite _randomReveal;
    [SerializeField] Sprite _randomRevealRevealed;
    [SerializeField] Sprite _randomRevealDestroyed;

    [Header("Energy Decoy")]
    [SerializeField] Sprite _energyDecoy;
    [SerializeField] Sprite _energyDecoyRevealed;
    [SerializeField] Sprite _energyDecoyDestroyed;
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    public void SetTileRoomAsset(scriptablePower ability, SpriteRenderer spriteRenderer, bool isDestroyed, bool isRevealed)
    {
        Debug.Log("set tile room asset " + ability.name);

        switch (ability.AbilityName)
        {
            case ("Simple Hit"):
                if (isDestroyed)
                    spriteRenderer.sprite = _vitalDestroyed;
                else if (isRevealed)
                    spriteRenderer.sprite = _vitalRevealed;
                else
                    spriteRenderer.sprite = _vital;
                break;

            case ("Alternate Shot"):
                if (isDestroyed)
                    spriteRenderer.sprite = _alternateShotDestroyed;
                else if (isRevealed)
                    spriteRenderer.sprite = _alternateShotRevealed;
                else
                    spriteRenderer.sprite = _alternateShot;
                break;

            case ("Capacitor"):
                if (isDestroyed)
                    spriteRenderer.sprite = _capacitorDestroyed;
                else if (isRevealed)
                    spriteRenderer.sprite = _capacitorRevealed;
                else
                    spriteRenderer.sprite = _capacitor;
                break;

            case ("EMP"):
                if (isDestroyed)
                    spriteRenderer.sprite = _empDestroyed;
                else if (isRevealed)
                    spriteRenderer.sprite = _empRevealed;
                else
                    spriteRenderer.sprite = _emp;
                break;

            case ("Probe"):
                if (isDestroyed)
                    spriteRenderer.sprite = _probeDestroyed;
                else if (isRevealed)
                    spriteRenderer.sprite = _probeRevealed;
                else
                    spriteRenderer.sprite = _probe;
                break;

            case ("Scanner"):
                if (isDestroyed)
                    spriteRenderer.sprite = _scannerDestroyed;
                else if (isRevealed)
                    spriteRenderer.sprite = _scannerRevealed;
                else
                    spriteRenderer.sprite = _scanner;
                break;

            case ("Time Accelerator"):
                if (isDestroyed)
                    spriteRenderer.sprite = _timeAcceleratorDestroyed;
                else if (isRevealed)
                    spriteRenderer.sprite = _timeAcceleratorRevealed;
                else
                    spriteRenderer.sprite = _timeAccelerator;
                break;

            case ("Upgrade Shot"):
                if (isDestroyed)
                    spriteRenderer.sprite = _upgradeShotDestroyed;
                else if (isRevealed)
                    spriteRenderer.sprite = _upgradeShotRevealed;
                else
                    spriteRenderer.sprite = _upgradeShot;
                break;

            case ("Bomb"):
                if (isDestroyed)
                    spriteRenderer.sprite = _bombDestroyed;
                else if (isRevealed)
                    spriteRenderer.sprite = _bombRevealed;
                else
                    spriteRenderer.sprite = _bomb;
                break;

            case ("Shield"):
                if (isDestroyed)
                    spriteRenderer.sprite = _shieldDestroyed;
                else if (isRevealed)
                    spriteRenderer.sprite = _shieldRevealed;
                else
                    spriteRenderer.sprite = _shield;
                break;

            case ("Random Reveal"):
                if (isDestroyed)
                    spriteRenderer.sprite = _randomRevealDestroyed;
                else if (isRevealed)
                    spriteRenderer.sprite = _randomRevealRevealed;
                else
                    spriteRenderer.sprite = _randomReveal;
                break;

            case ("Energy Decoy"):
                if (isDestroyed)
                    spriteRenderer.sprite = _energyDecoyDestroyed;
                else if (isRevealed)
                    spriteRenderer.sprite = _energyDecoyRevealed;
                else
                    spriteRenderer.sprite = _energyDecoy;
                break;
        }
    }
}
