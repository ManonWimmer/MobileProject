using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DraftShip : MonoBehaviour
{
    // ----- FIELDS ----- //
    [SerializeField] Image _captainImg;
    [SerializeField] Image _shipImg;
    [SerializeField] TMP_Text _captainName;
    [SerializeField] Image _selectedBackground;

    [SerializeField] int _shipIndex;

    private Image _image;

    private ShipSO _shipData;
    private Ship _ship;
    // ----- FIELDS ----- //

    private void Start()
    {
        _image = GetComponent<Image>();
    }

    public void InitDraftShip(Ship ship)
    {
        Debug.Log("init draft ship " + _shipIndex);
        _ship = ship;
        _shipData = ship.ShipData;

        _captainImg.sprite = _shipData.CaptainImg;
        _shipImg.sprite = _shipData.ShipImg;
        _captainName.text = _shipData.CaptainName.ToUpper();
    }

    public void SelectDraftShip() // on click
    {
        Debug.Log("select ship " + _shipIndex);
        DraftManager.instance.SelectShip(_shipIndex);
    }

    public Ship GetShip()
    {
        return _ship;
    }

    public void SelectShipUI()
    {
        Debug.Log("select ship ui");
        _selectedBackground.enabled = true;
    }

    public void DeselectShipUI()
    {
        Debug.Log("deselect ship ui");
        _selectedBackground.enabled = false;
    }
}
