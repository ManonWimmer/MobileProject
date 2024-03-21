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
    Animator _animator;

    [SerializeField] int _shipIndex;

    private Image _image;

    private ShipSO _shipData;
    private Ship _ship;
    // ----- FIELDS ----- //

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();

    }

    private void Start()
    {
        _image = GetComponent<Image>();
    }

    public void InitDraftShip(Ship ship)
    {
        Debug.Log("init draft ship " + _shipIndex);
        _ship = ship;
        _shipData = ship.ShipData;

        _captainImg.sprite = _shipData.CaptainImgCut;
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
        _animator.SetTrigger("Selected");
        _animator.SetBool("Unselected", false);
        _shipImg.color = new Color(0.094f, 0.09f, 0.15f, 1f);
    }

    public void DeselectShipUI()
    {
        Debug.Log("deselect ship ui");
        _animator.SetBool("Unselected", true);
        _shipImg.color = new Color(0.34f, 0.54f, 0.76f, 1f);
    }
}
